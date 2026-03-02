namespace SEP490G69.Training
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITrainingResultFrame : GameUIFrame
    {
        [Header("General UI")]
        [SerializeField] private TextMeshProUGUI m_TitleTmp;
        [SerializeField] private Button m_CloseBtn;

        [Header("Stat Modifiers")]
        [SerializeField] private UIStatModifier m_VitModifier;
        [SerializeField] private UIStatModifier m_PowModifier;
        [SerializeField] private UIStatModifier m_AgiModifier;
        [SerializeField] private UIStatModifier m_IntModifier;
        [SerializeField] private UIStatModifier m_StaModifier;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_CloseBtn.onClick.AddListener(CloseFrame);
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_CloseBtn.onClick.RemoveListener(CloseFrame);
        }

        #region Setters
        public void SetTitle(string title)
        {
            m_TitleTmp.text = title;
        }

        public void SetVitModifier(float before, float after)
        {
            m_VitModifier.SetValue(before, after);
        }

        public void SetPowModifier(float before, float after)
        {
            m_PowModifier.SetValue(before, after);
        }

        public void SetAgiModifier(float before, float after)
        {
            m_AgiModifier.SetValue(before, after);
        }

        public void SetIntModifier(float before, float after)
        {
            m_IntModifier.SetValue(before, after);
        }

        public void SetStaModifier(float before, float after)
        {
            m_StaModifier.SetValue(before, after);
        }
        #endregion

        #region Actions
        private void CloseFrame()
        {
            UIManager.HideFrame(FrameId);
        }
        #endregion
    }
}