namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Addons.Localization;
    using SEP490G69.Battle.Cards;
    using System;
    using System.Collections.Generic;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIUniqueCardElement : MonoBehaviour, IPooledObject
    {
        private Action<string, bool, Transform> _onSelect;

        [SerializeField] private Image m_CardIcon;
        [SerializeField] private TextMeshProUGUI m_CardNameTmp;
        [SerializeField] private TextMeshProUGUI m_CardDescTmp;
        [SerializeField] private Button m_BtnRef;

        private string _cardId;

        public void Spawn()
        {
            if (m_BtnRef != null)
                m_BtnRef.onClick.AddListener(OnClick);
        }

        public void Despawn()
        {
            if (m_BtnRef != null)
                m_BtnRef.onClick.RemoveListener(OnClick);

            _onSelect = null;
            _cardId = string.Empty;
        }

        public UIUniqueCardElement SetOnSelectCallback(Action<string, bool, Transform> onSelect)
        {
            _onSelect = onSelect;
            return this;
        }

        public void SetContent(string cardId, string cardName, string cardDesc, Sprite icon)
        {
            _cardId = cardId;

            if (m_CardNameTmp != null) m_CardNameTmp.text = cardName;
            if (m_CardDescTmp != null) m_CardDescTmp.text = cardDesc;

            if (m_CardIcon != null && icon != null)
            {
                m_CardIcon.sprite = icon;
                m_CardIcon.color = Color.white; // Đảm bảo icon hiện rõ
            }
        }

        private void OnClick()
        {
            // Truyền ID thẻ bài, true (trạng thái chọn), và transform của thẻ hiện tại ra ngoài
            _onSelect?.Invoke(_cardId, true, this.transform);
        }
    }
}