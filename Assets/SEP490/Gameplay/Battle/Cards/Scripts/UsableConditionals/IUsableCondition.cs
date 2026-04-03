namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;

    public interface IUsableCondition 
    {
        public bool IsCardUsable(CardSO card, SceneCombatController sceneController);
    }
}