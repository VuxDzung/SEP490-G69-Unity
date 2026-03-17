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

            //m_BoxingBtn.onClick.AddListener(PerformBoxing);
            //m_RunBtn.onClick.AddListener(PerformRun);
            //m_DodgeBtn.onClick.AddListener(PerformDodge);
            //m_StudyBtn.onClick.AddListener(PerformStudy);
            //m_YogaBtn.onClick.AddListener(PerformYoga);

            LoadStats();
            LoadExercisesUI();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BackBtn.onClick.RemoveListener(Back);
            m_UpgradeFacilityBtn.onClick.RemoveListener(UpgradeFacilityNav);

            ClearAllExercisesUI();

            //m_BoxingBtn.onClick.RemoveListener(PerformBoxing);
            //m_RunBtn.onClick.RemoveListener(PerformRun);
            //m_DodgeBtn.onClick.RemoveListener(PerformDodge);
            //m_StudyBtn.onClick.RemoveListener(PerformStudy);
            //m_YogaBtn.onClick.RemoveListener(PerformYoga);
        }

        private void LoadExercisesUI()
        {
            ITrainingStrategy[] strategies = TrainingController.GetAllTrainings();

            if (m_ExercisesUI.Count != strategies.Length)
            {
                return;
            }

            for (int i = 0; i < strategies.Length; i++)
            {
                ITrainingStrategy strategy = strategies[i];
                m_ExercisesUI[i].SetOnClick(PerformExercise)
                                .SetContent(strategy.DataHolder.GetId(), strategy.DataHolder.GetImage(),
                                strategy.DataHolder.GetName(), strategy.DataHolder.GetLevel());
                m_ExercisesUI[i].Spawn();
            }

            //foreach (ITrainingStrategy strategy in TrainingController.GetAllTrainings())
            //{
            //    if (!strategy.DataHolder.CanShowOnUI()) continue;

            //    Transform exerciseUITrans = PoolManager.Pools[poolName].Spawn(m_ExerciseUIPrefab, m_Container);
            //    UIExerciseElement exerciseUI = exerciseUITrans.GetComponent<UIExerciseElement>();
            //    if (exerciseUI != null)
            //    {
            //        exerciseUI.SetOnClick(PerformExercise)
            //                  .SetContent(strategy.DataHolder.GetId(), strategy.DataHolder.GetImage(), 
            //                  strategy.DataHolder.GetName(), strategy.DataHolder.GetLevel());
            //    }
            //}
        }

        private void ClearAllExercisesUI()
        {
            //string poolName = "UIExercise";
            //if (PoolManager.Pools[poolName].Count > 0)
            //{
            //    PoolManager.Pools[poolName].DespawnAll();
            //}
            m_ExercisesUI.ForEach(exUI => exUI.Despawn());
        }

        private void LoadStats()
        {
            if (TrainingController == null)
            {
                return;
            }

            SetEnergy(TrainingController.CharacterData.GetEnergy(), GameConstants.MAX_100);
            SetFailureRate(TrainingController.GetFailRate());

            // Stats
            SetVitality(TrainingController.CharacterData.GetVIT(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetVIT()));
            SetPower(TrainingController.CharacterData.GetPower(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetPower()));
            SetINT(TrainingController.CharacterData.GetINT(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetINT()));
            SetAgility(TrainingController.CharacterData.GetAgi(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetAgi()));
            SetStamina(TrainingController.CharacterData.GetStamina(), CharacterStatUtils.GetStatRankMaxValue(TrainingController.CharacterData.GetStamina()));
        }

        public void SetEnergy(float cur, float max)
        {
            m_EnergySlider.SetValue(cur, max);
        }
        public void SetMood(string mood)
        {
            m_MoodTmp.text = string.Format(GameConstants.MOOD_FORMAT, mood);
        }

        public void SetFailureRate(float rate)
        {
            m_FailRateTmp.text = string.Format(FORMAT_FAIL_RATE, rate);
        }

        public void SetVitality(float cur, float max)
        {
            m_HealthSlider.SetValue(cur, max);
            m_HealthSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetPower(float cur, float max)
        {
            m_PowerSlider.SetValue(cur, max);
            m_PowerSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
        }
        public void SetAgility(float cur, float max)
        {
            m_AgilitySlider.SetValue(cur, max);
            m_AgilitySlider.SetRank(CharacterStatUtils.GetStatRank(cur));
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

        public void Back()
        {
            TrainingController.OpenMainMenuBG();
            TrainingController.HideTrainingMenuBG();

            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
        }

        private void UpgradeFacilityNav()
        {

        }

        public void PerformBoxing()
        {
            if (TrainingController.CanJoinTraining())
                TrainingController.StartTraining(ETrainingType.Boxing);
        }
        private void PerformRun()
        {
            if (TrainingController.CanJoinTraining())
                TrainingController.StartTraining(ETrainingType.Run);
        }
        private void PerformDodge()
        {
            if (TrainingController.CanJoinTraining())
                TrainingController.StartTraining(ETrainingType.Dodge);
        }
        private void PerformStudy()
        {
            if (TrainingController.CanJoinTraining())
                TrainingController.StartTraining(ETrainingType.Study);
        }
        private void PerformYoga()
        {
            if (TrainingController.CanJoinTraining())
                TrainingController.StartTraining(ETrainingType.Yoga);
        }

        private void PerformExercise(string id)
        {
            if (TrainingController.CanJoinTraining())
            {
                TrainingController.StartTraining(id);
                LoadStats();
            }
        }
    }
}