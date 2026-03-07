namespace SEP490G69.Calendar
{
    using SEP490G69.Tournament;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITournamentDetailsFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;

        [SerializeField] private TextMeshProUGUI m_TournamentNameTmp;
        [SerializeField] private TextMeshProUGUI m_RequiredRankTmp;

        [SerializeField] private Transform m_RewardPreviewPrefab;
        [SerializeField] private Transform m_RewardContainer;
        [SerializeField] private Button m_EnterBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BackBtn.onClick.AddListener(Back);
            m_EnterBtn.onClick.AddListener(Enter);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BackBtn.onClick.AddListener(Back);
            m_EnterBtn.onClick.AddListener(Enter);
        }

        public void LoadTournamentData(TournamentSO tournamentSO)
        {

        }

        private void Back()
        {

        }
        private void Enter()
        {

        }
    }
}