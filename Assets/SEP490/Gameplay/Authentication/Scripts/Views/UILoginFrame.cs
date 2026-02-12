namespace SEP490G69
{
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
            _authManager.OnLoginByGGWindowsChanged += OnLoginByGGWindowsChanged;
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            loginBtn.onClick.RemoveListener(OnLoginClicked);
            ggLoginBtn.onClick.RemoveListener(OnGoogleLoginClicked);
            registerNavBtn.onClick.RemoveListener(Go2Register);
            forgotPWBtn.onClick.RemoveListener(ForgotPWNav);
            loginAsGuestBtn.onClick.RemoveListener(LoginAsGuest);
            _authManager.OnLoginByGGWindowsChanged -= OnLoginByGGWindowsChanged;
        }


        public async void OnLoginClicked()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_LOADING);
            bool success = await _authManager.LoginAsync(emailInput.text, passwordInput.text);
            UIManager.HideFrame(GameConstants.FRAME_ID_LOADING);

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

        }

        public async void OnGoogleLoginClicked()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_LOADING);
#if UNITY_ANDROID
            bool success = await _authManager.SignInWithGoogleAndroid();
            UIManager.HideFrame(GameConstants.FRAME_ID_LOADING);

            if (success)
            {
                OnLoginSuccess();
            }
            else
            {
                
            }
#else
            _authManager.SignInByGoogleWindows();
#endif
        }

        private void OnLoginByGGWindowsChanged(string result)
        {
            UIManager.HideFrame(GameConstants.FRAME_ID_LOADING);
            if (result.Equals("failed"))
            {

            }
            else
            {
                OnLoginSuccess();
            }
        }

        private void OnLoginSuccess()
        {
            // SceneManager.LoadScene("MainMenu");
            Debug.Log("[UI] Login success");
        }
        private void Go2Register()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_REGISTER);
        }
    }
}