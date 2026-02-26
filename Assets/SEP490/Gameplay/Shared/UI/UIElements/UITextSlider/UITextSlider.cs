namespace SEP490G69
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITextSlider : MonoBehaviour
    {
        [SerializeField] private bool m_Readonly = false;
        [SerializeField] private Slider m_Slider;
        [SerializeField] private TextMeshProUGUI m_Tmp;

        private void Start()
        {
            m_Slider.interactable = m_Readonly;
        }

        public void SetValue(float cur, float max)
        {
            //m_Slider.maxValue = max;
            m_Slider.value = cur / max;
            m_Tmp.text = $"{cur}/{max}";
        }
    }
}