namespace SEP490G69.GameSessions
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UICreditFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BackBtn.onClick.AddListener(Back);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BackBtn.onClick.RemoveListener(Back);
        }

        private void Back()
        {
            UIManager.HideFrame(GameConstants.FRAME_ID_CREDIT);
        }
    }
}