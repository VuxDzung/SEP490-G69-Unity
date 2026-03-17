namespace SEP490G69
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using System.Collections.Generic;

    public class ObjectSelector2D : MonoBehaviour
    {
        private EventSystem eventSystem;
        private PointerEventData pointerData;
        private List<RaycastResult> results = new List<RaycastResult>();

        void Awake()
        {
            eventSystem = EventSystem.current;
        }

        void Update()
        {
            // Mobile
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    if (IsPointerOverUI(touch.position)) return;

                    HandleSelection(touch.position);
                }
            }
            // PC
            else if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUI(Input.mousePosition)) return;

                HandleSelection(Input.mousePosition);
            }
        }

        private bool IsPointerOverUI(Vector2 screenPosition)
        {
            if (eventSystem == null) return false;

            pointerData = new PointerEventData(eventSystem);
            pointerData.position = screenPosition;

            results.Clear();
            eventSystem.RaycastAll(pointerData, results);

            return results.Count > 0;
        }

        private void HandleSelection(Vector2 screenPosition)
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);

            if (hitCollider != null && hitCollider.TryGetComponent<BaseSceneObject>(out BaseSceneObject selectedObj))
            {
                Debug.Log("<color=green>[ObjectSelector2D]</color> Selected object: " + selectedObj.ObjectId);
                HandleObjectSelection(selectedObj);
            }
        }

        protected virtual void HandleObjectSelection(BaseSceneObject selectedObj)
        {
            selectedObj.Interact();
        }
    }
}