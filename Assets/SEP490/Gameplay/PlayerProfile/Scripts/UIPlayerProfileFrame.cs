namespace SEP490G69.PlayerProfile
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPlayerProfileFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;

        [SerializeField] private TextMeshProUGUI m_PlayerNameTmp;
        [SerializeField] private TextMeshProUGUI m_PlayerId;
        [SerializeField] private TextMeshProUGUI m_Email;
        [SerializeField] private TextMeshProUGUI m_GenderTmp;
        [SerializeField] private TextMeshProUGUI m_TotalPlayTime;

        [SerializeField] private Button m_SyncBtn;
        [SerializeField] private Button m_SignOutBtn;
        [SerializeField] private Button m_EditNameBtn;

        [SerializeField] private string m_PrevFrame;

        private PlayerProfileController _profileController;
        private PlayerProfileController ProfileController
        {
            get
            {
                if (_profileController == null)
                {
                    _profileController = ContextManager.Singleton.ResolveGameContext<PlayerProfileController>();
                }
                return _profileController;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_SyncBtn.onClick.AddListener(Sync);
            m_SignOutBtn.onClick.AddListener(SignOut);
            m_EditNameBtn.onClick.AddListener(EditPlayerName);
            m_BackBtn.onClick.AddListener(Back);

            LoadPlayerInfo();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_SyncBtn.onClick.RemoveListener(Sync);
            m_SignOutBtn.onClick.RemoveListener(SignOut);
            m_EditNameBtn.onClick.RemoveListener(EditPlayerName);
            m_BackBtn.onClick.RemoveListener(Back);
        }

        public void LoadPlayerInfo()
        {
            string playerId = PlayerPrefs.GetString(GameConstants.PREF_KEY_PLAYER_ID);
            m_PlayerId.text = playerId;
            m_PlayerNameTmp.text = ProfileController.GetPlayerName(playerId);
            m_Email.text = ProfileController.GetPlayerEmail(playerId);
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
            if (!string.IsNullOrEmpty(m_PrevFrame)) UIManager.ShowFrame(m_PrevFrame);
        }

        private void Sync()
        {

        }

        private void SignOut()
        {

        }

        private void EditPlayerName()
        {

        }
    }
}