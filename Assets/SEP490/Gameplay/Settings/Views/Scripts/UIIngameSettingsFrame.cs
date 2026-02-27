namespace SEP490G69.Training
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.GameSessions;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIIngameSettingsFrame : UITitleSettingsFrame
    {
        [SerializeField] private Button m_BackToMenuBtn;
        [SerializeField] private Button m_SaveGameBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BackToMenuBtn.onClick.AddListener(BackToMenu);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BackToMenuBtn.onClick.RemoveListener(BackToMenu);
        }

        private void BackToMenu()
        {
            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TITLE);
        }
    }
}