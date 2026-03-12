namespace SEP490G69.Battle.Cards
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICardElement : UIDragableElement, IPooledObject
    {
        private Action<string, bool, Transform> _onSelect;
        private Action<string, Transform> _onDragEnd;

        [SerializeField] private Image m_CardIcon;
        [SerializeField] private Image m_Border;
        [SerializeField] private TextMeshProUGUI m_CardNameTmp;
        [SerializeField] private TextMeshProUGUI m_CardDescTmp;
        [SerializeField] private Button m_BtnRef;

        public string RawCardId { get; private set; }

        private bool _isSelected;

        public void Spawn()
        {
            onDropped += Drop;
        }

        public void Despawn()
        {
            onDropped -= Drop;
            _onSelect = null;
            _onDragEnd = null;
        }

        public UICardElement SetOnSelectCallback(Action<string, bool, Transform> onSelect)
        {
            m_BtnRef.onClick.RemoveListener(Select);
            m_BtnRef.onClick.AddListener(Select);
            _onSelect = null;
            _onSelect = onSelect;
            return this;
        }

        public UICardElement SetOnDragEnd(Action<string, Transform> onDragEnd)
        {
            _onDragEnd = onDragEnd;
            return this;
        }

        public void SetContent(string cardId, string cardName, string cardDesc, Sprite icon)
        {
            RawCardId = cardId;
            m_CardNameTmp.text = cardName;
            m_CardDescTmp.text = cardDesc;
            m_CardIcon.sprite = icon;
        }

        private void Select()
        {
            _isSelected = !_isSelected;

            m_Border.enabled = _isSelected;
            _onSelect?.Invoke(RawCardId, _isSelected, this.transform);
        }

        public void Deselect()
        {
            _isSelected = false;
            m_Border.enabled = false;
        }

        private void Drop(Transform cardTransform, Transform parent)
        {
            _onDragEnd?.Invoke(RawCardId, parent);
        }
    }
}