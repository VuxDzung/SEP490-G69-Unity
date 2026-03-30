namespace SEP490G69.Calendar
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using SEP490G69.Battle.Cards;

    public class UITournamentRewardElement : MonoBehaviour, IPooledObject
    {
        private Action<string, ERewardType> _onClick;

        // Đổi tên biến để trỏ đúng vào TargetId (CardId/ItemId)
        private string _rewardTargetId;
        private ERewardType _rewardType;

        [Header("UI Text")]
        [SerializeField] private Button m_DetailsBtn;
        [SerializeField] private TextMeshProUGUI m_ItemNameTmp;
        [SerializeField] private TextMeshProUGUI m_ItemTypeTmp;

        [Header("Card Slot Setup")]
        [SerializeField] private GameObject m_CardSlot;
        [SerializeField] private Image m_CardIconImg;

        [Header("Item Slot Setup")]
        [SerializeField] private GameObject m_ItemSlot;
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

        // Cập nhật tham số nhận vào là targetId
        public UITournamentRewardElement SetIdAndType(string targetId, ERewardType type)
        {
            _rewardTargetId = targetId;
            _rewardType = type;
            return this;
        }

        public void SetContent(string itemName, string itemType, Sprite icon)
        {
            if (m_ItemNameTmp != null) m_ItemNameTmp.text = itemName;
            if (m_ItemTypeTmp != null) m_ItemTypeTmp.text = itemType;

            if (m_CardSlot != null) m_CardSlot.SetActive(false);
            if (m_ItemSlot != null) m_ItemSlot.SetActive(false);

            if (icon != null)
            {
                if (_rewardType == ERewardType.Card)
                {
                    if (m_CardSlot != null) m_CardSlot.SetActive(true);
                    if (m_CardIconImg != null)
                    {
                        m_CardIconImg.sprite = icon;
                        m_CardIconImg.color = Color.white;
                    }
                }
                else
                {
                    if (m_ItemSlot != null) m_ItemSlot.SetActive(true);
                    if (m_ItemIconImg != null)
                    {
                        m_ItemIconImg.sprite = icon;
                        m_ItemIconImg.color = Color.white;
                    }
                }
            }
        }

        private void OnClick()
        {
            _onClick?.Invoke(_rewardTargetId, _rewardType);

            if (_rewardType == ERewardType.Card)
            {
                CardConfigSO cardConfig = ContextManager.Singleton.GetDataSO<CardConfigSO>();

                if (cardConfig != null)
                {
                    CardSO cardData = cardConfig.GetCardById(_rewardTargetId);

                    if (cardData != null)
                    {
                        var frame = GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_CARD_DETAILS).AsFrame<UICardDetailFrame>();
                        frame.LoadData(cardData);
                    }
                    else
                    {
                        Debug.LogWarning($"[UITournamentRewardElement] Không tìm thấy Card với TargetID: {_rewardTargetId}");
                    }
                }
            }
        }
    }
}