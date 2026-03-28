namespace SEP490G69.Training
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIFacilityStatChangesElement : MonoBehaviour, IPooledObject
    {
        [SerializeField] private Image m_Image;
        [SerializeField] private TextMeshProUGUI m_StatChangesTmp;

        public void Spawn()
        {
            
        }
        public void Despawn()
        {
            m_Image.sprite = null;
            m_StatChangesTmp.text = string.Empty;
        }

        public UIFacilityStatChangesElement SetContent(Sprite statIcon, float current, float next, bool isMax)
        {
            m_Image.sprite = statIcon;
            if (!isMax)
            {
                m_StatChangesTmp.text = $"{current} - {next}";
            }
            else
            {
                m_StatChangesTmp.text = "Max";
            }
            return this;
        }
    }
}