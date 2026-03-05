namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using UnityEngine;

    public class CardFactory
    {
        /// <summary>
        /// Reckless Charge
        /// </summary>
        public const string CARD_ID_12 = "card_0012";

        /// <summary>
        /// Execute
        /// </summary>
        public const string CARD_ID_16 = "card_0016";


        /// <summary>
        /// Last Resort
        /// </summary>
        public const string CARD_ID_20 = "card_0020";

        /// <summary>
        /// Nature's Wrath
        /// </summary>
        public const string CARD_ID_25 = "card_0025";

        /// <summary>
        /// Guardian Impact
        /// </summary>
        public const string CARD_ID_31 = "card_0031";

        /// <summary>
        /// Purify
        /// </summary>
        public const string CARD_ID_32 = "card_0032";

        /// <summary>
        /// Cursed Whisper
        /// </summary>
        public const string CARD_ID_41 = "card_0041";

        /// <summary>
        /// Nullify
        /// </summary>
        public const string CARD_ID_46 = "card_0046";


        public static BaseCard Create(CardSO data)
        {
            BaseCard card;

            switch (data.CardId)
            {
                case CARD_ID_12:
                    return new RecklessChargeCard(data);
                case CARD_ID_16:
                    return new ExecuteCard(data);
                case CARD_ID_20:
                    return new LastResortCard(data);
                case CARD_ID_25:
                    return new NatureWrathCard(data);
                case CARD_ID_31:
                    return new GuardianImpact(data);
                case CARD_ID_32:
                    return new PurifyCard(data);
                case CARD_ID_41:
                    return new CursedWhisperCard(data);
                case CARD_ID_46:
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