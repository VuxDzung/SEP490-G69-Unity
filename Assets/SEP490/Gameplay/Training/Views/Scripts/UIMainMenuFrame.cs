namespace SEP490G69.Training
{
    using SEP490G69.Calendar;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIMainMenuFrame : GameUIFrame
    {
        [SerializeField] private TextMeshProUGUI m_CalendarTimeTmp;
        [SerializeField] private TextMeshProUGUI m_RemainTimeTmp;
        [Header("Right vertical fields")]
        [SerializeField] private Button m_SettingsBtn;
        [SerializeField] private Button m_NoAdsBtn;
        [SerializeField] private Button m_HallOfFameBtn;
        [SerializeField] private Button m_ShopBtn;

        [Header("Actions")]
        [SerializeField] private Button m_TrainingBtn;
        [SerializeField] private Button m_RestBtn;
        [SerializeField] private Button m_DeckBtn;
        [SerializeField] private Button m_InventoryBtn;
        [SerializeField] private Button m_TournamentBtn;
        [SerializeField] private Button m_CharacterDetailsBtn;
        [SerializeField] private Button m_PlayerProfileBtn;

        [Header("Character stats")]
        [SerializeField] private TextMeshProUGUI m_MoodTmp;
        [SerializeField] private UITextSlider m_RPSlider;
        [SerializeField] private UITextSlider m_EnergySlider;

        [SerializeField] private UITextSlider m_VitSlider;
        [SerializeField] private UITextSlider m_PowerSlider;
        [SerializeField] private UITextSlider m_AgiSlider;
        [SerializeField] private UITextSlider m_INTSlider;
        [SerializeField] private UITextSlider m_StaminaSlider;

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

        private GameCalendarController _calendarController;
        private GameCalendarController CalendarController
        {
            get
            {
                if (_calendarController == null)
                {
                    ContextManager.Singleton.TryResolveSceneContext(out _calendarController);
                }
                return _calendarController;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
#if !UNITY_ANDROID
            m_NoAdsBtn.gameObject.SetActive(false);
#else
            m_NoAdsBtn.gameObject.SetActive(true);
#endif
            m_SettingsBtn.onClick.AddListener(ShowSettingsFrame);
            m_NoAdsBtn.onClick.AddListener(PurchasePremium);
            m_HallOfFameBtn.onClick.AddListener(ShowHallOfFame);
            m_ShopBtn.onClick.AddListener(ShowShop);

            m_TrainingBtn.onClick.AddListener(ShowTrainingMenu);
            m_RestBtn.onClick.AddListener(PerformRest);
            m_TournamentBtn.onClick.AddListener(ShowCalendar);
            m_DeckBtn.onClick.AddListener(ShowDeck);
            m_CharacterDetailsBtn.onClick.AddListener(ShowCharacterDetails);

            if (m_PlayerProfileBtn) m_PlayerProfileBtn.onClick.AddListener(ShowPlayerProfile);

            LoadCharacterStats();
            LoadCalendarTime();
            LoadObjectives();
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_SettingsBtn.onClick.RemoveListener(ShowSettingsFrame);
            m_NoAdsBtn.onClick.RemoveListener(PurchasePremium);
            m_HallOfFameBtn.onClick.RemoveListener(ShowHallOfFame);
            m_ShopBtn.onClick.RemoveListener(ShowShop);

            m_TrainingBtn.onClick.RemoveListener(ShowTrainingMenu);
            m_RestBtn.onClick.RemoveListener(PerformRest);
            m_TournamentBtn.onClick.RemoveListener(ShowCalendar);
            m_DeckBtn.onClick.RemoveListener(ShowDeck);
            m_CharacterDetailsBtn.onClick.RemoveListener(ShowCharacterDetails);

            if (m_PlayerProfileBtn) m_PlayerProfileBtn.onClick.RemoveListener(ShowPlayerProfile);
        }

        private void LoadCharacterStats()
        {
            SetEnergy(TrainingController.CharacterData.GetEnergy(), GameConstants.MAX_100);
            SetRP(TrainingController.CharacterData.GetRP(), GameConstants.DetermineNextRPCheckpoint(TrainingController.CharacterData.GetRP()));

            SetVitality(TrainingController.CharacterData.GetVIT(), GameConstants.MAX_STAT_VALUE);
            SetPower(TrainingController.CharacterData.GetPower(), GameConstants.MAX_STAT_VALUE);
            SetINT(TrainingController.CharacterData.GetINT(), GameConstants.MAX_STAT_VALUE);
            SetAgility(TrainingController.CharacterData.GetAgi(), GameConstants.MAX_STAT_VALUE);
            SetStamina(TrainingController.CharacterData.GetStamina(), GameConstants.MAX_STAT_VALUE);
        }
        private void LoadCalendarTime()
        {
            m_CalendarTimeTmp.text = CalendarController.GetCalendarTime();
            m_RemainTimeTmp.text = CalendarController.GetRemainTimeOfYear().ToString();
        }
        private void LoadObjectives()
        {

        }

        public void SetEnergy(float cur, float max)
        {
            m_EnergySlider.SetValue(cur, max);
        }
        public void SetRP(int cur, int max)
        {
            m_RPSlider.SetValue(cur, max);
        }
        public void SetMood(string mood)
        {
            m_MoodTmp.text = string.Format(GameConstants.MOOD_FORMAT, mood);
        }

        public void SetVitality(float cur, float max)
        {
            m_VitSlider.SetValue(cur, max);
        }
        public void SetPower(float cur, float max)
        {
            m_PowerSlider.SetValue(cur, max);
        }
        public void SetAgility(float cur, float max)
        {
            m_AgiSlider.SetValue(cur, max);
        }
        public void SetINT(float cur, float max)
        {
            m_INTSlider.SetValue(cur, max);
        }
        public void SetStamina(float cur, float max)
        {
            m_StaminaSlider.SetValue(cur, max);
        }

        private void ShowCharacterDetails()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_CHAR_DETAILS);
        }

        #region Actions
        private void ShowSettingsFrame()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_IN_GAME_SETTINGS);
        }
        private void PurchasePremium()
        {

        }
        private void ShowHallOfFame()
        {

        }
        private void ShowShop()
        {

        }

        private void ShowTrainingMenu()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_TRAINING_MENU);

            TrainingController.HideMainMenuBG();
            TrainingController.OpenTrainingMenuBG();
        }

        private void PerformRest()
        {
            TrainingController.StartTraining(ETrainingType.Rest);
            LoadCharacterStats();
        }

        private void ShowCalendar()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_CALENDAR);
        }
        private void ShowDeck()
        {

        }
        private void ShowPlayerProfile()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_PLAYER_PROFILE);
        }
        #endregion
    }
}