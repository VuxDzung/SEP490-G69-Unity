namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Calendar;
    using SEP490G69.Tournament;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIMainMenuFrame : GameUIFrame
    {
        [SerializeField] private TextMeshProUGUI m_CalendarTimeTmp;
        [SerializeField] private TextMeshProUGUI m_RemainTimeTmp;
        [SerializeField] private TextMeshProUGUI m_CurrentTurnTmp;
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

        [SerializeField] private Button m_TestCombatBtn;
        [SerializeField] private Button m_TestTournamentBtn;

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
        private TooltipController _tooltipController;
        private TooltipController TooltipController
        {
            get
            {
                if (_tooltipController == null) _tooltipController = ContextManager.Singleton.ResolveGameContext<TooltipController>();
                return _tooltipController;
            }
        }

        private TournamentProgressDAO _tournamentDAO;
        protected TournamentProgressDAO TournamentDAO
        {
            get
            {
                if (_tournamentDAO == null)
                {
                    _tournamentDAO = new TournamentProgressDAO();
                }
                return _tournamentDAO;
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
            m_InventoryBtn.onClick.AddListener(ShowInventory);

            if (m_PlayerProfileBtn) m_PlayerProfileBtn.onClick.AddListener(ShowPlayerProfile);
            if (m_TestCombatBtn) m_TestCombatBtn.onClick.AddListener(TestShowCombat);
            if (m_TestTournamentBtn) m_TestTournamentBtn.onClick.AddListener(TestTournament);

            m_ShopBtn.interactable = !HasAnyActiveTournament();
            m_TrainingBtn.interactable = !HasAnyActiveTournament();
            m_RestBtn.interactable = !HasAnyActiveTournament();
            m_DeckBtn.interactable = !HasAnyActiveTournament();
            m_CharacterDetailsBtn.interactable = !HasAnyActiveTournament();
            m_InventoryBtn.interactable = !HasAnyActiveTournament();

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
            m_InventoryBtn.onClick.RemoveListener(ShowInventory);

            if (m_PlayerProfileBtn) m_PlayerProfileBtn.onClick.RemoveListener(ShowPlayerProfile);

            if (m_TestCombatBtn) m_TestCombatBtn.onClick.RemoveListener(TestShowCombat);
            if (m_TestTournamentBtn) m_TestTournamentBtn.onClick.RemoveListener(TestTournament);
        }

        private void LoadCharacterStats()
        {
            SetEnergy(TrainingController.CharacterData.GetEnergy(), GameConstants.MAX_100);
            SetRP(TrainingController.CharacterData.GetRP(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetRP()));
            SetMood(TrainingController.CharacterData.GetMood());

            SetVitality(TrainingController.CharacterData.GetVIT(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetVIT()));
            SetPower(TrainingController.CharacterData.GetPower(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetPower()));
            SetINT(TrainingController.CharacterData.GetINT(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetINT()));
            SetAgility(TrainingController.CharacterData.GetAgi(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetAgi()));
            SetStamina(TrainingController.CharacterData.GetStamina(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetStamina()));
        }
        private void LoadCalendarTime()
        {
            m_CalendarTimeTmp.text = CalendarController.GetCalendarTime();
            m_RemainTimeTmp.text = CalendarController.GetRemainTimeOfYear().ToString();

            string turnStr = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_TEXT, "txt_turn");

            m_CurrentTurnTmp.text = $"{turnStr}: {CalendarController.GetCurrentWeek()}";
        }
        private void LoadObjectives()
        {

        }

        public void SetEnergy(float cur, float max)
        {
            m_EnergySlider.SetValue(cur, max);
            m_EnergySlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetRP(int cur, int max)
        {
            m_RPSlider.SetValue(cur, max);
            m_RPSlider.SetRank(CharacterStatUtils.GetReputationRank(cur));
        }
        public void SetMood(float moodValue)
        {
            m_MoodTmp.text = CharacterStatUtils.GetMoodRank(moodValue);
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
            if (HasAnyActiveTournament()) return;

            UIManager.ShowFrame(GameConstants.FRAME_ID_SHOP);
        }

        private void ShowTrainingMenu()
        {
            if (HasAnyActiveTournament()) return;

            TooltipController.Hide();
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_TRAINING_MENU);

            TrainingController.HideMainMenuBG();
            TrainingController.OpenTrainingMenuBG();
        }

        private void PerformRest()
        {
            if (HasAnyActiveTournament()) return;

            TooltipController.Hide();
            TrainingController.StartTraining(ETrainingType.Rest);
            LoadCharacterStats();
        }

        private void ShowCalendar()
        {
            TooltipController.Hide();
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_CALENDAR);
        }
        private void ShowDeck()
        {
            if (HasAnyActiveTournament()) return;

            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_DECK);
        }
        private void ShowPlayerProfile()
        {
            if (HasAnyActiveTournament()) return;

            TooltipController.Hide();
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_PLAYER_PROFILE);
        }

        private void ShowInventory()
        {
            if (HasAnyActiveTournament()) return;

            UIManager.ShowFrame(GameConstants.FRAME_ID_INVENTORY);
        }
        #endregion

        private void TestShowCombat()
        {
            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_COMBAT);
        }
        private void TestTournament()
        {
            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TOURNAMENT);
        }

        private bool HasAnyActiveTournament()
        {
            List<TournamentProgressData> tournaments = TournamentDAO.GetAllBySessionId(PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID));

            return tournaments.Count > 0;
        }
    }
}