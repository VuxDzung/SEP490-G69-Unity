namespace SEP490G69.Battle.Cards
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIStatusTooltipElement : MonoBehaviour, IPooledObject
    {
        [SerializeField] private Image m_StatusIcon;
        [SerializeField] private TextMeshProUGUI m_StatusNameTmp;
        [SerializeField] private TextMeshProUGUI m_StatusDescTmp;
        public void Spawn() { }
        public void Despawn() { }
        public void Setup(StatusEffectSO statusEffect)
        {
            if (statusEffect == null) return;

            if (m_StatusIcon != null) m_StatusIcon.sprite = statusEffect.Icon;
            if (m_StatusNameTmp != null) m_StatusNameTmp.text = statusEffect.EffectName;
            if (m_StatusDescTmp != null) m_StatusDescTmp.text = statusEffect.EffectDesc;
        }
    }
}