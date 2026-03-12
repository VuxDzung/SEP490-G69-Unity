namespace SEP490G69
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIDropHandler : MonoBehaviour, IDropHandler
    {
        [SerializeField] private string m_HandlerName;
        [SerializeField] private Transform m_DropContainer;

        public string HandlerName => m_HandlerName;

        public void OnDrop(PointerEventData eventData)
        {
            UIDragableElement drag = eventData.pointerDrag.GetComponent<UIDragableElement>();
            Debug.Log($"Prepare to set drop parent: {m_HandlerName}");

            if (drag == null)
            {
                Debug.Log($"[UIDropHandler:{m_HandlerName}] No dragable element");
                return;
            }

            Debug.Log($"Set drop parent: {m_HandlerName}");
            drag.SetDropParent(m_DropContainer);
        }
    }
}