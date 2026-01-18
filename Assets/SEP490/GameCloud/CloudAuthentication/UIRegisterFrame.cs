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

        public void OnRegisterClicked()
        {

        }

        public void BackToLoginNav()
        {
            UIManager.ShowFrame("Frame.Login");
        }
    }
}