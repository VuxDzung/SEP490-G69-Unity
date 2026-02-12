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
        [SerializeField] private TMP_Text messageText;

        [Header("Buttons")]
        [SerializeField] private Button loginBtn;
        [SerializeField] private Button registerNavBtn;
        [SerializeField] private Button ggLoginBtn;

        private GameAuthManager _authManager;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (_authManager == null) _authManager = ContextManager.Singleton.ResolveGameContext<GameAuthManager>();
            loginBtn.onClick.AddListener(OnLoginClicked);
            ggLoginBtn.onClick.AddListener(OnGoogleLoginClicked);
            registerNavBtn.onClick.AddListener(Go2Register);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            loginBtn.onClick.RemoveListener(OnLoginClicked);
            ggLoginBtn.onClick.RemoveListener(OnGoogleLoginClicked);
            registerNavBtn.onClick.RemoveListener(Go2Register);
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

        public async void OnGoogleLoginClicked()
        {
            //messageText.text = "Google login...";
            UIManager.ShowFrame(GameConstants.FRAME_ID_LOADING);
            bool success = await _authManager.SignInWithGoogleAsync();
            UIManager.HideFrame(GameConstants.FRAME_ID_LOADING);

            if (success)
            {

            }
            else
            {

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