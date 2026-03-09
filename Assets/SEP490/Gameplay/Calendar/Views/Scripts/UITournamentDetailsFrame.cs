namespace SEP490G69.Calendar
{
    using SEP490G69.Tournament;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Addons.Localization;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using SEP490G69.Addons.LoadScreenSystem;

    public class UITournamentDetailsFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private Button m_EnterBtn;

        [SerializeField] private TextMeshProUGUI m_TournamentNameTmp;
        [SerializeField] private TextMeshProUGUI m_RequiredRankTmp;
        [SerializeField] private TextMeshProUGUI m_GoldRewardTmp;
        [SerializeField] private TextMeshProUGUI m_RPRewardTmp;

        [Header("Rewards Setup")]
        [SerializeField] private Transform m_RewardContainer;
        [SerializeField] private Transform m_RewardPreviewPrefab;

        private string _currentTournamentId;
        private GameCalendarController _calendarController;
        protected GameCalendarController CalendarController
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
            m_BackBtn.onClick.AddListener(Back);
            m_EnterBtn.onClick.AddListener(Enter);
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BackBtn.onClick.RemoveListener(Back);
            m_EnterBtn.onClick.RemoveListener(Enter);
        }

        public void LoadTournamentData(TournamentSO tournamentSO)
        {
            if (tournamentSO == null) return;

            _currentTournamentId = tournamentSO.TournamentId;

            // 1. Tên giải đấu
            m_TournamentNameTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_TOUR_NAMES, tournamentSO.Name);

            // 2. Rank yêu cầu
            string requiredRank = "None";
            if (tournamentSO.EntryConditions != null && tournamentSO.EntryConditions.Count > 0)
            {
                // Dùng logic tương tự màn Calendar để bóc Rank
                TournamentConditionSO condition = tournamentSO.EntryConditions[0];
                ConditionParamData paramData = condition.GetParamByStatType(EStatusType.RP);
                if (paramData != null)
                {
                    requiredRank = CharacterStatUtils.GetReputationRank((int)paramData.RequiredValue);
                }
            }
            m_RequiredRankTmp.text = requiredRank;

            // 3. Load Phần Thưởng (Vàng, RP, Item, Card)
            LoadRewards(tournamentSO);
        }

        private void LoadRewards(TournamentSO tournamentSO)
        {
            int goldGain = 0;
            int rpGain = 0;

            // Clear các slot cũ trong Pool (Nếu bạn dùng key khác cho Pool, hãy đổi tên "UIRewardSlotElement")
            string poolKey = "UIRewardSlotElement";
            if (!PoolManager.Pools[poolKey].IsEmpty)
            {
                PoolManager.Pools[poolKey].DespawnAll();
            }

            // Lấy phần thưởng của Top 1 (Rank == 1)
            if (tournamentSO.RewardRanks != null && tournamentSO.RewardRanks.Count > 0)
            {
                RewardRankData topReward = tournamentSO.RewardRanks[0];

                foreach (RewardDataSO rewardSO in topReward.Rewards)
                {
                    if (rewardSO == null) continue;

                    // Cộng dồn tài nguyên cơ bản
                    if (rewardSO.RewardType == ERewardType.Gold)
                    {
                        goldGain += rewardSO.RewardAmount;
                    }
                    else if (rewardSO.RewardType == ERewardType.ReputationPoint)
                    {
                        rpGain += rewardSO.RewardAmount;
                    }
                    // Load Object (Card, Relic) vào ScrollView
                    else if (rewardSO.RewardType == ERewardType.Card || rewardSO.RewardType == ERewardType.Item)
                    {
                        Transform elementTrans = PoolManager.Pools[poolKey].Spawn(m_RewardPreviewPrefab, m_RewardContainer);
                        UITournamentRewardElement uiElement = elementTrans.GetComponent<UITournamentRewardElement>();

                        if (uiElement != null)
                        {
                            string itemName = rewardSO.RewardName;
                            string itemType = rewardSO.RewardType.ToString();
                            Sprite itemIcon = null;

                            // Bóc tách thông tin thẻ bài
                            if (rewardSO.RewardType == ERewardType.Card)
                            {
                                CardConfigSO cardConfig = ContextManager.Singleton.GetDataSO<CardConfigSO>();
                                if (cardConfig != null)
                                {
                                    CardSO cardData = cardConfig.GetCardById(rewardSO.RewardTargetId);
                                    if (cardData != null)
                                    {
                                        itemName = cardData.CardName; // Lấy tên thật của thẻ
                                        itemIcon = cardData.Icon;     // Lấy icon của thẻ
                                    }
                                }
                            }
                            else if (rewardSO.RewardType == ERewardType.Item)
                            {
                                // TODO: Hiện tại chưa có ItemConfigSO và RelicSO, tạm thời bỏ qua phần gán Icon.
                                // Khi nào làm xong Relic, bạn get Config như CardConfig ở trên.
                                // ItemConfigSO itemConfig = ContextManager.Singleton.GetDataSO<ItemConfigSO>();
                                // ItemSO itemData = itemConfig.GetItemById(rewardSO.RewardId);
                                // if (itemData != null) { itemName = itemData.ItemName; itemIcon = itemData.Icon; }
                            }

                            // Gán dữ liệu cho UI Element
                            uiElement.SetIdAndType(rewardSO.Id, rewardSO.RewardType)
                                     .SetOnClickDetails(ViewRewardDetails)
                                     .SetContent(itemName, itemType, itemIcon);
                        }
                    }
                }
            }

            // Hiển thị tổng Gold và RP ra Text
            m_GoldRewardTmp.text = $"{goldGain} g";
            m_RPRewardTmp.text = $"{rpGain} RP";
        }

        private void ViewRewardDetails(string rewardId, ERewardType type)
        {
            // Xử lý khi bấm nút Detail trên vật phẩm
            Debug.Log($"Clicked Detail for {type} with ID: {rewardId}");

            if (type == ERewardType.Card)
            {
                // Gọi popup hiển thị full Card
                // CardSO cardSO = ContextManager.Singleton.GetDataSO<CardConfigSO>().GetCardById(rewardId);
                // UIManager.ShowFrame("FRAME_CARD_DETAIL").AsFrame<UICardDetailFrame>().LoadData(cardSO);
            }
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
        }

        private void Enter()
        {
            if (CalendarController.TryEnterTournament(_currentTournamentId, out string result))
            {
                UIManager.HideFrame(FrameId);
                SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TOURNAMENT);
            }
            else
            {
                Debug.Log($"Result: {result}");
            }
        }
    }
}