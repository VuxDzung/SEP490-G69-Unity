namespace SEP490G69.Exploration
{
    using SEP490G69.GameSessions;
    using UnityEngine;

    public class GameExploreController : MonoBehaviour, ISceneContext
    {
        [SerializeField] private SceneCharacterLoader m_CharacterLoader;
        [SerializeField] private Renderer m_BgRenderer;

        private GameSessionDAO _sessionDAO;
        private string _sessionId;

        private ExplorationConfigSO _exploreConfig;
        private ExplorationConfigSO ExploreConfig
        {
            get
            {
                if (_exploreConfig == null)
                {
                    _exploreConfig = ContextManager.Singleton.GetDataSO<ExplorationConfigSO>();
                }
                return _exploreConfig;
            }
        }

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        private void Start()
        {
            LoadDAOs();
            LoadCharacter();
        }

        private void LoadDAOs()
        {
            _sessionDAO = new GameSessionDAO();

            _sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
        }

        private void LoadCharacter()
        {
            if (m_CharacterLoader == null ||
                string.IsNullOrEmpty(_sessionId))
            {
                return;
            }
            m_CharacterLoader.LoadPlayerCharacter(_sessionId);
        }

        public void StartExplore(string exploreId = "")
        {
            if (string.IsNullOrEmpty(exploreId))
            {
                return;
            }
            ExplorationSO exploreSO = ExploreConfig.GetById(exploreId);
            if (exploreSO == null)
            {
                return;
            }
            m_BgRenderer.gameObject.SetActive(true);
            m_BgRenderer.material = exploreSO.ScrollableMat;

            // Spawn character's running prefab here.
            // Scoll the background.
            // Start a countdown for about 2 - 4 seconds.
            // Roll 2 random events.
        }

        private void GenerateEvent()
        {

        }
    }
}