namespace SEP490G69
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIRegisterFrame : GameUIFrame
    {
        [SerializeField] private TextMeshProUGUI emailTmp;
        [SerializeField] private TextMeshProUGUI passwordTmp;
        [SerializeField] private TextMeshProUGUI confirmPWTmp;
        [SerializeField] private Button registerBtn;
        [SerializeField] private Button loginNavBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
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
                AuthResult result = await CloudAuthManager.Singleton.RegisterWithEmail(email, password);
                if (result.IsSuccess)
                {
                    // Go to game/menu scene.
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