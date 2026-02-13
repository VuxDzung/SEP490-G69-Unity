namespace SEP490G69.Shared
{
    using TMPro;
    using UnityEngine;

    public class UILoadingScreen : GameUIFrame
    {
        [SerializeField] private TextMeshProUGUI m_LoadingTmp;

        public void SetText(string text)
        {
            m_LoadingTmp.text = text;
        }
    }
}