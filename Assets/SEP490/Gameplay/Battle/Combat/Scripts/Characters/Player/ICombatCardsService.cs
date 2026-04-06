namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;

    public interface ICombatCardsService
    {
        public void SetOwner(PlayerActorController owner);

        public void InitializeDeck(string[] deckCardIdArray);

        public void DrawCards(int amount, out IReadOnlyList<CardSO> currentCards);

        /// <summary>
        /// Execute the selected card.
        /// </summary>
        /// <param name="opponent">Represent the owner's opponent</param>
        /// <returns>
        ///     True if the selected card is not null. 
        ///     False if the selected card is null (By default, it means that no action is selected or cannot be selected)
        /// </returns>
        public bool ExecuteCard(BaseCombatActor opponent);

        public void DiscardCurrentDraw();

        public string GetFinalCardDescription(CardSO cardSO, string localizedCardDesc);
        public float CalculateSelectedCardDmg(bool writeToDmgOutput);
        public float CalculateBaseDmg(CardSO cardSO);

        public bool IsCardUsable(CardSO card, SceneCombatController sceneController);

        public void SelectRest();
        public void SelectNoAction();
        public void SelectCard(CardSO cardSO);
        public void SelectCardById(string deckCardId);
        public int CalculateCardCost(CardSO cardSO);

        public int GetPlayedCardInTurn();
        public void ResetCardInTurnCount();

        public IReadOnlyList<CardSO> GetInHandCards();
        public IReadOnlyList<CardSO> GetInDeckCards();
        public IReadOnlyList<CardSO> GetDiscardedCards();
    }
}