namespace SEP490G69
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITextSlider : MonoBehaviour
    {
        [SerializeField] private bool m_Readonly = false;
        [SerializeField] private Slider m_Slider;
        [SerializeField] private TextMeshProUGUI m_Tmp;
        [SerializeField] private TextMeshProUGUI m_RankTmp;

        private void Start()
        {
            m_Slider.interactable = m_Readonly;
        }

        public void SetValue(float cur, float max)
        {
            //m_Slider.maxValue = max;

            float finalCur = (float)Math.Round(cur, 0);
            float finalMax = (float)Math.Round(max, 0);

            if (finalCur < 0)
            {
                finalCur = 0;
            }

            m_Slider.value = finalCur / finalMax;
            m_Tmp.text = $"{finalCur}/{finalMax}";
        }

        public void SetRank(string rank)
        {
            if (m_RankTmp != null)
            {
                m_RankTmp.text = rank;
            }
        }
    }
}