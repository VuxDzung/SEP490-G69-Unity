namespace SEP490G69.Exploration
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIOutcomeResultElement : MonoBehaviour, IPooledObject
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_MessageTmp;
        [SerializeField] private TextMeshProUGUI m_AmountTmp;

        public void Spawn()
        {

        }
        public void Despawn()
        {
            if (m_Icon != null) m_Icon.sprite = null;
            if (m_MessageTmp != null) m_MessageTmp.text = string.Empty;
        }

        public void SetContent(string message, Sprite icon, int amount)
        {
            if (m_Icon != null) m_Icon.sprite = icon;
            if (m_MessageTmp != null) m_MessageTmp.text = message;
            if (m_AmountTmp != null) m_AmountTmp.text = amount.ToString();
        }
    }
}