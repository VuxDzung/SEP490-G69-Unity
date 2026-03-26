namespace SEP490G69
{
    using TMPro;
    using UnityEngine;

    public class UITextStatSlider : UITextSlider
    {
        [SerializeField] private TextMeshProUGUI m_ExtraValue;
        [SerializeField] private TextMeshProUGUI m_FinalValue;

        public void SetValue(float curBase, float max, float extra, float final)
        {
            SetValue(curBase, max);
            if (m_ExtraValue != null)
            {
                if (extra != 0f)
                {
                    m_ExtraValue.text = $"({(extra >= 0 ? "+" + extra.ToString() : "-" + extra.ToString())})";
                }
                else
                {
                    m_ExtraValue.text = string.Empty;
                }
            }
            if (m_FinalValue != null)
            {
                m_FinalValue.text = final.ToString();
            }
        }
    }
}