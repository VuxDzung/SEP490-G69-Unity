namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;

    public class UseCardCountInTurnCondition : IUsableCondition
    {
        public bool IsCardUsable(CardSO card, SceneCombatController sceneController)
        {
            int cardCount = sceneController.Player.CardsService.GetPlayedCardInTurn();
            var varRequiredCardCount = card.GetVariableByName("required_card_count");
            return cardCount >= varRequiredCardCount.GetValue<int>();
        }
    }
}