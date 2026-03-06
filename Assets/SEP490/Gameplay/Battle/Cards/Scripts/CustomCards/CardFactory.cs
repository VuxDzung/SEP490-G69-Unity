namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using UnityEngine;

    public class CardFactory
    {
        public static BaseCard Create(CardSO data)
        {
            BaseCard card;

            switch (data.CardId)
            {
                case CardConstants.CARD_ID_12:
                    return new RecklessChargeCard(data);
                case CardConstants.CARD_ID_16:
                    return new ExecuteCard(data);
                case CardConstants.CARD_ID_20:
                    return new LastResortCard(data);
                case CardConstants.CARD_ID_25:
                    return new NatureWrathCard(data);
                case CardConstants.CARD_ID_31:
                    return new GuardianImpact(data);
                case CardConstants.CARD_ID_32:
                    return new PurifyCard(data);
                case CardConstants.CARD_ID_41:
                    return new CursedWhisperCard(data);
                case CardConstants.CARD_ID_46:
                    return new NullifyCard(data);
            }

            if (data.ActionType == EActionType.Attack)
            {
                card = new BaseAttackCard(data);
            }
            else
            {
                card = new BaseCard(data);
            }
            return card;
        }
    }
}