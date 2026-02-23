namespace SEP490G69.GameSessions
{
    using UnityEngine;

    public class GameSessionController : MonoBehaviour, ISceneContext
    {
        private IGameSessionCreator _sessionCreator;

        private void OnEnable()
        {
            ContextManager.Singleton.AddSceneContext(this);
        }
        private void OnDisable()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        private void Start()
        {
            _sessionCreator = new SingleSessionCreator();
        }

        public void CreateNewSession(string characterId)
        {
            if (_sessionCreator == null) return;

            string playerId = "";

            if (_sessionCreator.TryCreateSession(playerId, characterId, out string error))
            {
                // Create session success.
            }
            else
            {
                // Display error to UI.
            }
        }

        public void ContinueSession()
        {

        }
    }
}