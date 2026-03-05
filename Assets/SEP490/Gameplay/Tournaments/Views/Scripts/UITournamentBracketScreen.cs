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
        public void SetupTournament(string tournamentName, List<TournamentParticipant> participants, GameTournamentController controller)
        {
            _controller = controller;
            m_TmpTournamentName.text = tournamentName; // Hoặc dùng LocalizeManager

            // Reset toàn bộ Avatar vòng trong về trạng thái trống (Màu xám/trong suốt)
            ClearSlots(m_QuarterSlots);
            ClearSlots(m_FinalSlots);

            // Gán 8 người tham gia vào 8 ô ngoài cùng
            for (int i = 0; i < participants.Count; i++)
            {
                if (i < m_EliminationSlots.Length)
                {
                    m_EliminationSlots[i].sprite = participants[i].Avatar;
                    m_EliminationSlots[i].color = Color.white; // Hiện rõ avatar
                }
            }
        }

        // Cập nhật sau vòng loại (Đưa 4 người thắng vào ô Tứ Kết)
        public void UpdateQuarterFinals(List<TournamentParticipant> winners)
        {
            for (int i = 0; i < winners.Count; i++)
            {
                if (i < m_QuarterSlots.Length)
                {
                    m_QuarterSlots[i].sprite = winners[i].Avatar;
                    m_QuarterSlots[i].color = Color.white;
                }
            }

            // Tùy chọn: Làm tối màu/xám các ô Elimination của những người bị loại
        }

        private void ClearSlots(Image[] slots)
        {
            foreach (var img in slots)
            {
                img.sprite = null;
                img.color = new Color(1, 1, 1, 0); // Trong suốt
            }
        }

        private void OnContinueClicked()
        {
            // Báo cho Controller tính toán trận đấu tiếp theo
            if (_controller != null)
            {
                _controller.RequestProgressTournament();
            }
        }
    }
}