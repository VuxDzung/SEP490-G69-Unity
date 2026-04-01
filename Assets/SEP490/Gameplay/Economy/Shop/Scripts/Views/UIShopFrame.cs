namespace SEP490G69.Economy
{
    using SEP490G69.GameSessions;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIShopFrame : GameUIFrame
    {
        [Header("Shop Items")]
        [SerializeField] private Transform m_ItemContainer;
        [SerializeField] private Transform m_ItemPrefab;
        [SerializeField] private TextMeshProUGUI m_RemainGoldTmp;

        [Header("Item Details")]
        [SerializeField] private TextMeshProUGUI m_ItemType;
        [SerializeField] private Image m_ItemIcon;
        [SerializeField] private Image m_ItemRarityBorder;
        [SerializeField] private GameObject m_ItemDetailsGO;
        [SerializeField] private TextMeshProUGUI m_ItemNameTmp;
        [SerializeField] private TextMeshProUGUI m_ItemDescTmp;
        [SerializeField] private TextMeshProUGUI m_ItemCostTmp;
        [SerializeField] private TextMeshProUGUI m_RefreshCostTmp;

        [Header("Item Stats Details")]
        [Header("Flat Changes")]
        [SerializeField] private UIStatInventory m_RelicFlatDEF;
        [SerializeField] private UIStatInventory m_RelicFlatHP;
        [SerializeField] private UIStatInventory m_RelicFlatVIT;
        [SerializeField] private UIStatInventory m_RelicFlatPOW;
        [SerializeField] private UIStatInventory m_RelicFlatINT;
        [SerializeField] private UIStatInventory m_RelicFlatAGI;
        [SerializeField] private UIStatInventory m_RelicFlatSTA;
        [SerializeField] private UIStatInventory m_ItemFlatEnergy;
        [SerializeField] private UIStatInventory m_ItemFlatMood;
        [Header("Percent Changes")]
        [SerializeField] private UIStatInventory m_RelicPercentHP;
        [SerializeField] private UIStatInventory m_RelicPercentPOW;
        [SerializeField] private UIStatInventory m_RelicPercentINT;
        [SerializeField] private UIStatInventory m_RelicPercentAGI;
        [SerializeField] private UIStatInventory m_RelicPercentSTA;
        [SerializeField] private UIStatInventory m_ItemPercentEnergy;
        [SerializeField] private UIStatInventory m_ItemPercentMood;
        [SerializeField] private UIStatInventory m_RelicPercentDEF;
        [SerializeField] private UIStatInventory m_RelicPercentVIT;

        [Header("Buttons")]
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private Button m_BuyBtn;
        [SerializeField] private Button m_RefreshBtn;


        private List<UIShopItemElement> _slots = new();
        private ImageMasterConfigSO _imgMasterConfig;
        private ImageMasterConfigSO ImageMasterConfig
        {
            get
            {
                if (_imgMasterConfig == null)
                {
                    _imgMasterConfig = ContextManager.Singleton.GetDataSO<ImageMasterConfigSO>();
                }
                return _imgMasterConfig;
            }
        }
        private ImageConfigSO _imgRarityConfig;
        private ImageConfigSO ItemRarityImgConfig
        {
            get
            {
                if (_imgRarityConfig == null)
                {
                    _imgRarityConfig = ImageMasterConfig.GetCategoryConfig("item_rarity_icon");
                }
                return _imgRarityConfig;
            }
        }


        private string _selectedRawItemId;

        private GameShopManager _shopManager;
        private GameSessionDAO _sessionDAO;
        private GameShopManager ShopManager
        {
            get
            {
                if (_shopManager == null)
                {
                    ContextManager.Singleton.TryResolveSceneContext(out _shopManager);
                }
                return _shopManager;
            }
        }
        protected GameSessionDAO SessionDAO
        {
            get
            {
                if (_sessionDAO == null) _sessionDAO = new GameSessionDAO();
                return _sessionDAO;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BuyBtn.onClick.AddListener(BuyItem);
            m_RefreshBtn.onClick.AddListener(RefreshShop);
            m_BackBtn.onClick.AddListener(Back);

            LoadShopItems();
            LoadRemainGold();
            ReloadRefreshCost();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BuyBtn.onClick.RemoveListener(BuyItem);
            m_RefreshBtn.onClick.RemoveListener(RefreshShop);
            m_BackBtn.onClick.RemoveListener(Back);
        }

        /// <summary>
        /// Use PoolManager.Pools[GameConstants.POOL_UI_SHOP_ITEM].Spawn(itemPrefab:Transform, container:Transform)
        /// to spawn UI item.
        /// </summary>
        private void LoadShopItems()
        {
            ClearSlots();

            var items = ShopManager.GetAllAvailableShopItems();

            foreach (var item in items)
            {
                Transform itemSlotTrans = PoolManager.Pools[GameConstants.POOL_UI_SHOP_ITEM].Spawn(m_ItemPrefab, m_ItemContainer);
                UIShopItemElement slot = itemSlotTrans.GetComponent<UIShopItemElement>();

                if (slot == null) continue;

                slot.BindShopItem(item, LocalizeManager).SetClickAction(SelectItem);

                if (item.GetRemainAmount() <= 0)
                    slot.ShowSoldOut();
                else
                    slot.HideSoldOut();

                _slots.Add(slot);
            }
            m_ItemDetailsGO.SetActive(false);
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
        }
        private void RefreshShop()
        {
            ShopManager.RefreshShop();
            LoadShopItems();
            LoadRemainGold();
            ReloadRefreshCost();
        }

        private void BuyItem()
        {
            if (string.IsNullOrEmpty(_selectedRawItemId))
                return;

            ShopManager.BuyItem(_selectedRawItemId, 1);

            UpdateItemSlot(_selectedRawItemId);
            SelectItem(_selectedRawItemId, "");
            LoadRemainGold();
        }

        private void UpdateItemSlot(string itemId)
        {
            var item = ShopManager.GetAllAvailableShopItems()
                                  .FirstOrDefault(x => x.GetRawItemId() == itemId);

            if (item == null) return;

            foreach (var slot in _slots)
            {
                if (slot.RawItemId == itemId)
                {
                    slot.BindShopItem(item, LocalizeManager);

                    if (item.GetRemainAmount() <= 0)
                    {
                        slot.ShowSoldOut();
                    }
                }
            }
        }

        private void SelectItem(string rawItemId, string sessionItemId)
        {
            _selectedRawItemId = rawItemId;

            var items = ShopManager.GetAllAvailableShopItems();
            var item = items.FirstOrDefault(x => x.GetRawItemId() == _selectedRawItemId);

            Debug.Log($"Select shop item: {_selectedRawItemId}");

            if (item == null) return;

            m_ItemDetailsGO.SetActive(true);

            m_ItemType.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, GameConstants.ConvertItemType2LocalizeId(item.GetItemType()));
            m_ItemIcon.sprite = item.GetIcon();
            m_ItemNameTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES, item.GetItemName());
            m_ItemDescTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_DESC, item.GetItemDescription());
            //m_ItemCostTmp.text = $"{item.GetPrice().ToString()}G";

            m_BuyBtn.interactable = item.GetRemainAmount() > 0;

            m_ItemIcon.sprite = item.GetIcon();
            Sprite rarityIcon = ItemRarityImgConfig.GetById(GameConstants.ConvertRarityToImgId(item.GetRarity()))?.image;
            m_ItemRarityBorder.sprite = rarityIcon;

            m_ItemNameTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES, item.GetItemName());
            m_ItemDescTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_DESC, item.GetItemDescription());
            bool isRelic = item.GetItemType() == EItemType.Relic;

            DisableItemPreviewStats();

            IReadOnlyList<StatusModifierSO> modifiers = isRelic ? item.GetRelicModifiers() : item.GetUsableModifiers();
            
            foreach (var modifier in modifiers)
            {
                switch (modifier.StatType)
                {
                    // Base stats
                    case EStatusType.Power:
                        DisplayRelicStatChanges(m_RelicFlatPOW, m_RelicPercentPOW, modifier.Value, modifier.Operator);
                        break;
                    case EStatusType.Intelligence:
                        DisplayRelicStatChanges(m_RelicFlatINT, m_RelicPercentINT, modifier.Value, modifier.Operator);
                        break;
                    case EStatusType.Defense:
                        DisplayRelicStatChanges(m_RelicFlatDEF, m_RelicPercentDEF, modifier.Value, modifier.Operator);
                        break;
                    case EStatusType.Agi:
                        DisplayRelicStatChanges(m_RelicFlatAGI, m_RelicPercentAGI, modifier.Value, modifier.Operator);
                        break;
                    case EStatusType.Vitality:
                        DisplayRelicStatChanges(m_RelicFlatVIT, m_RelicPercentVIT, modifier.Value, modifier.Operator);
                        break;

                    case EStatusType.HP:
                        DisplayRelicStatChanges(m_RelicFlatHP, m_RelicPercentHP, modifier.Value, modifier.Operator);
                        break;

                    case EStatusType.Energy:
                        DisplayRelicStatChanges(m_ItemFlatEnergy, m_ItemPercentEnergy, modifier.Value, modifier.Operator);
                        break;
                    case EStatusType.Mood:
                        DisplayRelicStatChanges(m_ItemFlatMood, m_ItemPercentMood, modifier.Value, modifier.Operator);
                        break;
                }
            }
        }

        private void DisplayRelicStatChanges(UIStatInventory statFlatUI, UIStatInventory statPercentUI, float value, EOperator op)
        {
            if (op == EOperator.PercentAdd || op == EOperator.PercentSub)
            {
                statPercentUI.Enable();
                statPercentUI.SetPercentValue((op == EOperator.PercentSub ? -1 : 1) * value);
            }
            else if (op == EOperator.FlatAdd || op == EOperator.FlatSub) 
            {
                statFlatUI.Enable();
                statFlatUI.SetFlatValue((op == EOperator.FlatSub ? -1 : 1) * value);
            }
        }

        private void ClearSlots()
        {
            foreach (var slot in _slots)
            {
                PoolManager.Pools[GameConstants.POOL_UI_SHOP_ITEM].DespawnObject(slot.transform);
            }
            _slots.Clear();
        }

        private void DisableItemPreviewStats()
        {
            // Flats
            m_RelicFlatPOW.Disable();
            m_RelicFlatINT.Disable();
            m_RelicFlatDEF.Disable();
            m_RelicFlatAGI.Disable();
            m_RelicFlatVIT.Disable();
            m_RelicFlatSTA.Disable();
            m_RelicFlatHP.Disable();

            m_ItemFlatEnergy.Disable();
            m_ItemFlatMood.Disable();

            // Percents
            m_RelicPercentPOW.Disable();
            m_RelicPercentINT.Disable();
            m_RelicPercentDEF.Disable();
            m_RelicPercentAGI.Disable();
            m_RelicPercentVIT.Disable();
            m_RelicPercentSTA.Disable();
            m_RelicPercentHP.Disable();

            m_ItemPercentEnergy.Disable();
            m_ItemPercentMood.Disable();
        }

        private void LoadRemainGold()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError($"[UIMainMenuFrame] Session id is null/empty");
                return;
            }
            PlayerTrainingSession sessionData = SessionDAO.GetById(sessionId);

            if (sessionData == null)
            {
                Debug.LogError($"[UIMainMenuFrame] Session data with id {sessionId} does not exist");
                return;
            }
            Debug.Log($"[UIShopFrame.LoadRemainGold] Gold: {sessionData.CurrentGoldAmount}");
            m_RemainGoldTmp.text = sessionData.CurrentGoldAmount.ToString();// NumberFormatter.FormatGold(sessionData.CurrentGoldAmount);
        }

        private void ReloadRefreshCost()
        {
            m_RefreshCostTmp.text = $"({ShopManager.CalculateRefreshCost().ToString()}G)";
        }
    }
}