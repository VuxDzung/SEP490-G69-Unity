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

        }

        public void SelectFacility(string facilityId)
        {

        }

        private void UpgradeSelectedFacility()
        {

        }
    }
}