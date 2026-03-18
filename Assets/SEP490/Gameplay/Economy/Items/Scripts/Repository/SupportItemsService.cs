using SEP490G69.Economy;
using System.Collections.Generic;
using System.Linq;

namespace SEP490G69
{
    public class SupportItemsService 
    {
        private readonly TrainingSupportItemsDAO _supportItemsDAO;
        private readonly ItemDataConfigSO _itemConfig;

        public ItemDataConfigSO ItemConfig => _itemConfig;

        public SupportItemsService()
        {
            _supportItemsDAO = new TrainingSupportItemsDAO();
            _itemConfig = ContextManager.Singleton.GetDataSO<ItemDataConfigSO>();
        }

        /// <summary>
        /// Add supported item which increase the training effectiveness.
        /// RULE:
        /// - Only 1 stacking item is available.
        /// - If the player add the second item, it would compare the value between the two and keep the larger one.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="rawItemId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool StackSupportItem(string sessionId, string rawItemId, int amount = 1)
        {

            List<TrainingSupportItem> supportItems = GetAllBySessionId(sessionId);

            if (supportItems.Count == 0)
            {
                // Insert the item to db.
                TrainingSupportItem newSupportItem = new TrainingSupportItem();
                newSupportItem.EntityId = EntityIdConstructor.ConstructDBEntityId(sessionId, rawItemId);
                newSupportItem.SessionId = sessionId;
                newSupportItem.RawItemId = rawItemId;
                newSupportItem.RemainAmount = amount;
                return _supportItemsDAO.Insert(newSupportItem);
            }
            else
            {
                TrainingSupportItem supportItem = supportItems[0];

                // New item modifier
                ItemDataSO newItemSO = _itemConfig.GetItemById(rawItemId);
                StatusModifierSO newItemMods = newItemSO.GetModifiersByStatType(EStatusType.TrainingEffective).FirstOrDefault();

                // In-stack item modifier
                ItemDataSO currentItemSO = _itemConfig.GetItemById(supportItem.RawItemId);
                StatusModifierSO currentItemMods = currentItemSO.GetModifiersByStatType(EStatusType.TrainingEffective).FirstOrDefault();

                float newModValue = newItemMods.Value;
                float currentModValue = currentItemMods.Value;

                if (newModValue > currentModValue)
                {
                    TrainingSupportItem newSupportItem = new TrainingSupportItem();
                    newSupportItem.EntityId = EntityIdConstructor.ConstructDBEntityId(sessionId, rawItemId);
                    newSupportItem.SessionId = sessionId;
                    newSupportItem.RawItemId = rawItemId;
                    newSupportItem.RemainAmount = amount;
                    return _supportItemsDAO.Insert(newSupportItem);
                }

                // Nothing change.
                return true;
            }
        }

        public TrainingSupportItem GetItem(string entityId)
        {
            return _supportItemsDAO.GetById(entityId);
        }
        public TrainingSupportItem GetById(string sessionId, string rawItemId)
        {
            return _supportItemsDAO.GetById(sessionId, rawItemId);
        }
        public List<TrainingSupportItem> GetAllBySessionId(string sessionId)
        {
            return _supportItemsDAO.GetAllBySessionId(sessionId);
        }
        public void ClearAllItems(string sessionId)
        {
            _supportItemsDAO.DeleteAllBySessionId(sessionId);
        }
    }
}