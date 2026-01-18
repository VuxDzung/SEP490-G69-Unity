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

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            loginBtn.onClick.AddListener(OnLoginClicked);
            ggLoginBtn.onClick.AddListener(OnGoogleLoginClicked);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            loginBtn.onClick.RemoveListener(OnLoginClicked);
            ggLoginBtn.onClick.RemoveListener(OnGoogleLoginClicked);
        }

        public async void OnLoginClicked()
        {
            messageText.text = "Logging in...";

            var result = await CloudAuthManager.Singleton.LoginWithEmail(emailInput.text, passwordInput.text);

            if (result.IsSuccess)
            {
                messageText.text = "Login successful!";
                OnLoginSuccess();
            }
            else
            {
                messageText.text = result.Error;
            }
        }

        public async void OnGoogleLoginClicked()
        {
            messageText.text = "Google login...";

            // TODO: Get idToken from Google SDK
            //string googleIdToken = GoogleAuthHelper.GetIdToken();

            //var result = await CloudAuthManager.Instance
            //    .LoginWithGoogle(googleIdToken);

            //messageText.text = result.IsSuccess ? "Login successful!" : result.Error;
        }

        private void OnLoginSuccess()
        {
            // SceneManager.LoadScene("MainMenu");
            Debug.Log("[UI] Login success");
        }
    }
}