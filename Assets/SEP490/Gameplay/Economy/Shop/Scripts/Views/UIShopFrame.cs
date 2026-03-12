namespace SEP490G69.Economy
{
    using SEP490G69.GameSessions;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIShopFrame : GameUIFrame
    {
        [Header("Shop Items")]
        [SerializeField] private Transform m_ItemContainer;
        [SerializeField] private Transform m_ItemPrefab;
        [SerializeField] private TextMeshProUGUI m_RemainGoldTmp;

        [Header("Item Details")]
        [SerializeField] private Image m_ItemIcon;
        [SerializeField] private GameObject m_ItemDetailsGO;
        [SerializeField] private TextMeshProUGUI m_ItemNameTmp;
        [SerializeField] private TextMeshProUGUI m_ItemDescTmp;
        [SerializeField] private TextMeshProUGUI m_ItemCostTmp;

        [Header("Buttons")]
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private Button m_BuyBtn;
        [SerializeField] private Button m_RefreshBtn;

        private GameShopManager _shopManager;

        private List<UIShopItemElement> _slots = new();

        private string _selectedItemId;

        private GameShopManager ShopManager
        {
            get
            {
                if (_shopManager == null)
                {
                    ContextManager.Singleton.TryResolveSceneContext(out _shopManager);
                }
                return _shopManager;
            }
        }

        private GameSessionDAO _sessionDAO;
        protected GameSessionDAO SessionDAO
        {
            get
            {
                if (_sessionDAO == null) _sessionDAO = new GameSessionDAO();
                return _sessionDAO;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BuyBtn.onClick.AddListener(BuyItem);
            m_RefreshBtn.onClick.AddListener(RefreshShop);
            m_BackBtn.onClick.AddListener(Back);
            LoadShopItems();
            LoadRemainGold();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BuyBtn.onClick.RemoveListener(BuyItem);
            m_RefreshBtn.onClick.RemoveListener(RefreshShop);
            m_BackBtn.onClick.RemoveListener(Back);
        }

        /// <summary>
        /// Use PoolManager.Pools[GameConstants.POOL_UI_SHOP_ITEM].Spawn(itemPrefab:Transform, container:Transform)
        /// to spawn UI item.
        /// </summary>
        private void LoadShopItems()
        {
            ClearSlots();

            var items = ShopManager.GetAllAvailableShopItems();

            foreach (var item in items)
            {
                Transform itemSlotTrans = PoolManager.Pools[GameConstants.POOL_UI_SHOP_ITEM].Spawn(m_ItemPrefab, m_ItemContainer);
                UIShopItemElement slot = itemSlotTrans.GetComponent<UIShopItemElement>();

                if (slot == null) continue;

                slot.BindShopItem(item, LocalizeManager).SetClickAction(SelectItem);

                if (item.GetRemainAmount() <= 0)
                    slot.ShowSoldOut();
                else
                    slot.HideSoldOut();

                _slots.Add(slot);
            }
            m_ItemDetailsGO.SetActive(false);
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
        }
        private void RefreshShop()
        {
            ShopManager.RefreshShop();

            LoadShopItems();
        }
        private void BuyItem()
        {
            if (string.IsNullOrEmpty(_selectedItemId))
                return;

            ShopManager.BuyItem(_selectedItemId, 1);

            UpdateItemSlot(_selectedItemId);
        }
        private void UpdateItemSlot(string itemId)
        {
            var item = ShopManager.GetAllAvailableShopItems()
                                  .FirstOrDefault(x => x.GetRawItemId() == itemId);

            if (item == null) return;

            foreach (var slot in _slots)
            {
                if (slot.name.Contains(itemId))
                {
                    slot.BindShopItem(item, LocalizeManager);

                    if (item.GetRemainAmount() <= 0)
                        slot.ShowSoldOut();
                }
            }
        }

        private void SelectItem(string itemId)
        {
            _selectedItemId = itemId;

            var items = ShopManager.GetAllAvailableShopItems();
            var item = items.FirstOrDefault(x => x.GetRawItemId() == itemId);

            if (item == null) return;

            m_ItemDetailsGO.SetActive(true);

            m_ItemIcon.sprite = item.GetIcon();
            m_ItemNameTmp.text = item.GetItemName();
            m_ItemDescTmp.text = item.GetItemDescription();
            m_ItemCostTmp.text = item.GetPrice().ToString();

            m_BuyBtn.interactable = item.GetRemainAmount() > 0;
        }

        private void ClearSlots()
        {
            foreach (var slot in _slots)
            {
                Destroy(slot.gameObject);
            }

            _slots.Clear();
        }

        private void LoadRemainGold()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError($"[UIMainMenuFrame] Session id is null/empty");
                return;
            }
            PlayerTrainingSession sessionData = SessionDAO.GetById(sessionId);

            if (sessionData == null)
            {
                Debug.LogError($"[UIMainMenuFrame] Session data with id {sessionId} does not exist");
                return;
            }

            m_RemainGoldTmp.text = NumberFormatter.FormatGold(sessionData.CurrentGoldAmount);
        }
    }
}