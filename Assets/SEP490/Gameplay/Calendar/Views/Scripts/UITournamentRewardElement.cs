namespace SEP490G69.Calendar
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITournamentRewardElement : MonoBehaviour, IPooledObject
    {
        private Action<string, ERewardType> _onClick;
        private string _rewardId;
        private ERewardType _rewardType;

        [SerializeField] private Button m_DetailsBtn;
        [SerializeField] private TextMeshProUGUI m_ItemNameTmp;
        [SerializeField] private TextMeshProUGUI m_ItemTypeTmp;
        [SerializeField] private Image m_ItemIconImg;

        public void Spawn()
        {
            m_DetailsBtn.onClick.AddListener(OnClick);
        }

        public void Despawn()
        {
            m_DetailsBtn.onClick.RemoveListener(OnClick);
            _onClick = null;
        }

        public UITournamentRewardElement SetOnClickDetails(Action<string, ERewardType> onClick)
        {
            _onClick = onClick;
            return this;
        }

        public UITournamentRewardElement SetIdAndType(string id, ERewardType type)
        {
            _rewardId = id;
            _rewardType = type;
            return this;
        }

        public void SetContent(string itemName, string itemType, Sprite icon)
        {
            if (m_ItemNameTmp != null) m_ItemNameTmp.text = itemName;
            if (m_ItemTypeTmp != null) m_ItemTypeTmp.text = itemType;

            if (m_ItemIconImg != null && icon != null)
            {
                m_ItemIconImg.sprite = icon;
                m_ItemIconImg.color = Color.white; // Ensure it's fully visible
            }
            else if (m_ItemIconImg != null)
            {
                m_ItemIconImg.color = new Color(1, 1, 1, 0); // Hide if no icon
            }
        }

        private void OnClick()
        {
            _onClick?.Invoke(_rewardId, _rewardType);
        }
    }
}