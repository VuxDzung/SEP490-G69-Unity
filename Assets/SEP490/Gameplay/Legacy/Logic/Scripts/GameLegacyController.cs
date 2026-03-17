namespace SEP490G69.Legacy
{
    using UnityEngine;

    public class GameLegacyController : MonoBehaviour, IGameContext
    {
        private ContextManager _contextManager;

        private GameLegacyDAO _legacyDAO;

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;
        }

        private void Awake()
        {
            _legacyDAO = new GameLegacyDAO();
        }
    }
}