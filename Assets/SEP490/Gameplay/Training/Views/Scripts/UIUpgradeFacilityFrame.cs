namespace SEP490G69.Training
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIUpgradeFacilityFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private TextMeshProUGUI m_RemainGoldTmp;

        [SerializeField] private Transform m_ExerciseUIPrefab;
        [SerializeField] private Transform m_FacilityUIContainer;
        [SerializeField] private TextMeshProUGUI m_ExerciseNameTmp;
        [SerializeField] private Image m_ExerciseIcon;
        [SerializeField] private TextMeshProUGUI m_ExerciseLvTmp;
        [SerializeField] private TextMeshProUGUI m_ExerciseBaseStatsTmp;
        [SerializeField] private TextMeshProUGUI m_UpgradeCostTmp;
        [SerializeField] private TextMeshProUGUI m_RequiredRankTmp;

        [SerializeField] private Button m_UpgradeBtn;

        private string _selectedFacilityId;

        #region Lazy properties
        private FacilityUpgradeManager _facilityManager;
        protected FacilityUpgradeManager FacilityUpgradeManager
        {
            get
            {
                if (_facilityManager == null)
                    ContextManager.Singleton.TryResolveSceneContext(out  _facilityManager);
                return _facilityManager;
            }
        }
        private TrainingExerciseConfigSO _exercisesConfig;
        private TrainingExerciseConfigSO ExerciseConfig
        {
            get
            {
                if (_exercisesConfig == null)
                {
                    _exercisesConfig = ContextManager.Singleton.GetDataSO<TrainingExerciseConfigSO>();
                }
                return _exercisesConfig;
            }
        }
        #endregion

        protected override void OnFrameShown()
        {
            base.OnFrameShown();

            m_BackBtn.onClick.AddListener(Back);
            m_UpgradeBtn.onClick.AddListener(UpgradeSelectedFacility);

            LoadFacilities();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BackBtn.onClick.RemoveListener(Back);
            m_UpgradeBtn.onClick.RemoveListener(UpgradeSelectedFacility);

        }

        private void Back()
        {
            HideThisView();
        }

        private void LoadFacilities()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            foreach (var facility in FacilityUpgradeManager.GetAllExercises(sessionId))
            {
                Transform facilityUITrans = PoolManager.Pools["UIFacility"].Spawn(m_ExerciseUIPrefab, m_FacilityUIContainer);
                UIFacilityElement facilityUI = facilityUITrans.GetComponent<UIFacilityElement>();
                if (facilityUI != null)
                {
                    facilityUI.SetOnClickCallback(SelectFacility)
                              .SetContent(facility.ExerciseId, 
                              LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_EXERCISE_NAMES, 
                                                      ExerciseConfig.GetExercise(facility.ExerciseId).ExerciseName),
                              facility.Level);
                }
            }
        }

        public void SelectFacility(string facilityId)
        {
            Debug.Log($"<color=green>[UIUpgradeFacilityFrame]</color> Select facility {facilityId}");
            _selectedFacilityId = facilityId;
        }

        private void UpgradeSelectedFacility()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError($"[UIUpgradeFacilityFrame error] Session id is empty");
                return;
            }
            if (string.IsNullOrEmpty(_selectedFacilityId))
            {
                Debug.LogError($"[UIUpgradeFacilityFrame error] Selected facility raw id is empty");
                return;
            }
            EUpgradeResult result = FacilityUpgradeManager.TryUpgradeFacility(sessionId, _selectedFacilityId);

        }
    }
}