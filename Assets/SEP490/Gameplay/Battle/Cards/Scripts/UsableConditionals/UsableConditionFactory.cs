namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;

    public static class UsableConditionFactory
    {
        private static readonly Dictionary<string, IUsableCondition> _conditionals = new Dictionary<string, IUsableCondition>
        {
            { "play_nth_cards_in_one_turn", new UseCardCountInTurnCondition() },
        };

        public static IUsableCondition GetById(string conditionId)
        {
            if (_conditionals.ContainsKey(conditionId))
            {
                return _conditionals[conditionId];
            }
            return null;
        }
    }
}