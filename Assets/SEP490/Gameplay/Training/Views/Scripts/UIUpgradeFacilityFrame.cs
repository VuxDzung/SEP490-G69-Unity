namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using System.Linq;
    using SEP490G69.Shared;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIUpgradeFacilityFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private TextMeshProUGUI m_RemainGoldTmp;

        [SerializeField] private Transform m_ExerciseUIPrefab;
        [SerializeField] private Transform m_FacilityUIContainer;
        [Header("Details")]
        [SerializeField] private Transform m_StatChangesContainer;
        [SerializeField] private TextMeshProUGUI m_ExerciseNameTmp;
        [SerializeField] private Image m_ExerciseIcon;
        [SerializeField] private TextMeshProUGUI m_ExerciseLvTmp;
        [SerializeField] private TextMeshProUGUI m_ExerciseBaseStatsTmp;
        [SerializeField] private TextMeshProUGUI m_UpgradeCostTmp;
        [SerializeField] private TextMeshProUGUI m_RequiredRankTmp;
        [SerializeField] private Transform m_StatChangesPrefab;

        [SerializeField] private Button m_UpgradeBtn;

        private string _selectedFacilityId;
        private List<TrainingExerciseDataHolder> _exerciseList = new List<TrainingExerciseDataHolder>();

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
        private ImageMasterConfigSO m_ImgMasterConfig;
        private ImageMasterConfigSO ImgMasterConfig
        {
            get
            {
                if (m_ImgMasterConfig == null)
                {
                    m_ImgMasterConfig = Resources.Load<ImageMasterConfigSO>("Images/ImageMasterConfig");
                }
                return m_ImgMasterConfig;
            }
        }
        #endregion

        protected override void OnFrameShown()
        {
            base.OnFrameShown();

            m_BackBtn.onClick.AddListener(Back);
            m_UpgradeBtn.onClick.AddListener(UpgradeSelectedFacility);
            ClearDetailsContent();
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
            UIManager.ShowFrame(GameConstants.FRAME_ID_TRAINING_MENU);
        }

        private void LoadFacilities()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            _exerciseList.Clear();
            _exerciseList = FacilityUpgradeManager.GetAllExercises(sessionId);

            foreach (TrainingExerciseDataHolder facility in _exerciseList)
            {
                Transform facilityUITrans = PoolManager.Pools["UIFacility"].Spawn(m_ExerciseUIPrefab, m_FacilityUIContainer);
                UIFacilityElement facilityUI = facilityUITrans.GetComponent<UIFacilityElement>();
                if (facilityUI != null)
                {
                    facilityUI.SetOnClickCallback(SelectFacility)
                              .SetContent(facility.GetRawId(), 
                              LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_EXERCISE_NAMES, 
                                                      ExerciseConfig.GetExercise(facility.GetRawId()).ExerciseName),
                              facility.GetLevel());
                }
            }
        }

        private void ClearDetailsContent()
        {
            m_ExerciseNameTmp.text = string.Empty;
            m_ExerciseIcon.sprite = null;
            m_ExerciseIcon.enabled = false;
            m_ExerciseLvTmp.text = string.Empty;
            m_ExerciseBaseStatsTmp.text = string.Empty;
            m_UpgradeCostTmp.text = string.Empty;
            m_RequiredRankTmp.text = string.Empty;
        }

        public void SelectFacility(string facilityId)
        {
            Debug.Log($"<color=green>[UIUpgradeFacilityFrame]</color> Select facility {facilityId}");
            _selectedFacilityId = facilityId;

            if (string.IsNullOrEmpty(_selectedFacilityId))
            {
                return;
            }
            TrainingExerciseDataHolder exercise = _exerciseList.FirstOrDefault(ex => ex.GetRawId() == _selectedFacilityId);
            if (exercise != null)
            {
                m_ExerciseNameTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_EXERCISE_NAMES, exercise.GetName());
                m_ExerciseIcon.sprite = exercise.GetImage();

                bool isMaxLevel = exercise.GetLevel() >= GameConstants.FACIILITY_MAX_LV;

                int currentLevel = exercise.GetLevel();
                int nextLevel = isMaxLevel ? exercise.GetLevel() + 1 : exercise.GetLevel();
                m_ExerciseLvTmp.text = isMaxLevel == true ? "Max" : $"{currentLevel} -> {nextLevel}";
                m_UpgradeCostTmp.text = isMaxLevel == false ? GameConstants.GetUpgradeFacilityCost(nextLevel).ToString() : "Max";
                m_RequiredRankTmp.text = isMaxLevel == false ? FacilityUpgradeManager.GetRequirementForNextLevel(currentLevel).Value.RequiredRank.ToString() : "Max";

                foreach (var statChanges in exercise.GetSuccessRewards())
                {
                    Transform statChangesUITrans = PoolManager.Pools["UIStatChanges"].Spawn(m_StatChangesPrefab, m_StatChangesContainer);
                    UIFacilityStatChangesElement statChangesUI = statChangesUITrans.GetComponent<UIFacilityStatChangesElement>();   

                    if (statChangesUI != null)
                    {
                        float current = statChanges.Modifier.Value;
                        float next = current + statChanges.BonusPerLevel * exercise.GetLevel();
                        string iconId = GameConstants.GetStatIconId(statChanges.Modifier.StatType);
                        ImageData iconData = ImgMasterConfig.GetImage("stat_icons", iconId);
                        statChangesUI.SetContent(iconData?.image, current, next, isMaxLevel);
                    }
                }
            }
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

            string resultStrId = string.Empty;
            string title = string.Empty;
            switch(result)
            {
                case EUpgradeResult.Success:
                    title = "title_success";
                    resultStrId = "msg_upgrade_success";
                    break;
                case EUpgradeResult.MaxLevel:
                    title = "title_failed";
                    resultStrId = "msg_upgrade_max_level";
                    break;
                case EUpgradeResult.NotEnoughGold:
                    title = "title_failed";
                    resultStrId = "msg_not_enough_gold";
                    break;
                case EUpgradeResult.RankTooLow:
                    title = "title_failed";
                    resultStrId = "msg_rank_too_low";
                    break;
                case EUpgradeResult.Error:
                    title = "title_error";
                    resultStrId = "msg_upgrade_error";
                    break;
                default:
                    Debug.LogError($"[UIUpgradeFacilityFrame error] Unsupported upgrade result {result.ToString()}");
                    break;
            }
            if (!string.IsNullOrEmpty(resultStrId))
            {
                UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                         .AsFrame<UIMessagePopup>()
                         .SetContent(title, resultStrId, true, false);
            }
        }
    }
}