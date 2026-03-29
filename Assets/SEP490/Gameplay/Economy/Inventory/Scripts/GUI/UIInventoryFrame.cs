namespace SEP490G69.Economy
{
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIInventoryFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;

        [Header("Character info")]
        [SerializeField] private Image m_CharacterImg;
        [SerializeField] private TextMeshProUGUI m_CharacterNameTmp;
        [SerializeField] private UIRelicSlot[] m_RelicSlotArray;

        [Header("Item list")]
        [SerializeField] private Transform m_ItemContainer;
        [SerializeField] private Transform m_ItemSlotPrefab;

        [Header("Filter buttons")]
        [SerializeField] private Button m_AllItemsBtn;
        [SerializeField] private Button m_UsableItemsBtn;
        [SerializeField] private Button m_RelicItemsBtn;

        [Header("Item details")]
        [SerializeField] private GameObject m_UIItemDetailsGO;
        [SerializeField] private Image m_ItemIcon;
        [SerializeField] private TextMeshProUGUI m_ItemNameTmp;
        [SerializeField] private TextMeshProUGUI m_ItemDescTmp;
        [SerializeField] private TextMeshProUGUI m_ItemTypeTmp;
        [SerializeField] private Button m_UseBtn;
        [SerializeField] private Button m_EquipBtn;
        [SerializeField] private Button m_UnequipBtn;

        private string _selectedItemId = string.Empty;
        private int _selectedSlot = GameConstants.EMPTY_RELIC_SLOT;

        private EItemType _currentFilter = EItemType.None;
        private readonly List<UIInventoryItemSlot> _slots = new();

        private GameInventoryManager _invetoryManager;
        private GameInventoryManager InventoryManager
        {
            get
            {
                if (_invetoryManager == null)
                {
                    _invetoryManager = ContextManager.Singleton.ResolveGameContext<GameInventoryManager>();
                }
                return _invetoryManager;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_AllItemsBtn.onClick.AddListener(FilterAllItems);
            m_UsableItemsBtn.onClick.AddListener(FilterUsableItems);
            m_RelicItemsBtn.onClick.AddListener(FilterRelicItems);

            EventManager.Subscribe<AddItemEvent>(OnInventoryUpdated);
            EventManager.Subscribe<UseItemEvent>(OnInventoryUpdated);
            EventManager.Subscribe<EquipRelicEvent>(OnInventoryUpdated);
            EventManager.Subscribe<UnequipRelicEvent>(OnInventoryUpdated);

            m_EquipBtn.onClick.AddListener(EquipRelic);
            m_UseBtn.onClick.AddListener(UseItem);
            m_UnequipBtn.onClick.AddListener(UnequipRelic);

            DisplayItems(EItemType.None);
            SetupRelicSlots();

            m_BackBtn.onClick.AddListener(Back);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_AllItemsBtn.onClick.RemoveListener(FilterAllItems);
            m_UsableItemsBtn.onClick.RemoveListener(FilterUsableItems);
            m_RelicItemsBtn.onClick.RemoveListener(FilterRelicItems);

            EventManager.Unsubscribe<AddItemEvent>(OnInventoryUpdated);
            EventManager.Unsubscribe<UseItemEvent>(OnInventoryUpdated);
            EventManager.Unsubscribe<EquipRelicEvent>(OnInventoryUpdated);
            EventManager.Unsubscribe<UnequipRelicEvent>(OnInventoryUpdated);

            m_BackBtn.onClick.RemoveListener(Back);
            m_EquipBtn.onClick.RemoveListener(EquipRelic);
            m_UseBtn.onClick.RemoveListener(UseItem);
            m_UnequipBtn.onClick.RemoveListener(UnequipRelic);
            ClearAllUIElements();
            CloseDetails();
        }

        private void OnInventoryUpdated<T>(T evt)
        {
            DisplayItems(_currentFilter);
        }

        private void FilterAllItems()
        {
            DisplayItems(EItemType.None);
        }

        private void FilterUsableItems()
        {
            DisplayItems(EItemType.Consumable);
        }

        private void FilterRelicItems()
        {
            DisplayItems(EItemType.Relic);
        }

        private void DisplayItems(EItemType itemType)
        {
            _currentFilter = itemType;

            ClearAllUIElements();

            ItemDataHolder[] items = InventoryManager.GetAllItems();

            foreach (var slot in m_RelicSlotArray)
            {
                slot.SetEmpty();
            }

            foreach (ItemDataHolder item in items)
            {
                if (itemType != EItemType.None && item.GetItemType() != itemType)
                    continue;
                if (item.GetRemainAmount() == 0) continue;

                Transform slotTrans = PoolManager.Pools[GameConstants.POOL_UI_INVENTORY_ITEM].Spawn(m_ItemSlotPrefab, m_ItemContainer);

                UIInventoryItemSlot slot = slotTrans.GetComponent<UIInventoryItemSlot>();

                if (slot == null) continue;

                slot.BindInventoryItem(item).SetClickAction(SelectItem);

                _slots.Add(slot);
            }
            LoadRelicSlots();
            CloseDetails();
        }

        private void LoadRelicSlots()
        {
            IReadOnlyList<ItemDataHolder> items = InventoryManager.GetAllRelics();

            foreach (ItemDataHolder item in items)
            {
                if (item.GetItemType() != EItemType.Relic)
                    continue;

                if (item.IsRelicEquipped() == true)
                {
                    LoadRelicToEquipSlot(item);
                }
            }
        }

        private void SetupRelicSlots()
        {
            foreach (var slot in m_RelicSlotArray)
            {
                slot.SetOnClickCallback(OnClickRelicSlot);
            }
        }

        private void LoadRelicToEquipSlot(ItemDataHolder relic)
        {
            if (relic.TryConvertAsRelic(out EquipmentData relicData))
            {
                UIRelicSlot slotUI = GetRelicAtSlot(relicData.Slot);

                if (slotUI != null)
                {
                    slotUI.SetRelicInfo(relic.GetSessionItemId(), relic.GetIcon());
                }
            }
        }

        private void SelectItem(string rawItemId, string sessionItemId)
        {
            _selectedItemId = sessionItemId;

            //UIManager.ShowFrame(GameConstants.FRAME_ID_INVENTORY_ITEM_DETAILS)
            //         .AsFrame<UIInventoryItemDetailsFrame>()
            //         .SelectItem(_selectedItemId);
            m_UIItemDetailsGO.SetActive(true);


            ItemDataHolder item = InventoryManager.GetItemByEntityId(_selectedItemId);

            if (item == null)
            {
                return;
            }

            m_ItemIcon.sprite = item.GetIcon();
            m_ItemNameTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES, item.GetItemNameKey());
            m_ItemDescTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_DESC, item.GetItemDescription());
            m_ItemTypeTmp.text = item.GetItemType().ToString();
            bool isConsumable = item.GetItemType() == EItemType.Consumable;
            bool isRelic = item.GetItemType() == EItemType.Relic;

            if (isRelic)
            {
                m_UseBtn.gameObject.SetActive(false);
                m_EquipBtn.gameObject.SetActive(item.GetEquipmentSlot() == GameConstants.EMPTY_RELIC_SLOT);
                m_UnequipBtn.gameObject.SetActive(item.GetEquipmentSlot() != GameConstants.EMPTY_RELIC_SLOT);
                return;
            }

            m_EquipBtn.gameObject.SetActive(false);
            m_UnequipBtn.gameObject.SetActive(false);
            m_UseBtn.gameObject.SetActive(isConsumable);
        }

        private void CloseDetails()
        {
            m_UIItemDetailsGO.SetActive(false);
            _selectedItemId = string.Empty;
            _selectedSlot = GameConstants.EMPTY_RELIC_SLOT;

            m_ItemIcon.sprite = null;
            m_ItemNameTmp.text = string.Empty;
            m_ItemDescTmp.text = string.Empty;

            m_UseBtn.gameObject.SetActive(false);
            m_EquipBtn.gameObject.SetActive(false);
        }

        private void Back()
        {
            HideThisView();
            UIManager.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
        }

        private void ClearAllUIElements()
        {
            if (!PoolManager.Pools[GameConstants.POOL_UI_INVENTORY_ITEM].IsEmpty)
            {
                PoolManager.Pools[GameConstants.POOL_UI_INVENTORY_ITEM].DespawnAll();
            }
            if (_slots.Count > 0)
            {
                _slots.Clear();
            }
        }

        private void UseItem()
        {
            if (string.IsNullOrEmpty(_selectedItemId))
            {
                return;
            }

            if (InventoryManager.UseItem(_selectedItemId, 1))
            {
                LocalDBOrchestrator.UpdateDBChangeTime();
            }
        }

        private void EquipRelic()
        {
            if (string.IsNullOrEmpty(_selectedItemId))
            {
                Debug.Log("[UIInventoryFrame.EquipRelic error] The player haven't selected any relic item yet!");
                return;
            }

            Debug.Log("[UIInventoryFrame.EquipRelic] Prepare to equip");

            EEequipRelicResult result = InventoryManager.EquipRelic(_selectedItemId);

            if (result == EEequipRelicResult.Success)
            {
                LocalDBOrchestrator.UpdateDBChangeTime();

                DisplayItems(EItemType.None);
            }
            else
            {
                Debug.Log($"<color=red>[UIInventoryFrame.EquipRelic error]</color> Failed to equip relic!\nResult: {result.ToString()}");
            }
        }

        private void UnequipRelic()
        {
            if (string.IsNullOrEmpty(_selectedItemId))
            {
                return;
            }
            if (InventoryManager.UnequipRelic(_selectedItemId, 0))
            {
                LocalDBOrchestrator.UpdateDBChangeTime();
                DisplayItems(EItemType.None);
            }
        }

        private void OnClickRelicSlot(string sessionRelicId, int slot)
        {
            if (!string.IsNullOrEmpty(sessionRelicId))
            {
                // Show relic details.
                SelectItem("", sessionRelicId);
                _selectedSlot = slot;
            }
        }

        private UIRelicSlot GetRelicAtSlot(int slot)
        {
            if (slot < 0 || slot > 2)
            {
                return null;
            }
            return m_RelicSlotArray.FirstOrDefault(r => r.Slot == slot);
        }
    }
}