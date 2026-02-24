namespace SEP490G69.Training
{
    using SEP490G69.GameSessions;
    using UnityEngine;

    public class GameTrainingController : MonoBehaviour, ISceneContext
    {
        private GameSessionDAO _sessionDAO;
        private PlayerCharacterRepository _characterRepo;

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
            _sessionDAO = new GameSessionDAO(LocalDBInitiator.GetDatabase());
            _characterRepo = new PlayerCharacterRepository(LocalDBInitiator.GetDatabase());

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }
            PlayerTrainingSession sessionData = _sessionDAO.GetSession(sessionId);

            if (sessionData == null)
            {
                return;
            }
            SessionCharacterData characterData = _characterRepo.GetCharacterData(sessionId, sessionData.CharacterId);

            Debug.Log($"Character: {characterData.Id}");
        }
    }
}