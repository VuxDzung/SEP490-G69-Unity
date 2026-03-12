namespace SEP490G69.Battle.Cards
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using SEP490G69.Addons.Localization;

    public class UIStatusTooltipElement : MonoBehaviour, IPooledObject
    {
        [SerializeField] private Image m_StatusIcon;
        [SerializeField] private TextMeshProUGUI m_StatusNameTmp;
        [SerializeField] private TextMeshProUGUI m_StatusDescTmp;
        public void Spawn() { }
        public void Despawn() { }
        public void Setup(Sprite icon, string name, string desc)
        {
            if (m_StatusIcon != null) m_StatusIcon.sprite = icon;
            if (m_StatusNameTmp != null) m_StatusNameTmp.text = name;
            if (m_StatusDescTmp != null) m_StatusDescTmp.text = desc;
        }
    }
}