namespace SEP490G69
{
    using SEP490G69.Addons.Localization;

    /// <summary>
    /// This is a holder for both hard-data (in-editor data) and runtime data
    /// </summary>
    [System.Serializable]
    public class ItemDataHolder 
    {
        private ItemData _data;
        private ItemDataSO _dataSO;
        private LocalizationManager _localization;

        public string GetItemName()
        {
            if (_localization == null) return string.Empty;

            if (_dataSO == null) return string.Empty;

            return _localization.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES, _dataSO.ItemID);
        }
        public string GetItemDescription()
        {
            if (_localization == null) return string.Empty;

            if (_dataSO == null) return string.Empty;

            return _localization.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_DESC, _dataSO.ItemID);
        }

        public bool TryUseItem(int amount)
        {
            if (_data == null) return false;
            if (_data.RemainAmount < amount) return false;
            _data.RemainAmount -= amount;
            return true;
        }

        public void AddItemAmount(int amount)
        {
            if (_data == null) return;

            _data.RemainAmount += amount;
        }

        public class Builder
        {
            private ItemData data;
            private ItemDataSO dataSO;
            private LocalizationManager localization;

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
            public Builder WithLocalization(LocalizationManager localization)
            {
                this.localization = localization;
                return this;
            }
            public ItemDataHolder Build()
            {
                return new ItemDataHolder
                {
                    _data = data,
                    _dataSO = dataSO,
                    _localization = localization,
                };
            }
        }
    }
}