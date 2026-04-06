namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;

    public class CardFactory
    {
        /// <summary>
        /// Create a runtime-instance of a card.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseCard Create(CardSO data)
        {
            if (data == null)
            {
                return null;
            }
            BaseCard card;

            switch (data.CardId)
            {
                case CardConstants.CARD_ID_0007:
                case CardConstants.CARD_ID_0012:
                case CardConstants.CARD_ID_0047:
                    return new ExtraDmgWithEffectConditionCard(data);

                case CardConstants.CARD_ID_0008:
                    return new PurifyCard(data);

                case CardConstants.CARD_ID_0023:
                    return new BerserkerSlashCard(data);

                case CardConstants.CARD_ID_0040:
                    return new QuietCalculationCard(data);

                case CardConstants.CARD_ID_0050:
                    return new NullifyCard(data);

                case CardConstants.CARD_ID_0055:
                    return new EpidemicCard(data);

                case CardConstants.CARD_ID_0069:
                    return new FuriosoCard(data);
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