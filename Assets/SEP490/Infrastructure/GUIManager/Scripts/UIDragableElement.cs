namespace SEP490G69
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UIDragableElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Action<Transform, Transform> onDropped;

        public Transform _onDragParent;

        [SerializeField] private Transform _originalParent;
        [SerializeField] private Image m_ImageTarget;
        [SerializeField] private Color m_DraggableColor = Color.white;
        [SerializeField] private Color m_UndragableColor = Color.grey;

        private Transform _dropParent;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _isDraggable;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDisable()
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isDraggable == false) return;
            _originalParent = transform.parent;

            _dropParent = null;

            transform.SetParent(_onDragParent);

            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDraggable == false) return;

            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isDraggable == false) return;

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

        public virtual void EnableDrag()
        {
            _isDraggable = true;
            if (m_ImageTarget != null)
            {
                m_ImageTarget.color = m_DraggableColor;
            }
        }
        public virtual void DisableDrag()
        {
            _isDraggable = false;
            if (m_ImageTarget != null)
            {
                m_ImageTarget.color = m_UndragableColor;
            }
        }
    }
}