namespace SEP490G69.Graduation
{
    using TMPro;
    using UnityEngine;

    public class UIEndGameRecordElement : MonoBehaviour, IPooledObject
    {
        /// <summary>
        /// Order by rating value
        /// </summary>
        [SerializeField] private TextMeshProUGUI m_OrderTmp;
        [SerializeField] private TextMeshProUGUI m_TitleTmp;
        [SerializeField] private TextMeshProUGUI m_EndingTypeTmp; // Just leave it there.
        [SerializeField] private TextMeshProUGUI m_RatingTmp;
        [SerializeField] private TextMeshProUGUI m_EndGameDateTmp;
        
        public void Spawn()
        {
        }
        public void Despawn()
        {
        }

        public void SetContent(string order, float ratingPoint, string title, string endingType, System.DateTime endDate)
        {
            m_OrderTmp.text = order;
            m_RatingTmp.text = ratingPoint.ToString("0.0");
            m_TitleTmp.text = title;
            m_EndingTypeTmp.text = endingType;

            // Format date (dd/MM/yyyy)
            m_EndGameDateTmp.text = endDate.ToString("dd/MM/yyyy");
        }
    }
}