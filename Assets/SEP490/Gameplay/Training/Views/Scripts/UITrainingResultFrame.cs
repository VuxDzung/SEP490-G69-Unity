namespace SEP490G69.Training
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITrainingResultFrame : GameUIFrame
    {
        [SerializeField] private TextMeshProUGUI m_ResultTmp;
        [SerializeField] private Button m_CloseBtn;


        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_CloseBtn.onClick.AddListener(Close);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_CloseBtn.onClick.RemoveListener(Close);
        }

        public void SetResult(string result)
        {
            m_ResultTmp.text = result;
        }

        private void Close()
        {
            UIManager.HideFrame(FrameId);
        }
    }
}