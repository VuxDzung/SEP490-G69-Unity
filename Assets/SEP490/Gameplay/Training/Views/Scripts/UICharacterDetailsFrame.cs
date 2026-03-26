namespace SEP490G69.Training
{
    using SEP490G69.Battle;
    using SEP490G69.Battle.Combat;
    using SEP490G69.Economy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICharacterDetailsFrame : GameUIFrame
    {
        [SerializeField] private Button m_Back;

        [SerializeField] private UITextStatSlider m_VitSlider;
        [SerializeField] private UITextStatSlider m_PowerSlider;
        [SerializeField] private UITextStatSlider m_AgiSlider;
        [SerializeField] private UITextStatSlider m_INTSlider;
        [SerializeField] private UITextStatSlider m_StaminaSlider;

        [SerializeField] private List<UIRelicSlot> m_RelicSlotList = new List<UIRelicSlot>();

        [Header("In-combat stats")]
        [SerializeField] private TextMeshProUGUI m_CombatHPTmp;
        [SerializeField] private TextMeshProUGUI m_CritRateTmp;
        [SerializeField] private TextMeshProUGUI m_CombatStaTmp;
        [SerializeField] private TextMeshProUGUI m_CritMulTmp;
        [SerializeField] private TextMeshProUGUI m_CombatDefTmp;

        [SerializeField] private TextMeshProUGUI m_CharacterNameTmp;

        private IMaxHPCalculator _hpCalculator = new MaxHPCalculator();
        private IMaxStaminaCalculator _staminaCalculator = new MaxStaminaCalculator();
        private ICritCalculator _critCalculator = new CombatCritCalculator();

        private GameTrainingController _trainingController;
        private GameTrainingController TrainingController
        {
            get
            {
                if (_trainingController == null)
                {
                    ContextManager.Singleton.TryResolveSceneContext(out _trainingController);
                }
                return _trainingController;
            }
        }

        private GameInventoryManager _inventoryManager;
        private GameInventoryManager InventoryManager
        {
            get
            {
                if (_inventoryManager == null)
                {
                    _inventoryManager = ContextManager.Singleton.ResolveGameContext<GameInventoryManager>();
                }
                return _inventoryManager;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_Back.onClick.AddListener(Back);
            LoadCharacterDetails(TrainingController.CharacterData);
            LoadCharacterStats();
            SetupRelicSlots();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_Back.onClick.RemoveListener(Back);
            DeallocateRelicSlots();
        }

        private void SetupRelicSlots()
        {
            foreach (var slot in m_RelicSlotList)
            {
                slot.SetOnClickCallback(SelectRelicAtSlot);
            }
        }
        private void DeallocateRelicSlots()
        {
            foreach (var slot in m_RelicSlotList)
            {
                slot.SetOnClickCallback(null);
            }
        }

        private void LoadCharacterDetails(CharacterDataHolder characterHolder)
        {
            m_CharacterNameTmp.text = characterHolder.GetCharacterName();

            LoadPassives(null);
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
        }

        private void LoadPassives(List<string> passiveIdList)
        {
            if (passiveIdList == null || passiveIdList.Count == 0)
            {
                return;
            }
        }

        private void LoadCharacterStats()
        {
            IReadOnlyList<ItemDataHolder> relics = InventoryManager.GetAllRelics();

            float currentVit = TrainingController.CharacterData.GetVIT();
            float currentPow = TrainingController.CharacterData.GetPower();
            float currentInt = TrainingController.CharacterData.GetINT();
            float currentAgi = TrainingController.CharacterData.GetAgi();
            float currentSta = TrainingController.CharacterData.GetStamina();
            float currentDef = TrainingController.CharacterData.GetDef();

            float finalVit = currentVit;
            float finalPow = currentPow;
            float finalInt = currentInt;
            float finalAgi = currentAgi;
            float finalSta = currentSta;
            float finalDef = currentDef;

            float extraVit = 0;
            float extraPow = 0;
            float extraInt = 0;
            float extraAgi = 0;
            float extraSta = 0;
            float extraDef = 0;

            foreach (var slot in m_RelicSlotList)
            {
                slot.SetEmpty();
            }

            if (relics.Count > 0)
            {
                foreach (ItemDataHolder relic in relics)
                {
                    extraVit = relic.CalculateRelicModValue(EStatusType.Vitality, currentVit);
                    finalVit += extraVit;

                    extraPow = relic.CalculateRelicModValue(EStatusType.Power, currentPow);
                    finalPow += extraPow;

                    extraInt = relic.CalculateRelicModValue(EStatusType.Intelligence, currentInt);
                    finalInt += extraInt;

                    extraAgi = relic.CalculateRelicModValue(EStatusType.Agi, currentAgi);
                    finalAgi += extraAgi;

                    extraSta = relic.CalculateRelicModValue(EStatusType.Agi, currentSta);
                    finalSta += extraSta;

                    extraDef = relic.CalculateRelicModValue(EStatusType.Defense, currentDef);
                    finalDef += extraDef;

                    UIRelicSlot slot = GetSlot(relic.GetEquipmentSlot());
                    if (slot != null)
                    {
                        string relicName = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES, relic.GetItemNameKey());
                        slot.SetRelicInfo(relic.GetSessionItemId(), relic.GetIcon(), relicName);
                    }
                }
            }

            SetVitality(currentVit, CharacterStatUtils.GetStatRankMaxValue(currentVit), extraVit, finalVit);
            SetPower(currentPow, CharacterStatUtils.GetStatRankMaxValue(currentPow), extraPow, finalPow);
            SetINT(currentInt, CharacterStatUtils.GetStatRankMaxValue(currentInt), extraInt, finalInt);
            SetAgility(currentAgi, CharacterStatUtils.GetStatRankMaxValue(currentAgi), extraAgi, finalAgi);
            SetStamina(currentSta, CharacterStatUtils.GetStatRankMaxValue(currentSta), extraSta, finalSta);

            // Load combat stats.
            float maxHP = _hpCalculator.Calculate(finalVit);
            float critRate = _critCalculator.CalculateCritChance(finalVit);
            float critMul = _critCalculator.CalculateCritMul(finalPow);
            float combatStamina = _staminaCalculator.CalculateMax(finalSta);

            m_CombatHPTmp.text = Math.Round(maxHP, 0).ToString();
            m_CritRateTmp.text = Math.Round(critRate, 0).ToString();
            m_CritMulTmp.text = Math.Round(critMul, 0).ToString();
            m_CombatStaTmp.text = Math.Round(combatStamina, 0).ToString();
            m_CombatDefTmp.text = Math.Round(finalDef, 0).ToString();
        }

        public void SetVitality(float cur, float max, float extra, float final)
        {
            m_VitSlider.SetValue(cur, max, extra, final);
            m_VitSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetPower(float cur, float max, float extra, float final)
        {
            m_PowerSlider.SetValue(cur, max, extra, final);
            m_PowerSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetAgility(float cur, float max, float extra, float final)
        {
            m_AgiSlider.SetValue(cur, max, extra, final);
            m_AgiSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetINT(float cur, float max, float extra, float final)
        {
            m_INTSlider.SetValue(cur, max, extra, final);
            m_INTSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetStamina(float cur, float max, float extra, float final)
        {
            m_StaminaSlider.SetValue(cur, max, extra, final);
            m_StaminaSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }

        private void SelectRelicAtSlot(string relicId, int slot)
        {

        }

        private UIRelicSlot GetSlot(int slotIndex)
        {
            if (slotIndex < 0) return null;
            return m_RelicSlotList.FirstOrDefault(r => r.Slot == slotIndex);
        }
    }
}