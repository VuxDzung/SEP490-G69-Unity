namespace SEP490G69.Tournament
{
    using SEP490G69.Addons.Localization;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITournamentBracketScreen : GameUIFrame
    {
        [Header("Tournament Name")]
        [SerializeField] private TextMeshProUGUI m_TmpTournamentName;

        [Header("Elimination Slots (Starting 8)")]
        [SerializeField] private Image[] m_EliminationSlots = new Image[8]; // Order: L1,L2,... R1,R2,..

        [Header("Quarter Final Slots (Winners 4)")]
        [SerializeField] private Image[] m_QuarterSlots = new Image[4]; // Gắn 4 Image: QL1, QL2, QR1, QR2

        [Header("Final Slots (Winners 2)")]
        [SerializeField] private Image[] m_FinalSlots = new Image[2]; // Gắn 2 Image: FinalL, FinalR

        [Header("Champion")]
        [SerializeField] private Image m_ChampionSlot;

        [Header("Controls")]
        [SerializeField] private Button m_BtnContinue; // Nút để ấn Next Match

        private GameTournamentController _controller;

        #region Properties (Lazy getters)
        // ... (Giữ nguyên LocalizeManager)
        #endregion

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BtnContinue.onClick.AddListener(OnContinueClicked);
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BtnContinue.onClick.RemoveListener(OnContinueClicked);
        }

        // Khởi tạo ban đầu
        public void SetupTournament(string tournamentNameKey, List<TournamentParticipant> participants, GameTournamentController controller)
        {
            _controller = controller;

            m_TmpTournamentName.text =
                LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_TOUR_NAMES, tournamentNameKey);

            ClearSlots(m_QuarterSlots);
            ClearSlots(m_FinalSlots);
            ClearChampion();
            Debug.Log("ELIMINATION_ROUND");
            // Setup 8 participants
            for (int i = 0; i < m_EliminationSlots.Length; i++)
            {
                if (i < participants.Count)
                {
                    Debug.Log($"Participant: {participants[i].Name}. Slot: {i}");
                    m_EliminationSlots[i].sprite = participants[i].Avatar;
                    m_EliminationSlots[i].color = Color.white;
                }
                else
                {
                    m_EliminationSlots[i].sprite = null;
                    m_EliminationSlots[i].color = new Color(1, 1, 1, 0);
                }
            }
        }

        // ==========================================
        // ROUND UPDATES
        // ==========================================

        public void UpdateQuarterFinals(List<TournamentParticipant> winners)
        {
            //for (int i = 0; i < m_QuarterSlots.Length; i++)
            //{
            //    if (i < winners.Count)
            //    {
            //        m_QuarterSlots[i].sprite = winners[i].Avatar;
            //        m_QuarterSlots[i].color = Color.white;
            //    }
            //}
            if (winners.Count < 4) return;
            Debug.Log("QUATER_FINAL_ROUND");

            foreach (var participant in winners)
            {
                Debug.Log($"Participant: {participant.Name}");
            }

            // Left side
            SetSlot(m_QuarterSlots[0], winners[0]);
            SetSlot(m_QuarterSlots[1], winners[1]);

            // Right side
            SetSlot(m_QuarterSlots[2], winners[2]);
            SetSlot(m_QuarterSlots[3], winners[3]);
        }

        private void SetSlot(Image slot, TournamentParticipant participant)
        {
            slot.sprite = participant.Avatar;
            slot.color = Color.white;
        }

        public void UpdateSemiFinals(List<TournamentParticipant> winners)
        {
            //for (int i = 0; i < m_FinalSlots.Length; i++)
            //{
            //    if (i < winners.Count)
            //    {
            //        m_FinalSlots[i].sprite = winners[i].Avatar;
            //        m_FinalSlots[i].color = Color.white;
            //    }
            //}
            if (winners.Count < 2) return;

            SetSlot(m_FinalSlots[0], winners[0]); // Left semi winner
            SetSlot(m_FinalSlots[1], winners[1]); // Right semi winner
        }

        public void UpdateFinal(List<TournamentParticipant> winners)
        {
            if (winners.Count > 0)
            {
                m_ChampionSlot.sprite = winners[0].Avatar;
                m_ChampionSlot.color = Color.white;
            }
        }

        // ==========================================
        // CHAMPION
        // ==========================================

        public void ShowChampion(TournamentParticipant champion)
        {
            m_ChampionSlot.sprite = champion.Avatar;
            m_ChampionSlot.color = Color.white;

            m_BtnContinue.interactable = false;
        }

        // ==========================================
        // HELPERS
        // ==========================================

        private void ClearSlots(Image[] slots)
        {
            foreach (var img in slots)
            {
                img.sprite = null;
                img.color = new Color(1, 1, 1, 0);
            }
        }

        private void ClearChampion()
        {
            m_ChampionSlot.sprite = null;
            m_ChampionSlot.color = new Color(1, 1, 1, 0);
        }

        private void OnContinueClicked()
        {
            if (_controller != null)
            {
                _controller.RequestProgressTournament();
            }
            else
            {
                Debug.LogError("Tournament controller is null");
            }
        }
    }
}