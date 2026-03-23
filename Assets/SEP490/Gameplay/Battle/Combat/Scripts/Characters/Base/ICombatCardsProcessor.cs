namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;

    public interface ICombatCardsProcessor
    {
        public void SetOwner(BaseBattleCharacterController owner);

        public void InitializeDeck(string[] deckCardIdArray);

        public void DrawThreeCards(out IReadOnlyList<CardSO> currentCards);

        /// <summary>
        /// Execute the selected card.
        /// </summary>
        /// <param name="opponent">Represent the owner's opponent</param>
        /// <returns>
        ///     True if the selected card is not null. 
        ///     False if the selected card is null (By default, it means that no action is selected or cannot be selected)
        /// </returns>
        public bool ExecuteCard(BaseBattleCharacterController opponent);

        public void DiscardCurrentDraw();

        public float CalculateSelectedCardDmg(bool writeToDmgOutput);
        public float CalculateBaseDmg(CardSO cardSO);

        public void SelectRest();
        public void SelectNoAction();
        public void SelectCard(CardSO cardSO);
        public void SelectCardById(string deckCardId);
        public int CalculateCardCost(CardSO cardSO);
    }
}