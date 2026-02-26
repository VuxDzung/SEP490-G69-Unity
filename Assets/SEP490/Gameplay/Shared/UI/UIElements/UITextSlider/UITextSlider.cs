namespace SEP490G69
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITextSlider : MonoBehaviour
    {
        [SerializeField] private Slider m_Slider;
        [SerializeField] private TextMeshProUGUI m_Tmp;

        public void SetValue(float cur, float max)
        {
            //m_Slider.maxValue = max;
            m_Slider.value = cur / max;
            m_Tmp.text = $"{cur}/{max}";
        }
    }
}