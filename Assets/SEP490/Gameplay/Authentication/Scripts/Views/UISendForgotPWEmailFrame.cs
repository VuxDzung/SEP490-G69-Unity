namespace SEP490G69.Authentication
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UISendForgotPWEmailFrame : GameUIFrame
    {
        [SerializeField] private TMP_InputField m_EmailInput;
        [SerializeField] private Button m_ChangePWBtn;
        [SerializeField] private Button m_BackBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_ChangePWBtn.onClick.AddListener(SendEmail);
            m_BackBtn.onClick.AddListener(Back);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_ChangePWBtn.onClick.RemoveListener(SendEmail);
            m_BackBtn.onClick.RemoveListener(Back);
        }

        private void SendEmail()
        {

        }
        private void Back()
        {
            UIManager.HideFrame(GameConstants.FRAME_ID_CHANGE_PW);
        }
    }
}