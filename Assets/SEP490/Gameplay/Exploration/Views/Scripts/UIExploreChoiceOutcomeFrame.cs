namespace SEP490G69.Exploration
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Economy;
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

    public class UIExploreChoiceOutcomeFrame : GameUIFrame
    {
        [SerializeField] private TextMeshProUGUI m_OutcomeTmp;
        [SerializeField] private TextMeshProUGUI m_PrepareForCombatTmp;
        [SerializeField] private Transform m_RewardUIPrefab;
        [SerializeField] private Transform m_CardRewardPrefab;
        [SerializeField] private Transform m_PenaltyUIPrefab;
        [SerializeField] private Transform m_ContentContainer;

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

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            ClearPenalties();
            ClearRewards();
        }

        public UIExploreChoiceOutcomeFrame LoadPenalties(IReadOnlyList<StatusModifierSO> modifierList)
        {
            ClearPenalties();
            foreach (var penalty in modifierList)
            {
                Transform rewardUITrans = PoolManager.Pools["UIPenalty"].Spawn(m_PenaltyUIPrefab, m_ContentContainer);
                UIOutcomeResultElement rewardUI = rewardUITrans.GetComponent<UIOutcomeResultElement>();
                if (rewardUI != null)
                {
                    string rewardName = penalty.StatType.ToString();
                    string iconId =  GameConstants.GetStatIconId(penalty.StatType);
                    ImageData iconData = ImgMasterConfig.GetImage("stat_icons", iconId);

                    int amount = 0;

                    if (penalty.Value > 0 && penalty.Value < 1)
                    {
                        amount = (int)(penalty.Value * 100f);
                    }

                    rewardUI.SetContent(rewardName, iconData != null ? iconData.image : null, amount);
                }
            }
            return this;
        }

        public UIExploreChoiceOutcomeFrame LoadRewards(IReadOnlyList<RewardDataSO> rewardList, Dictionary<ERewardType, string[]> extraRewards)
        {
            ClearRewards();
            foreach (var reward in rewardList)
            {
                Transform rewardUITrans = PoolManager.Pools["UIReward"].Spawn(reward.RewardType == ERewardType.Card ? 
                                                                              m_CardRewardPrefab : m_RewardUIPrefab, m_ContentContainer);
                UIOutcomeResultElement rewardUI = rewardUITrans.GetComponent<UIOutcomeResultElement>();
                if (rewardUI != null)
                {
                    string rewardName = DetermineRewardName(reward.RewardType, reward.RewardTargetId);
                    Sprite icon = GetIcon(reward.RewardType, reward.RewardTargetId);
                    rewardUI.SetContent(rewardName, icon, reward.RewardAmount);
                }
            }

            if (extraRewards != null && extraRewards.Count > 0)
            {
                foreach (var rewardPool in extraRewards)
                {
                    if (rewardPool.Value != null && rewardPool.Value.Length > 0)
                    {
                        foreach (var rewardID in rewardPool.Value)
                        {
                            Transform rewardUITrans = PoolManager.Pools["UIReward"].Spawn(rewardPool.Key == ERewardType.Card ?
                                                                                          m_CardRewardPrefab : m_RewardUIPrefab, m_ContentContainer);
                            UIOutcomeResultElement rewardUI = rewardUITrans.GetComponent<UIOutcomeResultElement>();
                            if (rewardUI != null)
                            {
                                string rewardName = DetermineRewardName(rewardPool.Key, rewardID);
                                Sprite icon = GetIcon(rewardPool.Key, rewardID);
                                rewardUI.SetContent(rewardName, icon, 1);
                            }
                        }
                    }
                }
            }

            return this;
        }

        public UIExploreChoiceOutcomeFrame ShowOutcomeMessage(string message)
        {
            if (m_OutcomeTmp != null)
            {
                m_OutcomeTmp.text = message;
            }
            return this;
        }

        public UIExploreChoiceOutcomeFrame ShowPendingCombat(float delayTime)
        {
            m_PrepareForCombatTmp.text = $"Prepare for combat in {Math.Round(delayTime, 0)}";
            return this;
        }

        private void ClearPenalties()
        {
            if (PoolManager.Pools["UIPenalty"].Count > 0)
            {
                PoolManager.Pools["UIPenalty"].DespawnAll();
            }
        }
        private void ClearRewards()
        {
            if (PoolManager.Pools["UIReward"].Count > 0)
            {
                PoolManager.Pools["UIReward"].DespawnAll();
            }
        }

        private string DetermineRewardName(ERewardType type, string id)
        {
            return type switch
            {
                ERewardType.Gold => "Gold",
                ERewardType.ReputationPoint => "RP",
                ERewardType.Item => LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_ITEM_NAMES, id + "_name"),
                ERewardType.Card => LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, id + "_name"),
                _ => string.Empty
            };
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
    }
}