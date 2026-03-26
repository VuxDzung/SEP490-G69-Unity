namespace SEP490G69.Training
{
    using SEP490G69.Economy;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICharacterDetailsFrame : GameUIFrame
    {
        [SerializeField] private Button m_Back;

        [SerializeField] private UITextSlider m_VitSlider;
        [SerializeField] private UITextSlider m_PowerSlider;
        [SerializeField] private UITextSlider m_AgiSlider;
        [SerializeField] private UITextSlider m_INTSlider;
        [SerializeField] private UITextSlider m_StaminaSlider;

        [SerializeField] private Button m_PassiveBtnPrefab;

        [SerializeField] private TextMeshProUGUI m_CharacterNameTmp;
        [SerializeField] private Image m_CharacterImg;

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
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_Back.onClick.RemoveListener(Back);
        }

        private void LoadCharacterDetails(CharacterDataHolder characterHolder)
        {
            m_CharacterNameTmp.text = characterHolder.GetCharacterName();
            m_CharacterImg.sprite = characterHolder.GetFullBodyImg();

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

            if (relics.Count > 0)
            {
                foreach (var relic in relics)
                {
                    currentVit += relic.CalculateRelicModValue(EStatusType.Vitality, currentVit);
                    currentPow += relic.CalculateRelicModValue(EStatusType.Power, currentPow);
                    currentInt += relic.CalculateRelicModValue(EStatusType.Intelligence, currentInt);
                    currentAgi += relic.CalculateRelicModValue(EStatusType.Agi, currentAgi);
                    currentSta += relic.CalculateRelicModValue(EStatusType.Agi, currentSta);
                }
            }

            SetVitality(currentVit, CharacterStatUtils.GetStatRankMaxValue(currentVit));
            SetPower(currentPow, CharacterStatUtils.GetStatRankMaxValue(currentPow));
            SetINT(currentInt, CharacterStatUtils.GetStatRankMaxValue(currentInt));
            SetAgility(currentAgi, CharacterStatUtils.GetStatRankMaxValue(currentAgi));
            SetStamina(currentSta, CharacterStatUtils.GetStatRankMaxValue(currentSta));
        }

        public void SetVitality(float cur, float max)
        {
            m_VitSlider.SetValue(cur, max);
            m_VitSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetPower(float cur, float max)
        {
            m_PowerSlider.SetValue(cur, max);
            m_PowerSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetAgility(float cur, float max)
        {
            m_AgiSlider.SetValue(cur, max);
            m_AgiSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetINT(float cur, float max)
        {
            m_INTSlider.SetValue(cur, max);
            m_INTSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetStamina(float cur, float max)
        {
            m_StaminaSlider.SetValue(cur, max);
            m_StaminaSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
    }
}