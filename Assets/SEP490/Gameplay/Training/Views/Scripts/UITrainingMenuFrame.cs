namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITrainingMenuFrame : GameUIFrame
    {
        public const string FORMAT_FAIL_RATE = "Failure rate: {0}%";

        [SerializeField] private Button m_BackBtn;
        [SerializeField] private Button m_UpgradeFacilityBtn;

        [Header("Stat Sliders")]
        [SerializeField] private TextMeshProUGUI m_MoodTmp;
        [SerializeField] private UITextSlider m_EnergySlider;
        [SerializeField] private UITextSlider m_HealthSlider;
        [SerializeField] private UITextSlider m_PowerSlider;
        [SerializeField] private UITextSlider m_AgilitySlider;
        [SerializeField] private UITextSlider m_INTSlider;
        [SerializeField] private UITextSlider m_StaminaSlider;

        [Header("Bottom")]
        [SerializeField] private TextMeshProUGUI m_FailRateTmp;

        [SerializeField] private List<UIExerciseElement> m_ExercisesUI;
        [SerializeField] private Transform m_ExerciseUIPrefab;
        [SerializeField] private Transform m_Container;

        [Header("UI Control")]
        [SerializeField] private CanvasGroup m_MenuCanvasGroup;

        [Header("Stats Preview UI")]
        [SerializeField] private GameObject m_StatsPreviewVitObj;
        [SerializeField] private TextMeshProUGUI m_TmpVitNumber;

        [SerializeField] private GameObject m_StatsPreviewPowObj;
        [SerializeField] private TextMeshProUGUI m_TmpPowNumber;

        [SerializeField] private GameObject m_StatsPreviewAgiObj;
        [SerializeField] private TextMeshProUGUI m_TmpAgiNumber;

        [SerializeField] private GameObject m_StatsPreviewIntObj;
        [SerializeField] private TextMeshProUGUI m_TmpIntNumber;

        [SerializeField] private GameObject m_StatsPreviewStaObj;
        [SerializeField] private TextMeshProUGUI m_TmpStaNumber;

        private string _currentPreviewId = string.Empty;

        private GameTrainingController _trainingController;
        private GameTrainingController TrainingController
        {
            get
            {
                if (_trainingController == null)
                {
                    bool exist = ContextManager.Singleton.TryResolveSceneContext(out _trainingController);
                }
                return _trainingController;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BackBtn.onClick.AddListener(Back);
            m_UpgradeFacilityBtn.onClick.AddListener(UpgradeFacilityNav);

            _currentPreviewId = string.Empty;
            HideAllPreviews();

            LoadStats();
            LoadExercisesUI();
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BackBtn.onClick.RemoveListener(Back);
            m_UpgradeFacilityBtn.onClick.RemoveListener(UpgradeFacilityNav);

            _currentPreviewId = string.Empty;
            HideAllPreviews();
            ClearAllExercisesUI();
        }

        public void HideUIForAnimation()
        {
            if (m_MenuCanvasGroup != null)
            {
                m_MenuCanvasGroup.alpha = 0f;
                m_MenuCanvasGroup.blocksRaycasts = false;
                m_MenuCanvasGroup.interactable = false;
            }
        }

        public void ShowUIAfterAnimation()
        {
            if (m_MenuCanvasGroup != null)
            {
                m_MenuCanvasGroup.alpha = 1f;
                m_MenuCanvasGroup.blocksRaycasts = true;
                m_MenuCanvasGroup.interactable = true;
            }

            // Xoá preview sau khi tập xong
            _currentPreviewId = string.Empty;
            HideAllPreviews();
        }

        private void LoadExercisesUI()
        {
            ITrainingStrategy[] strategies = TrainingController.GetAllTrainings();

            if (m_ExercisesUI.Count != strategies.Length - 1)
            {
                Debug.Log($"<color=red>[UITrainingMenuSelection.LoadExercisesUI error]</color> The UI Element count ({m_ExercisesUI.Count})  does not equals with the exercise strategies count ({strategies.Length - 1})");
                return;
            }

            for (int i = 1; i < strategies.Length; i++)
            {
                ITrainingStrategy strategy = strategies[i];
                m_ExercisesUI[i - 1].SetOnClick(PerformExercise)
                                .SetContent(strategy.DataHolder.GetRawId(), strategy.DataHolder.GetImage(),
                                strategy.DataHolder.GetName(), strategy.DataHolder.GetLevel());
                m_ExercisesUI[i - 1].Spawn();
            }
        }

        private void ClearAllExercisesUI()
        {
            m_ExercisesUI.ForEach(exUI => exUI.Despawn());
        }

        public void LoadStats()
        {
            if (TrainingController == null)
            {
                return;
            }

            SetEnergy(TrainingController.CharacterData.GetEnergy(), GameConstants.MAX_100);
            SetFailureRate(TrainingController.GetFailRate());

            SetVitality(TrainingController.CharacterData.GetVIT(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetVIT()));
            SetPower(TrainingController.CharacterData.GetPower(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetPower()));
            SetINT(TrainingController.CharacterData.GetINT(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetINT()));
            SetAgility(TrainingController.CharacterData.GetAgi(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetAgi()));
            SetStamina(TrainingController.CharacterData.GetStamina(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetStamina()));
        }

        public void SetEnergy(float cur, float max) { m_EnergySlider.SetValue(cur, max); }
        public void SetMood(string mood) { m_MoodTmp.text = string.Format(GameConstants.MOOD_FORMAT, mood); }
        public void SetFailureRate(float rate) { m_FailRateTmp.text = string.Format(FORMAT_FAIL_RATE, rate); }
        public void SetVitality(float cur, float max) { m_HealthSlider.SetValue(cur, max); m_HealthSlider.SetRank(CharacterStatUtils.GetStatRank(cur)); }
        public void SetPower(float cur, float max) { m_PowerSlider.SetValue(cur, max); m_PowerSlider.SetRank(CharacterStatUtils.GetStatRank(cur)); }
        public void SetAgility(float cur, float max) { m_AgilitySlider.SetValue(cur, max); m_AgilitySlider.SetRank(CharacterStatUtils.GetStatRank(cur)); }
        public void SetINT(float cur, float max) { m_INTSlider.SetValue(cur, max); m_INTSlider.SetRank(CharacterStatUtils.GetStatRank(cur)); }
        public void SetStamina(float cur, float max) { m_StaminaSlider.SetValue(cur, max); m_StaminaSlider.SetRank(CharacterStatUtils.GetStatRank(cur)); }

        public void Back()
        {
            TrainingController.OpenMainMenuBG();
            TrainingController.HideTrainingMenuBG();

            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
        }

        private void UpgradeFacilityNav()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_UPGRADE_FACILITY);
            UIManager.HideFrame(FrameId);
        }

        private void PerformExercise(string id)
        {
            if (!TrainingController.CanJoinTraining()) return;

            if (_currentPreviewId != id)
            {
                // Bước 1: Hiện Preview
                _currentPreviewId = id;
                ShowPreviewForExercise(id);
            }
            else
            {
                // Bước 2: Thực hiện Training
                _currentPreviewId = string.Empty;
                HideAllPreviews();
                TrainingController.StartTraining(id);
            }
        }
        private void ShowPreviewForExercise(string id)
        {
            HideAllPreviews(); // Reset state

            List<StatChange> simulatedChanges = TrainingController.GetSimulatedStatChanges(id);
            if (simulatedChanges == null) return;

            foreach (var change in simulatedChanges)
            {
                if (Mathf.Approximately(change.Delta, 0f)) continue; // Bỏ qua nếu không tăng/giảm

                string prefix = change.Delta > 0 ? "+" : "";
                string colorHex = change.Delta > 0 ? "#00FF00" : "#FF4444"; // Xanh lục nếu tăng, Đỏ nếu giảm
                string formattedText = $"<color={colorHex}><b>{prefix}{change.Delta}</b></color>";

                // Đã sửa change.StatType thành change.StatusType
                // Đã sửa VIT, POW... thành tên đầy đủ tương ứng với Enum của bạn
                switch (change.StatusType)
                {
                    case EStatusType.Vitality:
                        if (m_StatsPreviewVitObj) m_StatsPreviewVitObj.SetActive(true);
                        if (m_TmpVitNumber) m_TmpVitNumber.text = formattedText;
                        break;
                    case EStatusType.Power:
                        if (m_StatsPreviewPowObj) m_StatsPreviewPowObj.SetActive(true);
                        if (m_TmpPowNumber) m_TmpPowNumber.text = formattedText;
                        break;
                    case EStatusType.Agi:
                        if (m_StatsPreviewAgiObj) m_StatsPreviewAgiObj.SetActive(true);
                        if (m_TmpAgiNumber) m_TmpAgiNumber.text = formattedText;
                        break;
                    case EStatusType.Intelligence:
                        if (m_StatsPreviewIntObj) m_StatsPreviewIntObj.SetActive(true);
                        if (m_TmpIntNumber) m_TmpIntNumber.text = formattedText;
                        break;
                    case EStatusType.Stamina:
                        if (m_StatsPreviewStaObj) m_StatsPreviewStaObj.SetActive(true);
                        if (m_TmpStaNumber) m_TmpStaNumber.text = formattedText;
                        break;
                }
            }
        }

        private void HideAllPreviews()
        {
            if (m_StatsPreviewVitObj) m_StatsPreviewVitObj.SetActive(false);
            if (m_StatsPreviewPowObj) m_StatsPreviewPowObj.SetActive(false);
            if (m_StatsPreviewAgiObj) m_StatsPreviewAgiObj.SetActive(false);
            if (m_StatsPreviewIntObj) m_StatsPreviewIntObj.SetActive(false);
            if (m_StatsPreviewStaObj) m_StatsPreviewStaObj.SetActive(false);
        }
    }
}