namespace SEP490G69.Shared
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SetPlayerNameFrame : GameUIFrame
    {
        [SerializeField] private TMP_InputField m_PlayerNameInput;
        [SerializeField] private Button m_NextBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_NextBtn.onClick.AddListener(Next);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_NextBtn.onClick.RemoveListener(Next);
        }

        public void Next()
        {

        }
    }
}