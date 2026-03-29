namespace SEP490G69.Exploration
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Economy;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static UnityEditor.FilePathAttribute;

    public class UIExplorationHomeFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private Button m_PrevBtn;
        [SerializeField] private Button m_NextBtn;


        [SerializeField] private TextMeshProUGUI m_LocatationNameTmp;
        [SerializeField] private TextMeshProUGUI m_DifficultyTmp;
        [SerializeField] private TextMeshProUGUI m_FinalBossNameTmp;

        [SerializeField] private Transform m_RewardContainer;
        [SerializeField] private Transform m_CardRewardPrefab;
        [SerializeField] private Transform m_RewardPrefab;

        [SerializeField] private Button m_StartExploreBtn;

        private string _selectedLocationId;

        private IReadOnlyList<ExploreLocationDataHolder> locations = new List<ExploreLocationDataHolder>();

        private int _currentLocationIndex;
        private CharacterConfigSO _characterConfig;
        private CharacterConfigSO CharacterConfig
        {
            get
            {
                if (_characterConfig == null)
                {
                    _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
                }
                return _characterConfig;
            }
        }

        private ImageMasterConfigSO m_ImgMasterConfig;

        private ItemDataConfigSO _itemConfig;
        private ItemDataConfigSO ItemConfig
        {
            get
            {
                if (_itemConfig == null)
                {
                    _itemConfig = ContextManager.Singleton.GetDataSO<ItemDataConfigSO>();
                }
                return _itemConfig;
            }
        }

        private CardConfigSO _cardConfig;
        private CardConfigSO CardConfig
        {
            get
            {
                if (_cardConfig == null)
                {
                    _cardConfig = ContextManager.Singleton.GetDataSO<CardConfigSO>();
                }
                return _cardConfig;
            }
        }

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

        #region Lazy initialization
        private GameExploreController _exploreController;
        protected GameExploreController ExploreController
        {
            get
            {
                if (_exploreController == null)
                {
                    ContextManager.Singleton.TryResolveSceneContext(out _exploreController);
                }
                return _exploreController;
            }
        }
        private ExplorationConfigSO _explorationConfig;
        private ExplorationConfigSO ExploreConfig
        {
            get
            {
                if (_explorationConfig == null)
                    _explorationConfig = ContextManager.Singleton.GetDataSO<ExplorationConfigSO>();
                return _explorationConfig;
            }
        }
        #endregion

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            _currentLocationIndex = 0;

            if (m_BackBtn) m_BackBtn.onClick.AddListener(Back);
            if (m_PrevBtn) m_PrevBtn.onClick.AddListener(ShowPrevLocation);
            if (m_NextBtn) m_NextBtn.onClick.AddListener(ShowNextLocation);
            if (m_StartExploreBtn) m_StartExploreBtn.onClick.AddListener(StartExplore);

            locations = ExploreController.GetAllLocations();

            ShowLocation(_currentLocationIndex);
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
            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_MAIN_MENU);
        }

        private void ShowPrevLocation()
        {
            _currentLocationIndex--;
            if (_currentLocationIndex < 0)
            {
                _currentLocationIndex = 0;
            }

            ShowLocation(_currentLocationIndex);
        }

        private void ShowNextLocation()
        {
            _currentLocationIndex++;
            if (_currentLocationIndex > locations.Count - 1)
            {
                _currentLocationIndex = locations.Count - 1;
            }

            ShowLocation(_currentLocationIndex);
        }

        private void ShowLocation(int index)
        {
            ExploreLocationDataHolder location = locations[index];

            if (location == null)
            {
                m_LocatationNameTmp.text = string.Empty;
                m_DifficultyTmp.text = string.Empty;
                m_FinalBossNameTmp.text = string.Empty;
                return;
            }

            _selectedLocationId = location.GetEntityId();

            m_LocatationNameTmp.text = !string.IsNullOrEmpty(location.GetLocationName()) ?
                                       LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_EXPLORE_LOCATIONS, location.GetLocationName()) :
                                       string.Empty;

            m_DifficultyTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, GameConstants.ConvertDifficulty2LocalizeId(location.GetDifficulty()));

            BaseCharacterSO enemySO = CharacterConfig.GetCharacterById(location.GetBossId());
            m_FinalBossNameTmp.text = enemySO != null ? enemySO.CharacterName : string.Empty;

            // Load rewards.
            ClearPossibleRewards();
            if (location != null && location.GetBossEvent() != null)
            {
                foreach (var reward in location.GetBossEvent().Choices[0].Outcomes[0].Rewards)
                {
                    Transform rewardUITrans = PoolManager.Pools["PossibleRewards"].Spawn(reward.RewardType == ERewardType.Card ? m_CardRewardPrefab : m_RewardPrefab, m_RewardContainer);
                    UIPossibleRewardElement rewardUI = rewardUITrans.GetComponent<UIPossibleRewardElement>();
                    if (rewardUI != null)
                    {
                        rewardUI.SetOnClickCallback(ShowPossibleRewardDetails)
                                .SetContent(reward.RewardTargetId, reward.RewardType, GetIcon(reward.RewardType, reward.RewardTargetId));
                    }
                }
            }
        }

        /// <summary>
        /// Trigger exploration logic.
        /// </summary>
        private void StartExplore()
        {
            ExploreController.StartExplore(_selectedLocationId);
        }

        private void ShowPossibleRewardDetails(ERewardType rewardType, string rewardId)
        {
            
        }

        private Sprite GetIcon(ERewardType type, string id)
        {
            return type switch
            {
                ERewardType.Gold => ImgMasterConfig.GetImage("general_icons", "ic_coin")?.image,
                ERewardType.ReputationPoint => null,
                ERewardType.Item => ItemConfig.GetItemById(id)?.ItemImage,
                ERewardType.Card => CardConfig.GetCardById(id)?.Icon,
                _ => null
            };
        }

        private void ClearPossibleRewards()
        {
            if (PoolManager.Pools["PossibleRewards"].Count > 0)
            {
                PoolManager.Pools["PossibleRewards"].DespawnAll();
            }
        }
    }
}