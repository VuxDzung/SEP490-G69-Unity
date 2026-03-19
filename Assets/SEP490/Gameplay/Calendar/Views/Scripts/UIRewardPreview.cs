namespace SEP490G69.Calendar
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Economy;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIRewardPreview : MonoBehaviour, IPooledObject
    {
        private Action<string> _onClick;

        [SerializeField] private TextMeshProUGUI m_NumericTmp;
        [SerializeField] private Image m_Icon;
        [SerializeField] private Button m_DetailsBtn;

        private string _id;

        private ItemDataConfigSO _itemConfig;
        private ItemDataConfigSO ItemConfig
        {
            get
            {
                if (_itemConfig == null)
                {
                    _itemConfig = ContextManager.Singleton.GetDataSO<ItemDataConfigSO>();
                }
                return _itemConfig;
            }
        }

        private CardConfigSO _cardConfig;
        private CardConfigSO CardConfig
        {
            get
            {
                if (_cardConfig == null)
                {
                    _cardConfig = ContextManager.Singleton.GetDataSO<CardConfigSO>();
                }
                return _cardConfig;
            }
        }

        public void Spawn()
        {
            if (m_DetailsBtn != null) m_DetailsBtn.onClick.AddListener(Click);
        }
        public void Despawn()
        {
            if (m_DetailsBtn != null) m_DetailsBtn.onClick.RemoveListener(Click);
        }

        public UIRewardPreview SetContent(string id, ERewardType rewardType, int amount)
        {
            _id = id;
            string amountStr = $"{amount.ToString()} {DeterminePostfix(rewardType)}";
            if (m_NumericTmp != null) m_NumericTmp.text = amountStr;

            switch (rewardType)
            {
                case ERewardType.Item:
                    ItemDataSO item = ItemConfig.GetItemById(id);
                    Debug.Log($"item id: {id}");
                    if (m_Icon) m_Icon.sprite = item.ItemImage;
                    break;
                case ERewardType.Card:
                    CardSO card = CardConfig.GetCardById(id);
                    if (m_Icon) m_Icon.sprite = card.Icon;
                    break;
            }

            return this;
        }

        public UIRewardPreview SetClickDetailsCallback(Action<string> callback)
        {
            _onClick = callback;
            return this;
        }

        private string DeterminePostfix(ERewardType rewardType)
        {
            switch(rewardType)
            {
                case ERewardType.Gold:
                    return "G";
                case ERewardType.ReputationPoint:
                    return "RP";
                default:
                    return "";
            }
        }

        private void Click()
        {
            _onClick?.Invoke(_id);
        }
    }
}