namespace SEP490G69.Economy
{
    using SEP490G69.Addons.Localization;
    using UnityEngine;

    public class ShopItemDataHolder 
    {
        private ShopItemData _data;
        private ItemDataSO _dataSO;

        #region Getters
        public string GetSessionItemId()
        {
            return _data.SessionItemId;
        }
        public string GetRawItemId()
        {
            return _data.RawItemId;
        }
        public string GetItemName()
        {
            if (_dataSO == null) return string.Empty;

            return _dataSO.ItemNameKey;//_localization.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES, _dataSO.ItemID);
        }
        public string GetItemDescription()
        {
            if (_dataSO == null) return string.Empty;

            return _dataSO.ItemDescKey; //_localization.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_DESC, _dataSO.ItemID);
        }
        public float GetPrice()
        {
            if (_dataSO == null)
            {
                return 0f;
            }
            return _dataSO.Cost;
        }
        public Sprite GetIcon()
        {
            return _dataSO.ItemImage;
        }
        public int GetRemainAmount()
        {
            if (_data == null) return 0;
            return _data.RemainAmount;
        }
        #endregion

        public bool TryDecreaseAmount(int amount)
        {
            if (_data == null)
            {
                return false;
            }
            if (_data.RemainAmount < amount)
            {
                return false;
            }
            _data.RemainAmount -= amount;
            if (_data.RemainAmount < 0)
            {
                _data.RemainAmount = 0;
            }
            return true;
        }

        public void SetRemainAmount(int remainAmount)
        {
            _data.RemainAmount = remainAmount;
        }

        public void UpdateChanges(GameShopDAO dao)
        {
            dao.Update(_data);
        }

        #region Builder
        public class Builder
        {
            ShopItemData _data;
            ItemDataSO _dataSO;

            public Builder WithDBData(ShopItemData data)
            {
                _data = data;
                return this;
            }
            public Builder WithSOData(ItemDataSO data)
            {
                _dataSO = data;
                return this;
            }
            public ShopItemDataHolder Build()
            {
                ShopItemDataHolder holder = new ShopItemDataHolder
                {
                    _data = _data,
                    _dataSO = _dataSO,
                };
                return holder;
            }
        }
        #endregion
    }
}