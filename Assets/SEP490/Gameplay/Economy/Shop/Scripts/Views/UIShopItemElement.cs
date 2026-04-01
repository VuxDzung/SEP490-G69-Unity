namespace SEP490G69.Economy
{
    using SEP490G69.Addons.Localization;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIShopItemElement : UIInventoryItemSlot
    {
        [SerializeField] private GameObject m_SoldOutTextGO;
        [SerializeField] private TextMeshProUGUI m_NameTmp;
        [SerializeField] private TextMeshProUGUI m_CostTmp;
        [SerializeField] private Image m_RarityImg;

        public UIShopItemElement BindShopItem(ShopItemDataHolder item, LocalizationManager localizeManager)
        {
            _rawItemId = item.GetRawItemId();

            if (m_Icon != null) m_Icon.sprite = item.GetIcon();
            if (m_NameTmp != null) m_NameTmp.text = localizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES , item.GetItemName());
            if (m_AmountTmp != null) m_AmountTmp.text = $"x{item.GetRemainAmount().ToString()}";
            if (m_CostTmp != null) m_CostTmp.text = $"{item.GetPrice().ToString()}G";
            if (m_RarityImg != null)
            {
                m_RarityImg.sprite = ContextManager.Singleton.GetDataSO<ImageMasterConfigSO>().GetImage("item_rarity_icon", GameConstants.ConvertRarityToImgId(item.GetRarity()))?.image;
            }
            return this;
        }

        public void ShowSoldOut()
        {
            m_SoldOutTextGO.SetActive(true);
            //m_CostTmp.enabled = false;
        }
        public void HideSoldOut()
        {
            m_SoldOutTextGO.SetActive(false);
            //m_CostTmp.enabled = true;
        }
    }
}