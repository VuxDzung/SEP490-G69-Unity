namespace SEP490G69.Battle.Cards
{
    using UnityEngine;

    /// <summary>
    /// Handle cards and deck business logic.
    /// - Load obtained cards.
    /// - Load in-deck cards.
    /// - Add card to deck.
    /// - Remove card from deck.
    /// </summary>
    public class GameDeckController : MonoBehaviour, IGameContext
    {
        private ContextManager _contextManager;

        private GameCardsDAO _cardsDAO;
        private GameDeckDAO _deckDAO;

        private void Awake()
        {
            _cardsDAO = new GameCardsDAO();
            _deckDAO = new GameDeckDAO();
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;
        }
    }
}