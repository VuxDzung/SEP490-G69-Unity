namespace SEP490G69.Battle.Cards
{
    using UnityEngine;

    public class DeckController : MonoBehaviour, IGameContext
    {
        private ContextManager _contextManager;

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;
        }
    }
}