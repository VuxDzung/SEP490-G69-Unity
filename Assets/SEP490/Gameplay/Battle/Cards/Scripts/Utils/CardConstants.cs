using UnityEngine;

namespace SEP490G69.Battle
{
    public class CardConstants 
    {
        #region Special cards
        /// <summary>
        /// Card 0: default rest.
        /// </summary>
        public const string CARD_ID_0000 = "card_0000";

        /// <summary>
        /// Nature's Wrath
        /// </summary>
        public const string CARD_ID_0007 = "card_0007";

        /// <summary>
        /// Purify
        /// </summary>
        public const string CARD_ID_0008 = "card_0008";

        /// <summary>
        /// Petal Counter
        /// </summary>
        public const string CARD_ID_0012 = "card_0012";

        /// <summary>
        /// Execute
        /// </summary>
        public const string CARD_ID_16 = "card_0016";

        /// <summary>
        /// Berserker Slash
        /// </summary>
        public const string CARD_ID_18 = "card_0018";

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

        /// <summary>
        /// Furioso
        /// </summary>
        public const string CARD_ID_0069 = "card_0069";
        #endregion

        #region Utils
        public static string GetCardTypeIconId(EActionType type)
        {
            string id = string.Empty;

            return type switch
            {
                EActionType.Attack => "ic_atk",
                EActionType.Effect => "ic_effect",
                EActionType.StatRecover => "ic_recover",
                _ => string.Empty,
            };
        }
        #endregion
    }
}