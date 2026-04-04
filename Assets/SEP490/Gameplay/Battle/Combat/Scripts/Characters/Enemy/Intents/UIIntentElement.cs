namespace SEP490G69.Battle.Combat
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIIntentElement : MonoBehaviour
    {
        [SerializeField] private Image m_IntentImgType;
        [SerializeField] private TextMeshProUGUI m_ContentTmp;

        public void SetContent(string content, Color textColor, Sprite typeSprite)
        {
            m_ContentTmp.color = textColor;
            m_ContentTmp.text = content;

            m_IntentImgType.enabled = typeSprite != null;
            m_IntentImgType.sprite = typeSprite;
        }
    }
}