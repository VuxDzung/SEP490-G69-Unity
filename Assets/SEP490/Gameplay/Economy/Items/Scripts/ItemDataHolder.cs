using UnityEngine;

namespace SEP490G69.Economy
{
    /// <summary>
    /// This is a holder for both hard-data (in-editor data) and runtime data.
    /// Use for getter only.
    /// </summary>
    [System.Serializable]
    public class ItemDataHolder 
    {
        private ItemData _data;
        private ItemDataSO _dataSO;

        #region Getters
        public string GetSessionItemId() => _data.SessionItemId;
        public string GetRawId() => _data.RawItemId;
        public string GetSessionId() => _data.SessionId;
        public Sprite GetIcon() => _dataSO == null ? null : _dataSO.ItemImage;
        public EItemType GetItemType() => _dataSO.ItemType;
        public string GetItemNameKey()
        {
            return _dataSO == null ? string.Empty : _dataSO.ItemNameKey;
        }
        public string GetItemDescription()
        {
            return _dataSO == null ? string.Empty : _dataSO.ItemDescKey;
        }
        public int GetRemainAmount()
        {
            return _data.RemainAmount;
        }

        #endregion

        public void AddItemAmount(int amount)
        {
            if (_data == null) return;
            _data.RemainAmount += amount;
        }

        public bool DecreaseItemAmount(int amount)
        {
            if (_data == null) return false;
            if (_data.RemainAmount < amount) return false;

            _data.RemainAmount -= amount;
            return true;
        }

        public void UpdateChanges(GameInventoryDAO dao)
        {
            dao.Update(_data);
        }

        public bool TryConvertAsRelic(out EquipmentData equipmentData)
        {
            equipmentData = _data as EquipmentData;
            return equipmentData != null;
        }

        public bool IsRelic()
        {
            return TryConvertAsRelic(out EquipmentData relic);
        }

        public int GetEquipmentSlot()
        {
            if (TryConvertAsRelic(out EquipmentData relic))
            {
                return relic.Slot;
            }
            return -1;
        }

        #region Builder 
        public class Builder
        {
            private ItemData data;
            private ItemDataSO dataSO;

            public Builder WithRuntimeData(ItemData data)
            {
                this.data = data;
                return this;
            }

            public Builder WithDataSO(ItemDataSO dataSO)
            {
                this.dataSO = dataSO;
                return this;
            }

            public ItemDataHolder Build()
            {
                return new ItemDataHolder
                {
                    _data = data,
                    _dataSO = dataSO
                };
            }
        }
        #endregion
    }
}