namespace SEP490G69.Training
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [System.Serializable]
    public class UIStatModifier : MonoBehaviour
    {
        [Tooltip("The root game object of this stat modifier (e.g., VitModify)")]
        [SerializeField] private GameObject m_RootObj;

        [SerializeField] private TextMeshProUGUI m_StatsBeforeTmp;
        [SerializeField] private TextMeshProUGUI m_StatsAfterTmp;
        [SerializeField] private TextMeshProUGUI m_StatsModifiedTmp;

        /// <summary>
        /// Set values for the stat modifier. If there is no change, the root object will be disabled.
        /// </summary>
        public void SetValue(float before, float after)
        {
            if (Mathf.Approximately(before, after))
            {
                m_RootObj.SetActive(false);
                return;
            }

            m_RootObj.SetActive(true);
            m_StatsBeforeTmp.text = before.ToString("F0");
            m_StatsAfterTmp.text = after.ToString("F0");

            float diff = after - before;
            if (diff > 0)
            {
                m_StatsModifiedTmp.text = $"+{diff:F0}";
                m_StatsModifiedTmp.color = Color.green;
            }
            else
            {
                m_StatsModifiedTmp.text = diff.ToString("F0");
                m_StatsModifiedTmp.color = Color.red;
            }
        }
    }
}