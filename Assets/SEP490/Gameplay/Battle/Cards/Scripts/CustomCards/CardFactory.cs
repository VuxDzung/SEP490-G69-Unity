namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using UnityEngine;
    public static class CardFactory
    {
        public const string CARD_12_ID = "card_0012";

        public static BaseCard Create(CardSO data)
        {
            BaseCard card;

            if (data.ActionType == EActionType.Attack)
            {
                card = new BaseAttackCard(data);

                switch (data.CardId)
                {
                    case CARD_12_ID:
                        card = new RecklessChargeCard(data);
                        break;
                }
            }
            else
            {
                card = new BaseCard(data);
            }
            return card;
        }
    }
}