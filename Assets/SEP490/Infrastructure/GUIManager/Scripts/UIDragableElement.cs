namespace SEP490G69
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIDragableElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Action<Transform, Transform> onDropped;

        public Transform _onDragParent;

        [SerializeField] private Transform _originalParent;
        private Transform _dropParent;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDisable()
        {
            Debug.Log("<color=green>[UIDragableElement.OnDisable]</color>");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalParent = transform.parent;

            _dropParent = null;

            transform.SetParent(_onDragParent);

            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;

            Transform finalParent = _dropParent != null ? _dropParent : _originalParent;

            transform.SetParent(finalParent);
            _rectTransform.anchoredPosition = Vector2.zero;
            onDropped?.Invoke(this.transform, finalParent);
        }

        public void SetDropParent(Transform parent)
        {
            _dropParent = parent;
        }
    }
}