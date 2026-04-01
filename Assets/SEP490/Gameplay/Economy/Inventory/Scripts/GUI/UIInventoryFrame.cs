namespace SEP490G69.Economy
{
    using SEP490G69.Battle;
    using SEP490G69.Battle.Combat;
    using SEP490G69.GameSessions;
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
        //[SerializeField] private UITabButton m_AllItemsBtn;
        //[SerializeField] private UITabButton m_UsableItemsBtn;
        //[SerializeField] private UITabButton m_RelicItemsBtn;
        [SerializeField] private TMP_Dropdown m_FilterDropdown;

        [Header("Item details")]
        [SerializeField] private GameObject m_UIItemDetailsGO;
        [SerializeField] private Image m_ItemIcon;
        [SerializeField] private Image m_ItemRarityBoder;
        [SerializeField] private TextMeshProUGUI m_ItemNameTmp;
        [SerializeField] private TextMeshProUGUI m_ItemDescTmp;
        [SerializeField] private TextMeshProUGUI m_ItemTypeTmp;
        [SerializeField] private Button m_UseBtn;
        [SerializeField] private Button m_EquipBtn;
        [SerializeField] private Button m_UnequipBtn;

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
        [Header("Character Stats")]
        [Header("Character Base Stats")]
        [SerializeField] private UIStatInventory m_CharacterStatVIT;
        [SerializeField] private UIStatInventory m_CharacterStatPOW;
        [SerializeField] private UIStatInventory m_CharacterStatAGI;
        [SerializeField] private UIStatInventory m_CharacterStatINT;
        [SerializeField] private UIStatInventory m_CharacterStatSTA;
        [Header("Combat Stats")]
        [SerializeField] private UIStatInventory m_CombatStatHP;
        [SerializeField] private UIStatInventory m_CombatStatSTA;
        [SerializeField] private UIStatInventory m_CombatStatDEF;
        [SerializeField] private UIStatInventory m_CombatStatCritRate;
        [SerializeField] private UIStatInventory m_CombatStatCritMul;

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

        private CharacterConfigSO _characterConfig;
        private CharacterConfigSO CharacterConfig
        {
            get
            {
                if (_characterConfig == null)
                {
                    _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
                }
                return _characterConfig;
            }
        }

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

        private CharacterDataHolder _characterHolder;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BackBtn.onClick.AddListener(Back);

            //m_AllItemsBtn.Enable();
            //m_UsableItemsBtn.Enable();
            //m_RelicItemsBtn.Enable();

            //m_AllItemsBtn.SetCategory(EItemType.None.ToString(), FilterItems);
            //m_UsableItemsBtn.SetCategory(EItemType.Consumable.ToString(), FilterItems);
            //m_RelicItemsBtn.SetCategory(EItemType.Relic.ToString(), FilterItems);

            EventManager.Subscribe<AddItemEvent>(OnInventoryUpdated);
            EventManager.Subscribe<UseItemEvent>(OnInventoryUpdated);
            EventManager.Subscribe<EquipRelicEvent>(OnInventoryUpdated);
            EventManager.Subscribe<UnequipRelicEvent>(OnInventoryUpdated);

            m_EquipBtn.onClick.AddListener(EquipRelic);
            m_UseBtn.onClick.AddListener(UseItem);
            m_UnequipBtn.onClick.AddListener(UnequipRelic);

            //FilterItems(EItemType.None.ToString());
            FilterAllItems();
            SetupRelicSlots();

            SetupFilterDropdown();
            m_FilterDropdown.onValueChanged.AddListener(OnDropdownChanged);

            LoadCharacterDetails(GetCharacterData(), null);
            DisableItemPreviewStats();
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            ClearFilterOptions();
            //m_AllItemsBtn.Disable();
            //m_UsableItemsBtn.Disable();
            //m_RelicItemsBtn.Disable();

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

        private void OnDropdownChanged(int index)
        {
            EItemType itemType = (EItemType)index;
            DisplayItems(itemType);
        }

        private void SetupFilterDropdown()
        {
            TMP_Dropdown.OptionData allOption = new TMP_Dropdown.OptionData
            {
                text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, GameConstants.ConvertItemType2LocalizeId(EItemType.None))
            };
            TMP_Dropdown.OptionData usableOption = new TMP_Dropdown.OptionData
            {
                text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, GameConstants.ConvertItemType2LocalizeId(EItemType.Consumable))
            };
            TMP_Dropdown.OptionData relicOption = new TMP_Dropdown.OptionData
            {
                text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, GameConstants.ConvertItemType2LocalizeId(EItemType.Relic))
            };

            m_FilterDropdown.ClearOptions();
            m_FilterDropdown.AddOptions(new List<TMP_Dropdown.OptionData>
            {
                allOption, usableOption, relicOption
            });
        }

        private void ClearFilterOptions()
        {
            m_FilterDropdown.ClearOptions();
        }

        //public void FilterItems(string category)
        //{
        //    m_AllItemsBtn.Deselect();
        //    m_UsableItemsBtn.Deselect();
        //    m_RelicItemsBtn.Deselect();

        //    switch(category)
        //    {
        //        case "None":
        //            m_AllItemsBtn.Select();
        //            FilterAllItems();
        //            break;
        //        case "Consumable":
        //            m_UsableItemsBtn.Select();
        //            FilterUsableItems();
        //            break;
        //        case "Relic":
        //            m_RelicItemsBtn.Select();
        //            FilterRelicItems();
        //            break;
        //    }
        //}

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

            IReadOnlyList<ItemDataHolder> items = InventoryManager.GetAllItems();
            Debug.Log($"DisplayItems: {items.Count}");
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
            foreach (var slot in m_RelicSlotArray)
            {
                slot.SetEmpty();
            }
            LoadRelicSlots();
            CloseDetails();
            LoadCharacterDetails(GetCharacterData(), null);
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

            m_UIItemDetailsGO.SetActive(true);

            ItemDataHolder item = InventoryManager.GetItemByEntityId(_selectedItemId);

            if (item == null)
            {
                return;
            }

            m_ItemIcon.sprite = item.GetIcon();
            Sprite rarityIcon = ItemRarityImgConfig.GetById(GameConstants.ConvertRarityToImgId(item.GetRarity()))?.image;
            m_ItemRarityBoder.sprite = rarityIcon;

            m_ItemNameTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES, item.GetItemNameKey());
            m_ItemDescTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_DESC, item.GetItemDescription());
            m_ItemTypeTmp.text = item.GetItemType().ToString();
            bool isConsumable = item.GetItemType() == EItemType.Consumable;
            bool isRelic = item.GetItemType() == EItemType.Relic;

            DisableItemPreviewStats();

            if (isRelic)
            {
                m_UseBtn.gameObject.SetActive(false);
                m_EquipBtn.gameObject.SetActive(item.GetEquipmentSlot() == GameConstants.EMPTY_RELIC_SLOT);
                m_UnequipBtn.gameObject.SetActive(item.GetEquipmentSlot() != GameConstants.EMPTY_RELIC_SLOT);

                IReadOnlyList<StatusModifierSO> relicModifiers = item.GetRelicModifiers();

                foreach (var modifier in relicModifiers)
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

                        // Combat stats
                        case EStatusType.HP:
                            DisplayRelicStatChanges(m_RelicFlatHP, m_RelicPercentHP, modifier.Value, modifier.Operator);
                            break;
                    }
                }
                LoadCharacterStatPreview(item);
            }
            else
            {
                m_EquipBtn.gameObject.SetActive(false);
                m_UnequipBtn.gameObject.SetActive(false);
                m_UseBtn.gameObject.SetActive(isConsumable);

                IReadOnlyList<StatusModifierSO> usableModifiers = item.GetUsableModifiers();
                foreach (var modifier in usableModifiers)
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

                        // Combat stats
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
                LoadCharacterStatPreview(null);
            }
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

        private void DisplayRelicStatChanges(UIStatInventory statFlatUI, UIStatInventory statPercentUI, float value, EOperator op)
        {
            if (op == EOperator.PercentAdd || op == EOperator.PercentSub)
            {
                statPercentUI.Enable();
                statPercentUI.SetPercentValue(op == EOperator.PercentSub ? -1 : 1 * value);
            }
            else if (op == EOperator.FlatAdd || op == EOperator.FlatSub)
            {
                statFlatUI.Enable();
                statFlatUI.SetFlatValue(op == EOperator.FlatSub ? -1 : 1 * value);
            }
        }

        private void LoadCharacterStatPreview(ItemDataHolder relicHolder)
        {
            LoadCharacterDetails(GetCharacterData(), relicHolder);
        }

        private void LoadCharacterDetails(CharacterDataHolder characterHolder, ItemDataHolder previewRelic)
        {
            m_CharacterNameTmp.text = characterHolder.GetCharacterName();

            IReadOnlyList<ItemDataHolder> relics = InventoryManager.GetAllRelics();

            // Current relic list
            List<ItemDataHolder> currentRelics = new List<ItemDataHolder>(relics);

            // Preview relic list
            List<ItemDataHolder> previewRelics = new List<ItemDataHolder>(relics);
            if (previewRelic != null)
                previewRelics.Add(previewRelic);

            // ===== Calculate =====
            CalculateStats(characterHolder, currentRelics,
                out float curVit, out float curPow, out float curInt,
                out float curAgi, out float curSta, out float curDef);

            CalculateStats(characterHolder, previewRelics,
                out float preVit, out float prePow, out float preInt,
                out float preAgi, out float preSta, out float preDef);

            // ===== UI =====
            if (previewRelic != null)
            {
                // Show preview
                m_CharacterStatVIT.SetPreviewValue(curVit, preVit);
                m_CharacterStatPOW.SetPreviewValue(curPow, prePow);
                m_CharacterStatINT.SetPreviewValue(curInt, preInt);
                m_CharacterStatAGI.SetPreviewValue(curAgi, preAgi);
                m_CharacterStatSTA.SetPreviewValue(curSta, preSta);
                m_CombatStatDEF.SetPreviewValue(curDef, preDef);
            }
            else
            {
                // Show normal
                m_CharacterStatVIT.SetValue(curVit);
                m_CharacterStatPOW.SetValue(curPow);
                m_CharacterStatINT.SetValue(curInt);
                m_CharacterStatAGI.SetValue(curAgi);
                m_CharacterStatSTA.SetValue(curSta);
                m_CombatStatDEF.SetValue(curDef);
            }

            // ===== Combat stats =====
            float staBefore = new CombatStaminaCalculator().CalculateMax(curSta);
            float hpBefore = new CombatHPCalculator().Calculate(curVit);

            float staAfter = new CombatStaminaCalculator().CalculateMax(preSta);
            float hpAfter = new CombatHPCalculator().Calculate(preVit);

            ICritCalculator critCalculator = new CombatCritCalculator();

            float critRateBefore = critCalculator.CalculateCritChance(curAgi);
            float critRateAfter = critCalculator.CalculateCritChance(preAgi);

            float critMulBefore = critCalculator.CalculateCritMul(curAgi);
            float critMulAfter = critCalculator.CalculateCritMul(preAgi);

            if (previewRelic != null)
            {
                m_CombatStatSTA.SetPreviewValue(staBefore, staAfter);
                m_CombatStatHP.SetPreviewValue(hpBefore, hpAfter);
                m_CombatStatCritRate.SetPreviewValue(critRateBefore, critRateAfter);
                m_CombatStatCritMul.SetPreviewValue(critMulBefore, critMulAfter);
            }
            else
            {
                m_CombatStatSTA.SetValue(staBefore);
                m_CombatStatHP.SetValue(hpBefore);
                m_CombatStatCritRate.SetValue(critRateBefore);
                m_CombatStatCritMul.SetValue(critMulBefore);
            }
        }

        private void CalculateStats(CharacterDataHolder characterHolder,
                                    List<ItemDataHolder> relics,
                                    out float vit, out float pow, out float intel,
                                    out float agi, out float sta, out float def)
        {
            vit = characterHolder.GetVIT();
            pow = characterHolder.GetPower();
            intel = characterHolder.GetINT();
            agi = characterHolder.GetAgi();
            sta = characterHolder.GetStamina();
            def = characterHolder.GetDef();

            foreach (var relic in relics)
            {
                vit = relic.CalculateRelicModValue(EStatusType.Vitality, vit);
                pow = relic.CalculateRelicModValue(EStatusType.Power, pow);
                intel = relic.CalculateRelicModValue(EStatusType.Intelligence, intel);
                agi = relic.CalculateRelicModValue(EStatusType.Agi, agi);
                sta = relic.CalculateRelicModValue(EStatusType.Stamina, sta);
                def = relic.CalculateRelicModValue(EStatusType.Defense, def);
            }

            vit = Mathf.Max(vit, 1);
            pow = Mathf.Max(pow, 1);
            intel = Mathf.Max(intel, 1);
            agi = Mathf.Max(agi, 1);
            sta = Mathf.Max(sta, 1);
            def = Mathf.Max(def, 1);
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
            DisableItemPreviewStats();
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

        private void LoadCharacterInfo()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID, string.Empty);
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }
            PlayerTrainingSession sessionData = new GameSessionDAO().GetById(sessionId);
            if (sessionData == null)
            {
                return;
            }

            BaseCharacterSO characterSO = CharacterConfig.GetCharacterById(sessionData.RawCharacterId);

            if (characterSO == null)
            {
                return;
            }

            m_CharacterImg.sprite = characterSO.Thumbnail;
            m_CharacterNameTmp.text = characterSO.CharacterName;
        }

        private CharacterDataHolder GetCharacterData()
        {
            if (_characterHolder == null)
            {
                string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

                PlayerTrainingSession sessionData = new GameSessionDAO().GetById(sessionId);

                SessionCharacterData characterData = new PlayerCharacterRepository().GetCharacterData(sessionId, sessionData.RawCharacterId);

                if (characterData == null)
                {
                    return null;
                }

                BaseCharacterSO characterSO = CharacterConfig.GetCharacterById(sessionData.RawCharacterId);

                _characterHolder = new CharacterDataHolder.Builder().WithCharacterData(characterData).WithCharacterSO(characterSO).Build();
            }

            return _characterHolder;
        }
    }
}