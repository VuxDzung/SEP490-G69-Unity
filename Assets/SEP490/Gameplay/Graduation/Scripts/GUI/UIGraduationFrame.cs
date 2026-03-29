namespace SEP490G69.Graduation
{
    using SEP490G69.Addons.LoadScreenSystem;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIGraduationFrame : GameUIFrame
    {
        [SerializeField] private TextMeshProUGUI m_RankTmp;
        [SerializeField] private TextMeshProUGUI m_CharTitleTmp;
        [SerializeField] private TextMeshProUGUI m_ObtainedCardsTmp;
        [SerializeField] private TextMeshProUGUI m_RelicObtainedTmp;
        [SerializeField] private TextMeshProUGUI m_TournamentWinCountTmp;
        [SerializeField] private TextMeshProUGUI m_LPGainedTmp;
        [SerializeField] private Button m_CompleteBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_CompleteBtn.onClick.AddListener(Complete);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_CompleteBtn.onClick.RemoveListener(Complete);
        }

        public UIGraduationFrame SetRank(string rank)
        {
            m_RankTmp.text = rank;
            return this;
        }

        public UIGraduationFrame SetCharacterTitle(string title)
        {
            m_CharTitleTmp.text = title;
            return this;
        }

        public UIGraduationFrame SetCardCount(int cardCount)
        {
            m_ObtainedCardsTmp.text = cardCount.ToString();
            return this;
        }

        public UIGraduationFrame SetTournamentWinCount(int count)
        {
            m_TournamentWinCountTmp.text = count.ToString();
            return this;
        }

        public UIGraduationFrame SetLPGained(int lp)
        {
            m_LPGainedTmp.text = lp.ToString();
            return this;
        }

        public UIGraduationFrame SetRelicCount(int count)
        {
            m_RelicObtainedTmp.text = count.ToString();
            return this;
        }

        private void Complete()
        {
            FadingController.Singleton.FadeIn2Out(1f, 1f, () =>
            {
                SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TITLE);
            });
        }
    }
}