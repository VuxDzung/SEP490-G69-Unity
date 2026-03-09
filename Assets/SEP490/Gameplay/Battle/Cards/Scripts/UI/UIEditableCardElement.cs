namespace SEP490G69.Battle.Cards
{
    using TMPro;
    using UnityEngine;

    public class UIEditableCardElement : UICardElement
    {
        [SerializeField] private TextMeshProUGUI m_AmountTmp;
        public string DeckCardId { get; private set; }
        public void SetContent(string cardId, string deckCardId, string cardName, string cardDesc, Sprite icon, int amount)
        {
            DeckCardId = deckCardId;
            SetContent(cardId, cardName, cardDesc, icon);
            if (m_AmountTmp != null)
            {
                m_AmountTmp.text = amount > 1 ?  amount.ToString() : "";
            }
        }
    }
}