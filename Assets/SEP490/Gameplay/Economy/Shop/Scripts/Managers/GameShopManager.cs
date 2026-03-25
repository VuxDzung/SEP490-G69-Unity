namespace SEP490G69.Economy
{
    using LiteDB;
    using SEP490G69.Addons.Localization;
    using SEP490G69.Calendar;
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
        public const int MAX_ITEMS_PER_SESSION = 6;

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

            if (_eventManager) _eventManager.Subscribe<NextWeekEvent>(HandleNewWeekEvent);

            LoadStarterShop();
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
            if (_eventManager) _eventManager.Unsubscribe<NextWeekEvent>(HandleNewWeekEvent);
        }

        private void LoadShopItemPool()
        {
            _shopItemPool.Clear();
            ItemDataSO[] items = _itemConfig.Items.Where(x => x.IsShopItem()).ToArray();
            Debug.Log($"Load {items.Length} shop items");
            _shopItemPool.AddRange(items);
        }
        private void LoadStarterShop()
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                return;
            }

            if (_shopDAO.GetAll(_sessionId).Count == 0)
            {
                RefreshShop(true);
            }
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

        private void HandleNewWeekEvent(NextWeekEvent ev)
        {
            if (ev.IsNewMonth)
            {
                RefreshShop(true);
                PlayerTrainingSession sessionData = GetSessionData();
                if (sessionData == null)
                {
                    return;
                }
                sessionData.RefreshShopCount = 0;
                _sessionDAO.Update(sessionData);
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
        /// <param name="rawItemId"></param>
        /// <param name="amount"></param>
        public void BuyItem(string rawItemId, int amount)
        {
            ShopItemDataHolder shopItem = _shopItems.FirstOrDefault(x => x.GetRawItemId() == rawItemId);

            if (shopItem == null)
            {
                Debug.LogError($"Shop item {rawItemId} does not exist!");
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

            ItemDataSO itemSO = _itemConfig.GetItemById(rawItemId);

            int totalCost = itemSO.Cost * amount;

            if (session.CurrentGoldAmount < totalCost)
            {
                Debug.Log("<color=red>[GameShopManager]</color> Not enough money");
                return;
            }

            session.CurrentGoldAmount -= totalCost;

            _inventoryManager.AddItem(rawItemId, amount);

            shopItem.TryDecreaseAmount(amount);

            shopItem.UpdateChanges(_shopDAO);

            _sessionDAO.Update(session);

            LocalDBOrchestrator.UpdateDBChangeTime();

            Debug.Log($"<color=green>[GameShopManager.BuyItem]</color> Purchase item {rawItemId} successfully!");
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

            LocalDBOrchestrator.UpdateDBChangeTime();
        }

        /// <summary>
        /// Refresh shop items
        /// - Reset all shop items amount.
        /// - Fetch 6 random items.
        /// </summary>
        public void RefreshShop(bool auto = false)
        {
            if (!auto)
            {
                PlayerTrainingSession session = GetSessionData();

                if (session == null) return;

                float refreshCost = CalculateRefreshCost(session);

                if (refreshCost > session.CurrentGoldAmount)
                {
                    // Cannot refresh notification.
                    return;
                }
            }

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
                    SessionItemId = EntityIdConstructor.ConstructDBEntityId(_sessionId, item.ItemID),
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

            LocalDBOrchestrator.UpdateDBChangeTime();
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

        public float CalculateRefreshCost()
        {
            PlayerTrainingSession session = GetSessionData();

            return CalculateRefreshCost(session);
        }

        private float CalculateRefreshCost(PlayerTrainingSession session)
        {
            if (session == null) return -1f;

            int refreshCount = session.RefreshShopCount;

            float refreshCost = GameConstants.CalculateRefreshCost(refreshCount);

            refreshCost = (float)System.Math.Round(refreshCost, 0);

            return refreshCost;
        }
    }
}