namespace SEP490G69.Authentication
{
    using SEP490G69.Shared;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UISendForgotPWEmailFrame : GameUIFrame
    {
        [SerializeField] private TMP_InputField m_EmailInput;
        [SerializeField] private Button m_ChangePWBtn;
        [SerializeField] private Button m_BackBtn;

        private GameAuthManager _authManager;

        private GameAuthManager AuthManager
        {
            get
            {
                if (_authManager == null)
                {
                    _authManager = ContextManager.Singleton.ResolveGameContext<GameAuthManager>();
                }
                return _authManager;
            }
        }

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
            string email = m_EmailInput.text;

            AuthManager.SendPasswordResetEmail(email, () => {
                UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                         .AsFrame<UIMessagePopup>()
                         .SetContent("title_noti", "msg_cancel_reset_pw", true, false);
            }, () => {
                UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                        .AsFrame<UIMessagePopup>()
                        .SetContent("title_noti", " ", true, false);
            }, () => {
                UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                         .AsFrame<UIMessagePopup>()
                         .SetContent("title_noti", "msg_reset_pw_success", true, false);
            });

            UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                     .AsFrame<UIMessagePopup>()
                     .SetContent("title_noti", "msg_reset_pw_email_sent", true, false);
        }

        private void Back()
        {
            UIManager.HideFrame(GameConstants.FRAME_ID_CHANGE_PW);
        }
    }
}