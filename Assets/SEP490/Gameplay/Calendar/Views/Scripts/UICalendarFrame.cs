namespace SEP490G69.Calendar
{
    using SEP490G69.Tournament;
    using SEP490G69.Training;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICalendarFrame : GameUIFrame
    {
        [SerializeField] private Button m_PrevBtn;
        [SerializeField] private Button m_NextBtn;
        [SerializeField] private TextMeshProUGUI m_CurrentTimeTmp;
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private Transform m_TournamentContainer;
        [SerializeField] private Transform m_TournamentUIPrefab;
        [SerializeField] private GameObject m_NoTournamentGO;

        private int _currentWeek;

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

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_PrevBtn.onClick.AddListener(Prev);
            m_NextBtn.onClick.AddListener(Next);
            m_BackBtn.onClick.AddListener(Back);
            LoadCalendarTime();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_PrevBtn.onClick.RemoveListener(Prev);
            m_NextBtn.onClick.RemoveListener(Next);
            m_BackBtn.onClick.RemoveListener(Back);
        }

        public void LoadCalendarTime()
        {
            _currentWeek = CalendarController.GetCurrentWeekInt();
            if (_currentWeek < 0)
            {
                return;
            }
            DisplayPreviewWeek(_currentWeek);
        }

        private void Prev()
        {
            _currentWeek--;
            if (_currentWeek < 0)
            {
                _currentWeek = 0;
            }
            DisplayPreviewWeek(_currentWeek);
        }
        private void Next()
        {
            _currentWeek++;
            if (_currentWeek > CalendarController.GetTotalWeeks() - 1)
            {
                _currentWeek = CalendarController.GetTotalWeeks() - 1;
            }
            DisplayPreviewWeek(_currentWeek);
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
        }

        private void DisplayPreviewWeek(int week)
        {
            CalendarWeekSO weekSO = CalendarController.GetWeekData(week + 1);
            m_CurrentTimeTmp.text = CalendarController.GetCalendarTime(week);

            // 1. Dọn dẹp các khung thông tin Giải đấu cũ
            if (!PoolManager.Pools["UITournament"].IsEmpty)
            {
                PoolManager.Pools["UITournament"].DespawnAll();
            }

            // 2. Dọn dẹp các khung Preview Phần thưởng (Reward) cũ
            if (PoolManager.Pools.ContainsKey("UIRewardPreview") && !PoolManager.Pools["UIRewardPreview"].IsEmpty)
            {
                PoolManager.Pools["UIRewardPreview"].DespawnAll();
            }

            if (weekSO.Tournaments.Count > 0)
            {
                m_NoTournamentGO.SetActive(false);
                foreach (TournamentSO data in weekSO.Tournaments)
                {
                    Transform tournamentUITrans = PoolManager.Pools["UITournament"].Spawn(m_TournamentUIPrefab, m_TournamentContainer);
                    UITournamentElement tournamentUI = tournamentUITrans.GetComponent<UITournamentElement>();
                    if (tournamentUI == null) continue;

                    string tournamentName = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_TOUR_NAMES, data.Name);

                    string requiredRank = "";

                    if (data.EntryConditions.Count > 0)
                    {
                        TournamentConditionSO condition = data.EntryConditions[0];
                        ConditionParamData paramData = condition.GetParamByStatType(EStatusType.RP);

                        if (paramData != null)
                        {
                            requiredRank = CharacterStatUtils.GetReputationRank((int)paramData.RequiredValue);
                        }
                    }

                    IReadOnlyList<RewardDataSO> rewards = data.ChampionRewards.Count > 0 ? data.ChampionRewards : null;

                    // ========================================================
                    // XÁC ĐỊNH ĐÂY CÓ PHẢI LÀ GIẢI ĐẤU OBJECTIVE (MANDATORY)
                    // (Bạn hãy thay đổi data.Objective != null hoặc data.IsMandatory 
                    // cho khớp với thiết lập trong file TournamentSO.cs của bạn)
                    // ========================================================
                    bool isObjectiveTournament = false;
                    isObjectiveTournament = data.IsCheckpointTournament; 


                    tournamentUI.SetOnClickDetails(ViewTournamentDetails)
                                .SetId(data.TournamentId)
                                .SetContent(tournamentName, requiredRank, rewards, isObjectiveTournament); // <-- Truyền isObjective vào đây
                }
            }
            else
            {
                m_NoTournamentGO.SetActive(true);
            }
        }

        private void ViewTournamentDetails(string tournamentId)
        {
            TournamentSO tournamentSO = CalendarController.TournamentConfig.GetTournamentById(tournamentId);

            if (tournamentSO != null)
            {
                UIManager.ShowFrame(GameConstants.FRAME_ID_TOURNAMENT_DETAILS).AsFrame<UITournamentDetailsFrame>().LoadTournamentData(tournamentSO);
            }
            else
            {
                Debug.LogError($"Tournament with ID {tournamentId} not found in TournamentConfig.");
            }
        }
    }
}