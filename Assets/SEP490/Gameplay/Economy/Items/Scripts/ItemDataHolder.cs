using System.Collections.Generic;
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

        private EquipmentData _relicData;

        #region Getters
        public string GetSessionItemId()
        {
            return _data != null ? _data.SessionItemId : string.Empty;
        }
        public string GetRawId()
        {
            return _dataSO != null ? _dataSO.ItemID : string.Empty;
        }

        public string GetSessionId()
        {
            return _data != null ? _data.SessionId : string.Empty;
        }

        public Sprite GetIcon() => _dataSO == null ? null : _dataSO.ItemImage;
        public EItemType GetItemType() => _dataSO != null ? _dataSO.ItemType : EItemType.None;
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
            return _data != null ? _data.RemainAmount : 0;
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
            if (_relicData == null)
            {
                equipmentData = _data as EquipmentData;
                return equipmentData != null;
            }
            else
            {
                equipmentData = _relicData;
                return true;
            }
        }

        public bool IsRelic()
        {
            return TryConvertAsRelic(out EquipmentData relic);
        }

        public void EquipRelic(int slot)
        {
            if (TryConvertAsRelic(out EquipmentData relic))
            {
                relic.Slot = slot;
                relic.RemainAmount--;
                if (relic.RemainAmount < 0)
                {
                    relic.RemainAmount = 0;
                }
            }
        }

        public void UnequipRelic()
        {
            if (TryConvertAsRelic(out EquipmentData relic))
            {
                relic.Slot = GameConstants.EMPTY_RELIC_SLOT;
                relic.RemainAmount++;
            }
        }

        public bool IsRelicEquipped()
        {
            if (!IsRelic()) return false;

            if (TryConvertAsRelic(out EquipmentData relic))
            {
                return relic.Slot != GameConstants.EMPTY_RELIC_SLOT;
            }
            return false;
        }

        public int GetEquipmentSlot()
        {
            if (TryConvertAsRelic(out EquipmentData relic))
            {
                return relic.Slot;
            }
            return -1;
        }

        public IReadOnlyList<StatusModifierSO> GetUsableModifiers()
        {
            return _dataSO.UsableModifiers;
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