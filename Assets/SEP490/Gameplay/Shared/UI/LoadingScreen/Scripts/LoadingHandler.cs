namespace SEP490G69
{
    using TMPro;
    using UnityEngine;

    public class LoadingHandler : GlobalSingleton<LoadingHandler>
    {
        [SerializeField] private GameObject m_UIPanel;
        [SerializeField] private TextMeshProUGUI m_LoadingTmp;

        public LoadingHandler Show()
        {
            m_UIPanel.gameObject.SetActive(true);
            return this;
        }
        public LoadingHandler Hide()
        {
            m_UIPanel.gameObject.SetActive(false);
            m_LoadingTmp.text = string.Empty;
            return this;
        }

        public void SetText(string text)
        {
            m_LoadingTmp.text = text;
        }
    }
}