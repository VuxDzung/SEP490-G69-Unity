namespace SEP490G69.Economy
{
    using LiteDB;
    using SEP490G69.Addons.Localization;
    using SEP490G69.GameSessions;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Handle business logic of shop.
    /// Logic includes:
    /// - Get all items.
    /// - Buy item.
    /// - Refresh shop items.
    /// </summary>
    public class GameShopManager : MonoBehaviour, ISceneContext
    {
        public const int MAX_ITEMS_PER_SESSION = 2;

        private GameShopDAO _shopDAO;
        private GameSessionDAO _sessionDAO;

        private GameInventoryManager _inventoryManager;
        private LocalizationManager _localization;

        private EventManager _eventManager;
        private List<ItemDataSO> _shopItemPool = new List<ItemDataSO>();
        private ItemDataConfigSO _itemConfig;

        private PlayerTrainingSession _sessionData;
        private string _sessionId;

        private List<ShopItemDataHolder> _shopItems = new List<ShopItemDataHolder>();

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            _itemConfig = ContextManager.Singleton.GetDataSO<ItemDataConfigSO>();
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
            _inventoryManager = ContextManager.Singleton.ResolveGameContext<GameInventoryManager>();
            _localization = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();

            _sessionDAO = new GameSessionDAO();
            _shopDAO = new GameShopDAO();

            LoadShopItemPool();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.Log("Session id cache does not exist!");
                return;
            }
            SetSessionId(sessionId);
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        private void LoadShopItemPool()
        {
            _shopItemPool.Clear();
            ItemDataSO[] items = _itemConfig.Items.Where(x => x.IsShopItem()).ToArray();
            Debug.Log($"Load {items.Length} shop items");
            _shopItemPool.AddRange(items);
        }
        public void SetSessionId(string sessionId)
        {
            _sessionId = sessionId;
        }
        private void LoadShopItems()
        {
            _shopItems.Clear();

            List<ShopItemData> items = _shopDAO.GetAll(_sessionId);

            LocalizationManager localization = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();

            foreach (ShopItemData data in items)
            {
                ItemDataSO so = _itemConfig.GetItemById(data.RawItemId);

                ShopItemDataHolder holder = new ShopItemDataHolder.Builder()
                                                                  .WithDBData(data)
                                                                  .WithSOData(so)
                                                                  .Build();

                _shopItems.Add(holder);
            }
        }

        /// <summary>
        /// Get all current shop items.
        /// </summary>
        /// <returns></returns>
        public ShopItemDataHolder[] GetAllAvailableShopItems()
        {
            if (_shopItems.Count == 0)
            {
                LoadShopItems();
            }

            return _shopItems.ToArray();
        }

        /// <summary>
        /// Buy an item with a specific amount.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="amount"></param>
        public void BuyItem(string itemId, int amount)
        {
            ShopItemDataHolder shopItem = _shopItems.FirstOrDefault(x => x.GetRawItemId() == itemId);

            if (shopItem == null)
            {
                Debug.LogError($"Shop item {itemId} does not exist!");
                return;
            }
            if (shopItem.GetRemainAmount() < amount)
            {
                Debug.Log($"<color=red>[GameShopManager]</color> Not enough item {shopItem.GetRawItemId()}");
                return;
            }

            PlayerTrainingSession session = GetSessionData();
            if (session == null)
            {
                Debug.LogError("[GameShopManager] Session data is null");
                return;
            }

            ItemDataSO itemSO = _itemConfig.GetItemById(itemId);

            int totalCost = itemSO.Cost * amount;

            if (session.CurrentGoldAmount < totalCost)
            {
                Debug.Log("<color=red>[GameShopManager]</color> Not enough money");
                return;
            }

            session.CurrentGoldAmount -= totalCost;

            _inventoryManager.AddItem(itemId, amount);

            shopItem.TryDecreaseAmount(amount);

            shopItem.UpdateChanges(_shopDAO);

            _sessionDAO.Update(session);

            Debug.Log($"<color=green>[GameShopManager.BuyItem]</color> Purchase item {itemId} successfully!");
        }

        /// <summary>
        /// Sell an obtained item with a specific amount.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="amount"></param>
        public void SellItem(string itemId, int amount)
        {
            ItemDataSO itemSO = _itemConfig.GetItemById(itemId);

            PlayerTrainingSession session = GetSessionData();
            if (session == null) return;

            if (!_inventoryManager.RemoveItem(itemId, amount))
                return;

            int sellPrice = itemSO.Cost / 2;

            session.CurrentGoldAmount += sellPrice * amount;

            _sessionDAO.Update(session);
        }

        /// <summary>
        /// Refresh shop items
        /// - Reset all shop items amount.
        /// - Fetch 6 random items.
        /// </summary>
        public void RefreshShop()
        {
            _shopDAO.DeleteManyBySessionId(_sessionId);

            _shopItems.Clear();

            List<ItemDataSO> randomItems = _shopItemPool
                .OrderBy(x => UnityEngine.Random.value)
                .Take(MAX_ITEMS_PER_SESSION)
                .ToList();

            foreach (ItemDataSO item in randomItems)
            {
                ShopItemData data = new ShopItemData
                {
                    SessionItemId = string.Format(GameInventoryManager.FORMAT_INVENTORY_ITEM_ID, _sessionId, item.ItemID),
                    SessionId = _sessionId,
                    RawItemId = item.ItemID,
                    RemainAmount = UnityEngine.Random.Range(1, 5)
                };

                _shopDAO.Insert(data);
                ShopItemDataHolder holder = new ShopItemDataHolder.Builder()
                                                                  .WithDBData(data)
                                                                  .WithSOData(item).Build();
                _shopItems.Add(holder);
            }
            Debug.Log($"<color=green>[GameShopManager]</color> Refresh {randomItems.Count} shop items.");
        }

        public PlayerTrainingSession GetSessionData()
        {
            if (_sessionData == null)
            {
                _sessionData = _sessionDAO.GetById(_sessionId);
            }
            return _sessionData;
        }
    }
}