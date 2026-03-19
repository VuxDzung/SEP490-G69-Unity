namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using SEP490G69.Economy;
    using UnityEngine;

    /// <summary>
    /// Handle business logic of inventory.
    /// Logic includes:
    /// - Add item
    /// - Use item
    /// - Sell item
    /// - Equip relic
    /// - Unequip relic.
    /// </summary>
    public class GameInventoryManager : MonoBehaviour, IGameContext
    {
        public const string FORMAT_INVENTORY_ITEM_ID = "{0}:{1}";
        public const int EMPTY_RELIC_SLOT = -1;

        private EventManager _eventManager;
        private ContextManager _contextManager;

        private GameInventoryDAO _inventoryDAO;
        private ItemDataConfigSO _itemConfig;

        private string _sessionId;

        private List<ItemDataHolder> _inventoryItems = new();


        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;
        }

        private void Awake()
        {
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
            _inventoryDAO = new GameInventoryDAO();
            _itemConfig = ContextManager.Singleton.GetDataSO<ItemDataConfigSO>();
        }

        public void SetSessionId(string sessionId)
        {
            _sessionId = sessionId;
            LoadInventory();
        }

        private void LoadInventory()
        {
            _inventoryItems.Clear();

            List<ItemData> items = _inventoryDAO.GetAllItems(_sessionId);

            foreach (ItemData data in items)
            {
                ItemDataSO so = _itemConfig.GetItemById(data.RawItemId);

                var holder = new ItemDataHolder.Builder()
                                               .WithRuntimeData(data)
                                               .WithDataSO(so)
                                               .Build();

                _inventoryItems.Add(holder);
            }
        }

        public ItemDataHolder[] GetAllItems()
        {
            return _inventoryItems.ToArray();
        }

        /// <summary>
        /// Add item.
        /// If the item already in the inventory, increase the stack.
        /// If not, create the item data and add to database.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="amount"></param>
        public void AddItem(string itemId, int amount)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                _sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

                if (string.IsNullOrEmpty(_sessionId))
                {
                    Debug.LogError("[GameInventoryManager.AddItem] Session id is null/empty!");
                    return;
                }
            }

            ItemDataHolder item = GetItemBy(itemId);

            if (item != null)
            {
                item.AddItemAmount(amount);
                item.UpdateChanges(_inventoryDAO);
            }
            else
            {
                ItemData newItem = new ItemData
                {
                    SessionItemId = string.Format(FORMAT_INVENTORY_ITEM_ID, _sessionId, itemId),
                    SessionId = _sessionId,
                    RawItemId = itemId,
                    RemainAmount = amount
                };

                _inventoryDAO.Insert(newItem);

                ItemDataSO so = _itemConfig.GetItemById(itemId);

                item = new ItemDataHolder.Builder()
                    .WithRuntimeData(newItem)
                    .WithDataSO(so)
                    .Build();

                _inventoryItems.Add(item);
            }
            _eventManager.Publish(new AddItemEvent
            {
                ItemData = item
            });
        }

        /// <summary>
        /// Consumable item only.
        /// Use the item for the players's character.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool UseItem(string itemId, int amount)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                Debug.LogError("[GameInventoryManager.UseItem] Session id is null/empty!");
                return false;
            }

            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogError("[GameInventoryManager.UseItem] Item id is null!");
                return false;
            }

            ItemDataHolder item = GetItemBy(itemId);

            if (item == null)
            {
                Debug.LogError($"[GameInventoryManager.UseItem] Item SO with id {itemId} is not configured!");
                return false;
            }

            if (!item.DecreaseItemAmount(amount))
            {
                Debug.Log($"<color=red>[GameInventoryManager.UseItem]</color> Failed to decrease item {itemId} amount");
                return false;
            }

            if (item.GetRemainAmount() == 0)
            {
                _inventoryDAO.Delete(item.GetSessionItemId());
                _inventoryItems.Remove(item);
            }
            else
            {
                item.UpdateChanges(_inventoryDAO);
            }
            _eventManager.Publish(new UseItemEvent
            {
                ItemData = item
            });
            return true;
        }

        /// <summary>
        /// Remove item from database.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool RemoveItem(string itemId, int amount)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                Debug.LogError("[GameInventoryManager.GetItemBy error] Session id is null");
                return false;
            }

            ItemDataHolder item = GetItemBy(itemId);

            if (item == null)
            {
                Debug.LogError($"[GameInventoryManager.GetItemBy error] Item with id {itemId} is null");
                return false;
            }

            if (!item.DecreaseItemAmount(amount))
                return false;

            if (item.GetRemainAmount() == 0)
            {
                _inventoryDAO.Delete(item.GetSessionItemId());
                _inventoryItems.Remove(item);
            }
            else
            {
                item.UpdateChanges(_inventoryDAO);
            }

            return true;
        }

        /// <summary>
        /// Relic only. equip the relic to the specific slot.
        /// If that slot has already had a relic, remove the existed relic from the slot 
        /// and then equip the selected relic.
        /// </summary>
        /// <param name="relicId"></param>
        /// <param name="slot"></param>
        public void EquipRelic(string relicId, int slot)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                Debug.LogError("[GameInventoryManager.EquipRelic error] Session id is null");
                return;
            }

            ItemDataHolder holder = GetItemBy(relicId);

            if (holder == null)
                return;

            if (!holder.TryConvertAsRelic(out EquipmentData relic))
                return;

            EquipmentData currentRelic = _inventoryDAO.GetEquippedRelic(_sessionId, slot);

            if (currentRelic != null)
            {
                currentRelic.Slot = EMPTY_RELIC_SLOT;
                _inventoryDAO.Update(currentRelic);
            }

            relic.Slot = slot;

            _inventoryDAO.Update(relic);

            _eventManager.Publish(new EquipRelicEvent
            {
                ItemData = holder
            });
        }

        /// <summary>
        /// Unequip the relic.
        /// </summary>
        /// <param name="relic"></param>
        /// <param name="slot"></param>
        public void UnequipRelic(string relicId, int slot)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                Debug.LogError("[GameInventoryManager.UnequipRelic error] Session id is null");
                return;
            }

            EquipmentData relic = _inventoryDAO.GetRelic(_sessionId, relicId);

            if (relic == null) return;

            if (relic.Slot != slot) return;

            relic.Slot = EMPTY_RELIC_SLOT;

            _inventoryDAO.Update(relic);

            _eventManager.Publish(new UnequipRelicEvent());
        }

        public ItemDataHolder GetItemBy(string sessionId, string rawId)
        {
            if (string.IsNullOrEmpty(_sessionId) || string.IsNullOrEmpty(rawId))
            {
                Debug.LogError("[GameInventoryManager.GetItemBy error] Session id/raw item id is null");
                return null;
            }
            ItemDataHolder item = _inventoryItems.FirstOrDefault(item => item.GetSessionId().Equals(_sessionId) &&
                                                  item.GetRawId().Equals(rawId));
            return item;
        }

        public ItemDataHolder GetItemBy(string rawId)
        {
            if (string.IsNullOrEmpty(_sessionId) || string.IsNullOrEmpty(rawId))
            {
                Debug.LogError("[GameInventoryManager.GetItemBy error] Raw item id is null");
                return null;
            }

            return _inventoryItems.FirstOrDefault(x => x.GetRawId() == rawId);
        }
    }
}