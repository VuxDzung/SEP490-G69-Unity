namespace SEP490G69
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIRegisterFrame : GameUIFrame
    {
        [SerializeField] private TMP_InputField emailTmp;
        [SerializeField] private TMP_InputField passwordTmp;
        [SerializeField] private TMP_InputField confirmPWTmp;
        [SerializeField] private Button registerBtn;
        [SerializeField] private Button loginNavBtn;

        private GameAuthManager _authManager;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (_authManager == null) _authManager = ContextManager.Singleton.ResolveGameContext<GameAuthManager>();
            registerBtn.onClick.AddListener(OnRegisterClicked);
            loginNavBtn.onClick.AddListener(BackToLoginNav);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            registerBtn.onClick.RemoveListener(OnRegisterClicked);
            loginNavBtn.onClick.RemoveListener(BackToLoginNav);
            emailTmp.text = string.Empty;
            passwordTmp.text = string.Empty;
            confirmPWTmp.text = string.Empty;
        }

        public async void OnRegisterClicked()
        {
            string email = emailTmp.text;
            string password = passwordTmp.text;
            string confirmPW = confirmPWTmp.text;

            if (string.IsNullOrEmpty(email))
            {
                // Error here.
                return;
            }

            if (confirmPW.Equals(password))
            {
                LoadingHandler.Singleton.Show().SetText("Signing up...");
                bool success = await _authManager.RegisterAsync(email, password);
                LoadingHandler.Singleton.Hide();

                if (success)
                {
                    // Set player name.
                    UIManager.ShowFrame(GameConstants.FRAME_ID_SET_NAME);
                }
                else
                {
                    // Error here.
                }
            }
            else
            {
                // Error here.
            }
        }

        public void BackToLoginNav()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_LOGIN);
        }
    }
}