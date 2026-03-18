namespace SEP490G69.Legacy
{
    using UnityEngine;

    [System.Serializable]
    public class LegacyStatDataHolder 
    {
        private LegacyStatData _data;
        private LegacyStatSO _dataSO;

        public string GetEntityId()
        {
            return _data.Id;
        }
        public string GetRawId()
        {
            return _dataSO.LegacyStatId;
        }

        public int GetCurrentLevel()
        {
            return _data.Level;
        }

        public Sprite GetIcon()
        {
            return _dataSO.Icon;
        }

        public string GetName()
        {
            return _dataSO.LegacyName;
        }
        public string GetDesc()
        {
            return _dataSO?.LegacyDesc;
        }

        public int GetValue(int level)
        {
            return _dataSO.BaseValue + _dataSO.BonusPerLevel * level;
        }

        public int GetCurrentValue()
        {
            return GetValue(GetCurrentLevel());
        }

        public int GetUpgradeCost()
        {
            if (_data.Level >= GameConstants.LEGACY_STATS_MAX_LV)
            {
                return 0;
            }
            return _dataSO.BaseCost + _dataSO.CostPerLevel * _data.Level;
        }

        public class Builder
        {
            private LegacyStatData data;
            private LegacyStatSO dataSO;

            public Builder WithData(LegacyStatData data)
            {
                this.data = data;
                return this;
            }
            public Builder WithSO(LegacyStatSO dataSO)
            {
                this.dataSO = dataSO;
                return this;
            }
            public LegacyStatDataHolder Build()
            {
                return new LegacyStatDataHolder
                {
                    _data = data,
                    _dataSO = dataSO,
                };
            }
        }
    }
}