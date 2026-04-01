namespace SEP490G69.Economy
{
    using TMPro;
    using UnityEngine;

    public class UIStatInventory : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_ValueTmp;
        [SerializeField] private Color m_DefaultColor  = Color.white;
        [SerializeField] private Color m_IncreaseColor = Color.green;
        [SerializeField] private Color m_DecreaseColor = Color.red;

        public void Enable()
        {
            gameObject.SetActive(true);
        }
        public void Disable()
        {
            gameObject.SetActive(false);
            m_ValueTmp.text = string.Empty;
        }

        public void SetFlatValue(float value)
        {
            string prefix = "";
            if (value < 0)
            {
                m_ValueTmp.color = m_DecreaseColor;
                prefix = "-";
            }
            else if (value > 0)
            {
                m_ValueTmp.color = m_IncreaseColor;
                prefix = "+";
            }
            else
            {
                m_ValueTmp.color = Color.white;
            }
            float finalValue = Mathf.Abs(value);

            m_ValueTmp.text = prefix + finalValue.ToString();
        }

        public void SetPercentValue(float value)
        {
            string prefix = "";
            if (value < 0)
            {
                m_ValueTmp.color = m_DecreaseColor;
                prefix = "-";
            }
            else if (value > 0)
            {
                m_ValueTmp.color = m_IncreaseColor;
                prefix = "+";
            }
            else
            {
                m_ValueTmp.color = Color.white;
            }
            float finalValue = Mathf.Abs(value) * 100f;
            Debug.Log($"Value: {value} - FinalValue: {finalValue}");
            m_ValueTmp.text = prefix + finalValue.ToString() + "%";
        }

        public void SetPreviewValue(float beforeValue, float afterValue)
        {
            m_ValueTmp.color = beforeValue > afterValue ? m_DecreaseColor : m_IncreaseColor;
            m_ValueTmp.text = afterValue.ToString();
        }

        public void SetValue(float value)
        {
            m_ValueTmp.color = m_DefaultColor;
            m_ValueTmp.text = value.ToString();
        }
    }
}