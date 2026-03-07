namespace SEP490G69.Economy
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIInventoryItemDetailsFrame : GameUIFrame
    {
        [Header("Item details")]
        [SerializeField] private Image m_ItemIcon;
        [SerializeField] private TextMeshProUGUI m_ItemNameTmp;
        [SerializeField] private TextMeshProUGUI m_ItemDescTmp;
        [SerializeField] private Button m_UseBtn;
        [SerializeField] private Button m_EquipBtn;
        [SerializeField] private Button m_UnequipBtn;

        private string _selectedItemId;
        private InventoryManager _invetoryManager;
        private InventoryManager InventoryManager
        {
            get
            {
                if (_invetoryManager == null)
                {
                    _invetoryManager = ContextManager.Singleton.ResolveGameContext<InventoryManager>();
                }
                return _invetoryManager;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_EquipBtn.onClick.AddListener(EquipRelic);
            m_UseBtn.onClick.AddListener(UseItem);
            m_UnequipBtn.onClick.AddListener(UnequipRelic);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_EquipBtn.onClick.RemoveListener(EquipRelic);
            m_UseBtn.onClick.RemoveListener(UseItem);
            m_UnequipBtn.onClick.RemoveListener(UnequipRelic);
            ClearItemDetails();
        }

        public void SelectItem(string itemId)
        {
            _selectedItemId = itemId;

            ItemDataHolder item = InventoryManager.GetItemBy(itemId);

            if (item == null)
                return;

            m_ItemIcon.sprite = item.GetIcon();
            m_ItemNameTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES, item.GetItemNameKey());
            m_ItemDescTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_DESC, item.GetItemDescription());

            bool isConsumable = item.GetItemType() == EItemType.Consumable;
            bool isRelic = item.GetItemType() == EItemType.Relic;

            if (isRelic)
            {
                m_EquipBtn.gameObject.SetActive(item.GetEquipmentSlot() == InventoryManager.EMPTY_RELIC_SLOT);
                m_UnequipBtn.gameObject.SetActive(item.GetEquipmentSlot() != InventoryManager.EMPTY_RELIC_SLOT);
                return;
            }
            m_EquipBtn.gameObject.SetActive(false);
            m_UnequipBtn.gameObject.SetActive(false);
            m_UseBtn.gameObject.SetActive(isConsumable);
        }

        private void ClearItemDetails()
        {
            _selectedItemId = null;

            m_ItemIcon.sprite = null;
            m_ItemNameTmp.text = "";
            m_ItemDescTmp.text = "";

            m_UseBtn.gameObject.SetActive(false);
            m_EquipBtn.gameObject.SetActive(false);
        }

        private void UseItem()
        {
            if (string.IsNullOrEmpty(_selectedItemId))
                return;

            InventoryManager.UseItem(_selectedItemId, 1);
        }

        private void EquipRelic()
        {
            if (string.IsNullOrEmpty(_selectedItemId))
                return;

            InventoryManager.EquipRelic(_selectedItemId, 0);
        }

        private void UnequipRelic()
        {
            if (string.IsNullOrEmpty(_selectedItemId))
                return;
            InventoryManager.UnequipRelic(_selectedItemId, 0);
        }
    }
}