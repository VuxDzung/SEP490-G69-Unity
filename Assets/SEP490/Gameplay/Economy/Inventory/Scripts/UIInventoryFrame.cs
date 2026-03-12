namespace SEP490G69.Economy
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIInventoryFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;

        [Header("Character info")]
        [SerializeField] private Image m_CharacterImg;
        [SerializeField] private TextMeshProUGUI m_CharacterNameTmp;
        [SerializeField] private Transform[] m_RelicSlotArray;

        [Header("Item list")]
        [SerializeField] private Transform m_ItemContainer;
        [SerializeField] private Transform m_ItemSlotPrefab;

        [Header("Filter buttons")]
        [SerializeField] private Button m_AllItemsBtn;
        [SerializeField] private Button m_UsableItemsBtn;
        [SerializeField] private Button m_RelicItemsBtn;

        private string _selectedItemId;
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

            DisplayItems(EItemType.None);

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
            ClearAllUIElements();
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

            var items = InventoryManager.GetAllItems();

            foreach (var item in items)
            {
                if (itemType != EItemType.None && item.GetItemType() != itemType)
                    continue;

                Transform slotTrans = PoolManager.Pools[GameConstants.POOL_UI_INVENTORY_ITEM].Spawn(m_ItemSlotPrefab, m_ItemContainer);

                UIInventoryItemSlot slot = slotTrans.GetComponent<UIInventoryItemSlot>();

                if (slot == null) continue;

                slot.BindInventoryItem(item).SetClickAction(SelectItem);

                _slots.Add(slot);
            }

            CloseDetails();
        }

        private void SelectItem(string rawItemId)
        {
            _selectedItemId = rawItemId;
            UIManager.ShowFrame(GameConstants.FRAME_ID_INVENTORY_ITEM_DETAILS)
                     .AsFrame<UIInventoryItemDetailsFrame>()
                     .SelectItem(_selectedItemId);
        }

        private void CloseDetails()
        {
            UIManager.HideFrame(GameConstants.FRAME_ID_INVENTORY_ITEM_DETAILS);
        }

        private void Back()
        {
            HideThisView();
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
    }
}