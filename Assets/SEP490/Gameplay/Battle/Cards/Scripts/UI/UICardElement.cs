namespace SEP490G69.Battle.Cards
{
    using System;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICardElement : MonoBehaviour, IPooledObject
    {
        private Action<string, bool, Transform> _onSelect;

        [SerializeField] private Image m_CardIcon;
        [SerializeField] private Image m_Border;
        [SerializeField] private TextMeshProUGUI m_CardNameTmp;
        [SerializeField] private TextMeshProUGUI m_CardDescTmp;
        [SerializeField] private Button m_BtnRef;

        private string _cardId;
        private bool _isSelected;

        public void Spawn()
        {
            m_BtnRef.onClick.AddListener(Select);
        }

        public void Despawn()
        {
            m_BtnRef.onClick.RemoveListener(Select);
            _onSelect = null;
            Deselect();
        }

        public UICardElement SetOnSelectCallback(Action<string, bool, Transform> onSelect)
        {
            _onSelect = onSelect;
            return this;
        }
        public void SetContent(string cardId, string cardName, string cardDesc, Sprite icon)
        {
            _cardId = cardId;
            m_CardNameTmp.text = cardName;
            m_CardDescTmp.text = cardDesc;
            m_CardIcon.sprite = icon;
        }

        private void Select()
        {
            _isSelected = !_isSelected;

            m_Border.enabled = _isSelected;
            _onSelect?.Invoke(_cardId, _isSelected, this.transform);
        }

        public void Deselect()
        {
            _isSelected = false;
            m_Border.enabled = false;
        }
    }
}