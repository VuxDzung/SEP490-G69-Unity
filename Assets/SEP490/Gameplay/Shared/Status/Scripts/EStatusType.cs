namespace SEP490G69
{
    public enum EStatusType 
    {
        /// <summary>
        /// Default field.
        /// </summary>
        None = 0,

        #region Base stats
        Power = 1,
        Intelligence = 2,
        Defense = 3,
        Agi = 4,
        /// <summary>
        /// In training and combat, this is the max vitality.
        /// </summary>
        Vitality = 5,
        Energy = 6,
        Mood = 7,
        Stamina = 8,
        #endregion

        #region In-combat (runtime) stats only
        // Combat fields (They can only taken from in combat and does not exist in character model.
        ActionGauge = 9, // When a card has an effect like add/reduce action gauge, the action gauge is applied in the next character turn.
        ReceivedDmg = 10,
        RP = 11, // Reputation point.
        Damage = 12,
        ActionCost = 13, // Action cost is based on the card's cost.
        HitRate = 14,
        #endregion

        TrainingEffective = 15,
    }
}