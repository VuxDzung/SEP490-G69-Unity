namespace SEP490G69
{
    using SEP490G69.GameSessions;
    using UnityEngine;

    public class SceneCharacterLoader : MonoBehaviour, ISceneContext
    {
        [SerializeField] private bool m_AutoLoad = false;
        [SerializeField] private Transform[] m_Containers;

        private Transform _characterTrans;

        private GameSessionDAO _sessionDAO;
        private PlayerCharacterRepository _characterRepo;

        private CharacterConfigSO _characterConfig;
        private CharacterConfigSO CharacterConfig
        {
            get
            {
                if (_characterConfig == null)
                {
                    _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
                }
                return _characterConfig;
            }
        }

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
            LoadDAOs();
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        private void Start()
        {
            if (m_AutoLoad == true) AutoLoadCharacter();
        }

        private void LoadDAOs()
        {
            _sessionDAO = new GameSessionDAO();
            _characterRepo = new PlayerCharacterRepository();
        }

        private void AutoLoadCharacter()
        {
            string _sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            PlayerTrainingSession sessionData = _sessionDAO.GetById(_sessionId);

            if (sessionData == null)
            {
                Debug.LogError("[GameExploreController.LoadCharacter fatal error] Session data is null");
                return;
            }
            if (string.IsNullOrEmpty(sessionData.RawCharacterId))
            {
                Debug.LogError("[GameExploreController.LoadCharacter fatal error] Raw character id in session data is empty.");
                return;
            }

            BaseCharacterSO characterSO = CharacterConfig.GetCharacterById(sessionData.RawCharacterId);

            if (characterSO == null)
            {
                Debug.LogError("[GameExploreController.LoadCharacter fatal error] CharacterSO is null!");
                return;
            }

            LoadPlayerCharacter(characterSO);
        }

        public void LoadPlayerCharacter(BaseCharacterSO characterSO)
        {
            _characterTrans = PoolManager.Pools["Character"].Spawn(characterSO.Prefab, m_Containers.Length > 0 ? m_Containers[0] : this.transform);
        }

        public void DespawnCharacter()
        {
            if (_characterTrans != null)
            {
                PoolManager.Pools["Character"].DespawnObject(_characterTrans);
                _characterTrans = null;
            }
        }
    }
}