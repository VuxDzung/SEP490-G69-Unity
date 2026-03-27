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
        [SerializeField] private TextMeshProUGUI m_CostTmp;
        [SerializeField] private Image m_CardTypeImg;

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
            _onSelect = null;
            RawCardId = string.Empty;
            m_CardNameTmp.text = string.Empty;
            m_CardDescTmp.text = string.Empty;
            m_CardIcon.sprite = null;
        }

        public UICardElement SetOnSelectCallback(Action<string, bool, Transform> onSelect)
        {
            m_BtnRef.onClick.RemoveListener(Select);
            m_BtnRef.onClick.AddListener(Select);
            _onSelect = onSelect;
            return this;
        }

        public UICardElement SetOnDragEnd(Action<string, Transform> onDragEnd)
        {
            _onDragEnd = onDragEnd;
            return this;
        }

        public UICardElement SetContent(string cardId, string cardName, string cardDesc, Sprite icon)
        {
            RawCardId = cardId;
            m_CardNameTmp.text = cardName;
            m_CardDescTmp.text = cardDesc;
            m_CardIcon.sprite = icon;
            return this;
        }

        public UICardElement SetCost(int cost)
        {
            if (m_CostTmp != null)
            {
                m_CostTmp.text = cost.ToString();
            }
            return this;
        }

        public UICardElement SetCardTypeSprite(Sprite sprite)
        {
            if (m_CardTypeImg != null)
            {
                if (sprite == null)
                {
                    m_CardTypeImg.enabled = false;
                }
                m_CardTypeImg.enabled = true;
                m_CardTypeImg.sprite = sprite;
            }
            return this;
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