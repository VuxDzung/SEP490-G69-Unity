namespace SEP490G69
{
    using SEP490G69.Addons.LoadScreenSystem;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UILoginFrame : GameUIFrame
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;

        [Header("Buttons")]
        [SerializeField] private Button loginBtn;
        [SerializeField] private Button forgotPWBtn;
        [SerializeField] private Button registerNavBtn;
        [SerializeField] private Button ggLoginBtn;
        [SerializeField] private Button loginAsGuestBtn;

        private GameAuthManager _authManager;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (_authManager == null) _authManager = ContextManager.Singleton.ResolveGameContext<GameAuthManager>();
            loginBtn.onClick.AddListener(OnLoginClicked);
            ggLoginBtn.onClick.AddListener(OnGoogleLoginClicked);
            registerNavBtn.onClick.AddListener(Go2Register);
            forgotPWBtn.onClick.AddListener(ForgotPWNav);
            loginAsGuestBtn.onClick.AddListener(LoginAsGuest);
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            loginBtn.onClick.RemoveListener(OnLoginClicked);
            ggLoginBtn.onClick.RemoveListener(OnGoogleLoginClicked);
            registerNavBtn.onClick.RemoveListener(Go2Register);
            forgotPWBtn.onClick.RemoveListener(ForgotPWNav);
            loginAsGuestBtn.onClick.RemoveListener(LoginAsGuest);
        }


        public async void OnLoginClicked()
        {
            LoadingHandler.Singleton.Show().SetText("Logging in...");
            bool success = await _authManager.LoginAsync(emailInput.text, passwordInput.text);
            LoadingHandler.Singleton.Hide().SetText("Logging in...");

            if (success)
            {
                LoggerUtils.Logging("LOGIN_SUCCES");
                OnLoginSuccess();
            }
            else
            {
                LoggerUtils.Logging("LOGIN_FAILED", "", TextColor.Red);
            }
        }

        private void ForgotPWNav()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_CHANGE_PW);
        }

        private void LoginAsGuest()
        {
            if (_authManager.LoginByGuest())
            {
                OnLoginSuccess();
            }
            else
            {

            }
        }

        public void OnGoogleLoginClicked()
        {
            LoadingHandler.Singleton.Show().SetText("Logging in...");
            _authManager.SignInByGoogle();
        }

        private void OnLoginSuccess()
        {
            // SceneManager.LoadScene("MainMenu");
            Debug.Log("[UI] Login success");
            //UIManager.ShowFrame(GameConstants.FRAME_ID_SET_NAME);
            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TITLE);
        }
        private void Go2Register()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_REGISTER);
        }
    }
}