namespace SEP490G69
{
    using SEP490G69.GameSessions;
    using UnityEngine;

    public class SceneCharacterLoader : MonoBehaviour
    {
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
            LoadDAOs();
        }

        private void LoadDAOs()
        {
            _sessionDAO = new GameSessionDAO();
            _characterRepo = new PlayerCharacterRepository();
        }

        public void LoadPlayerCharacter(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }

            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);

            if (sessionData == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(sessionData.RawCharacterId))
            {
                return;
            }

            BaseCharacterSO characterSO = CharacterConfig.GetCharacterById(sessionData.RawCharacterId);
            if (characterSO == null)
            {
                return;
            }

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