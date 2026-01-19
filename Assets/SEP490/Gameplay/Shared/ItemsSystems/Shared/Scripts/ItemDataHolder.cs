namespace SEP490G69
{
    using SEP490G69.Addons.Localization;
    using UnityEngine;

    /// <summary>
    /// This is a holder for both hard-data (in-editor data) and runtime data
    /// </summary>
    [System.Serializable]
    public class ItemDataHolder 
    {
        private ItemData _data;
        private ItemDataSO _dataSO;
        private LocalizationManager _localization;

        public ItemDataHolder(ItemData data, ItemDataSO dataSO, LocalizationManager localization)
        {
            _data = data;
            _dataSO = dataSO;
            _localization = localization;
        }

        public string GetItemName()
        {
            if (_localization == null) return string.Empty;

            return _localization.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES, _data.ItemID);
        }
        public string GetItemDescription()
        {
            if (_localization == null) return string.Empty;

            return _localization.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_DESC, _data.ItemID);
        }

        public bool TryUseItem(int amount)
        {
            if (_data.RemainAmount < amount) return false;
            _data.RemainAmount -= amount;
            return true;
        }

        public void AddItemAmount(int amount)
        {
            _data.RemainAmount += amount;
        }
    }
}