namespace SEP490G69
{
    using UnityEngine;

    public class ObjectSelector2D : MonoBehaviour
    {
        void Update()
        {
            // Check for left mouse button click (or tap)
            if (Input.GetMouseButtonDown(0))
            {
                // Convert mouse position from screen space to world space
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Check if any collider overlaps with the mouse position
                Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

                if (hitCollider != null && hitCollider.TryGetComponent<BaseSceneObject>(out BaseSceneObject selectedObj))
                {
                    // A 2D object was clicked, you can now access it
                    Debug.Log("<color=green>[ObjectSelector2D]</color> Selected object: " + selectedObj.ObjectId);

                    // Add your selection logic here (e.g., highlight the object, call a function)
                    // Example: selectedObject.GetComponent<MyObjectScript>().SelectMe();
                    HandleObjectSelection(selectedObj);
                }
            }
        }

        protected virtual void HandleObjectSelection(BaseSceneObject selectedObj)
        {

        }
    }
}