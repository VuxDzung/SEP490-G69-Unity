namespace SEP490G69.Exploration
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIExplorationFrame : GameUIFrame
    {
        [SerializeField] private Button m_PrevBtn;
        [SerializeField] private Button m_NextBtn;
        [SerializeField] private Button m_BackBtn;

        [SerializeField] private TextMeshProUGUI m_LocatationNameTmp;
        [SerializeField] private TextMeshProUGUI m_DifficultyTmp;
        [SerializeField] private TextMeshProUGUI m_FinalBossNameTmp;

        [SerializeField] private Transform m_RewardContainer;
        [SerializeField] private Transform m_RewardPrefab;

        [SerializeField] private Button m_StartExploreBtn;

        private int _currentLocationIndex;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (m_BackBtn) m_BackBtn.onClick.AddListener(Back);
            if (m_PrevBtn) m_PrevBtn.onClick.AddListener(ShowPrevLocation);
            if (m_NextBtn) m_NextBtn.onClick.AddListener(ShowNextLocation);
            if (m_StartExploreBtn) m_StartExploreBtn.onClick.AddListener(StartExplore);    
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            if (m_BackBtn) m_BackBtn.onClick.RemoveListener(Back);
            if (m_PrevBtn) m_PrevBtn.onClick.RemoveListener(ShowPrevLocation);
            if (m_NextBtn) m_NextBtn.onClick.RemoveListener(ShowNextLocation);
            if (m_StartExploreBtn) m_StartExploreBtn.onClick.RemoveListener(StartExplore);
        }

        /// <summary>
        /// Close this frame or go back to Main Menu scene.
        /// Use UIManager.HideFrame(frameId:string) to close frame
        /// Use SceneLoader.Singleton.StartLoadScene(sceneName:string) to go to other scene.
        /// </summary>
        private void Back()
        {

        }

        private void ShowPrevLocation()
        {
            _currentLocationIndex--;
            if (_currentLocationIndex < 0)
            {

            }

            // Call ShowLocation(locationId:string) here.

        }

        private void ShowNextLocation()
        {
            _currentLocationIndex++;
            
            // Call ShowLocation(locationId:string) here.

        }

        private void ShowLocation(string locationId)
        {
            // Load Exploration Scriptable Object data here.

            // Display location name here.

            // Display location difficulty here.

            // Display final boss name here.

            // Load possbile reward here.

        }

        /// <summary>
        /// Trigger exploration logic.
        /// </summary>
        private void StartExplore()
        {

        }

        private void ShowPossibleRewardDetails(string rewardId)
        {
            
        }
    }
}