namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using SEP490G69.Economy;
    using UnityEngine;

    public enum EEequipRelicResult
    {
        None = 0,
        FatalError = 1,
        NoSlotAvailable = 2,
        Success = 3,
    }

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

        public IReadOnlyList<ItemDataHolder> GetAllRelics()
        {
            return _inventoryItems.Where(itm => itm.GetItemType() == EItemType.Relic && itm.GetEquipmentSlot() != GameConstants.EMPTY_RELIC_SLOT).ToList();
        }

        /// <summary>
        /// Add item.
        /// If the item already in the inventory, increase the stack.
        /// If not, create the item data and add to database.
        /// </summary>
        /// <param name="rawItemId"></param>
        /// <param name="amount"></param>
        public void AddItem(string rawItemId, int amount)
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

            ItemDataHolder item = GetItemByRawId(rawItemId);

            if (item != null)
            {
                item.AddItemAmount(amount);
                item.UpdateChanges(_inventoryDAO);
            }
            else
            {
                ItemDataSO so = _itemConfig.GetItemById(rawItemId);

                ItemData newItem = null;

                if (so.ItemType == EItemType.Consumable)
                {
                    newItem = new ItemData
                    {
                        SessionItemId = EntityIdConstructor.ConstructDBEntityId(_sessionId, rawItemId),//string.Format(FORMAT_INVENTORY_ITEM_ID, _sessionId, itemId),
                        SessionId = _sessionId,
                        RawItemId = rawItemId,
                        RemainAmount = amount
                    };
                }
                else
                {
                    newItem = new EquipmentData
                    {
                        SessionItemId = EntityIdConstructor.ConstructDBEntityId(_sessionId, rawItemId),//string.Format(FORMAT_INVENTORY_ITEM_ID, _sessionId, itemId),
                        SessionId = _sessionId,
                        RawItemId = rawItemId,
                        RemainAmount = amount,
                        Slot = GameConstants.EMPTY_RELIC_SLOT
                    };
                }

                _inventoryDAO.Insert(newItem);


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
        /// <param name="sessionItemId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool UseItem(string sessionItemId, int amount)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                Debug.LogError("[GameInventoryManager.UseItem] Session id is null/empty!");
                return false;
            }

            if (string.IsNullOrEmpty(sessionItemId))
            {
                Debug.LogError("[GameInventoryManager.UseItem] Item id is null!");
                return false;
            }

            ItemDataHolder item = GetItemByEntityId(sessionItemId);

            if (item == null)
            {
                Debug.LogError($"[GameInventoryManager.UseItem] Item SO with id {sessionItemId} is not configured!");
                return false;
            }

            if (item.IsRelic())
            {
                return false;
            }

            if (!item.DecreaseItemAmount(amount))
            {
                Debug.Log($"<color=red>[GameInventoryManager.UseItem]</color> Failed to decrease item {sessionItemId} amount");
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

            ItemDataHolder item = GetItemByRawId(itemId);

            if (item == null)
            {
                Debug.LogError($"[GameInventoryManager.GetItemBy error] Item with id {itemId} is null");
                return false;
            }

            if (!item.DecreaseItemAmount(amount))
            {
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

            return true;
        }

        /// <summary>
        /// Equip relic at any available slot. If not slot is available, return error.
        /// </summary>
        /// <param name="relicId"></param>
        public EEequipRelicResult EquipRelic(string sessionRelicId)
        {
            if (string.IsNullOrEmpty(sessionRelicId))
            {
                return EEequipRelicResult.FatalError;
            }

            // Get available slot.
            int slot = GetRemainEmptySlot();

            if (slot == GameConstants.EMPTY_RELIC_SLOT)
            {
                return EEequipRelicResult.NoSlotAvailable;
            }

            
            return EquipRelic(sessionRelicId, slot) == true ? EEequipRelicResult.Success : 
                                                              EEequipRelicResult.FatalError;
        }

        /// <summary>
        /// Relic only. equip the relic to the specific slot.
        /// If that slot has already had a relic, remove the existed relic from the slot 
        /// and then equip the selected relic.
        /// </summary>
        /// <param name="relicId"></param>
        /// <param name="slot"></param>
        public bool EquipRelic(string sessionRelicId, int slot)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                Debug.LogError("[GameInventoryManager.EquipRelic error] Session id is null");
                return false;
            }

            ItemDataHolder holder = GetItemByEntityId(sessionRelicId);

            if (holder == null)
            {
                Debug.Log($"<color=red>[GameInventoryManager.EquipRelic error]</color> Enity with id {sessionRelicId} does not exist!");
                return false;
            }

            if (!holder.TryConvertAsRelic(out EquipmentData relic))
            {
                Debug.Log($"<color=red>[GameInventoryManager.EquipRelic error]</color> Failed to convert {sessionRelicId} as relic");
                return false;
            }

            EquipmentData currentRelic = _inventoryDAO.GetEquippedRelic(_sessionId, slot);

            if (currentRelic != null)
            {
                Debug.Log($"<color=yellow>[GameInventoryManager.EquipRelic]</color> A relic has already in slot {slot}. Remove it by default/nExisted relic: {currentRelic.SessionItemId}");
                UnequipRelic(currentRelic.SessionItemId, slot);
            }

            holder.EquipRelic(slot);
            

            Debug.Log($"[GameInventoryManager.EquipRelic] Equip relic {sessionRelicId} at slot {relic.Slot}");

            holder.UpdateChanges(_inventoryDAO);

            _eventManager.Publish(new EquipRelicEvent
            {
                ItemData = holder
            });

            return true;
        }

        /// <summary>
        /// Unequip the relic.
        /// </summary>
        /// <param name="relic"></param>
        /// <param name="slot"></param>
        public bool UnequipRelic(string sessionRelicId, int slot)
        {
            if (string.IsNullOrEmpty(sessionRelicId))
            {
                return false;
            }

            ItemDataHolder item = GetItemByEntityId(sessionRelicId);

            if (item == null) return false;

            item.UnequipRelic();
            item.UpdateChanges(_inventoryDAO);

            _eventManager.Publish(new UnequipRelicEvent());
            return true;
        }

        public ItemDataHolder GetItemById(string sessionId, string rawId)
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

        public ItemDataHolder GetItemByRawId(string rawId)
        {
            if (string.IsNullOrEmpty(_sessionId) || string.IsNullOrEmpty(rawId))
            {
                Debug.LogError("[GameInventoryManager.GetItemBy error] Raw item id is null");
                return null;
            }

            return _inventoryItems.FirstOrDefault(x => x.GetRawId() == rawId);
        }

        public ItemDataHolder GetItemByEntityId(string entityId)
        {
            if (string.IsNullOrEmpty(_sessionId) || string.IsNullOrEmpty(entityId))
            {
                Debug.LogError("[GameInventoryManager.GetItemBy error] Session id/entity id is null");
                return null;
            }

            return _inventoryItems.FirstOrDefault(x => x.GetSessionItemId() == entityId);
        }

        private int GetRemainEmptySlot()
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                return GameConstants.EMPTY_RELIC_SLOT;
            }

            return _inventoryDAO.GetRemainEquipSlot(_sessionId);
        }
    }
}