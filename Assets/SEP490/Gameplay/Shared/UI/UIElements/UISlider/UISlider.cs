namespace SEP490G69
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class UISlider : MonoBehaviour
    {
        public UnityAction<float> onValueChanged;

        [SerializeField] private Slider m_Slider;
        [SerializeField] private TextMeshProUGUI m_ValueTmp;

        public void Enable()
        {
            m_Slider.onValueChanged.AddListener(OnSliderChanged);
        }
        public void Disable()
        {
            m_Slider.onValueChanged.RemoveListener(OnSliderChanged);
        }

        public void SetValue(float value, float max)
        {
            m_Slider.value = value / max;
            m_ValueTmp.text = Mathf.Round((value / max) * 100f).ToString() + "%";
        }

        private void OnSliderChanged(float newValue)
        {

        }
    }
}