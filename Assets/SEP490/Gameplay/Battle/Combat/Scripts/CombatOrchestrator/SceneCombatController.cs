namespace SEP490G69.Battle.Combat
{
    using SEP490G69.GameSessions;
    using SEP490G69.Tournament;
    using System;
    using UnityEngine;

    public enum EBattleState
    {
        Pending = 0,
        InProgress = 1,
        Pause = 2,
        Finish = 3,
    }

    public class SceneCombatController : MonoBehaviour, ISceneContext
    {
        public event Action<EBattleState> OnStateChanged;

        [Header("Scene References")]
        [SerializeField] private Transform m_PlayerContainer;
        [SerializeField] private Transform m_EnemyContainer;
        [SerializeField] private Transform m_PlayerCombatPos;
        [SerializeField] private Transform m_EnemyCombatPos;
        [SerializeField] private string m_CharacterPoolName = "CombatCharacter";

        private BattleStateMachine _battleState;
        private CombatInitializer _initializer;
        private CombatTurnProcessor _turnProcessor;
        private CombatUIUpdater _uiUpdater;

        private GameSessionDAO _sessionDAO;
        private TournamentProgressDAO _tournamentDAO;

        private PlayerBattleCharaterController _player;
        private EnemyCombatController _enemy;

        public PlayerBattleCharaterController Player => _player;
        public EnemyCombatController Enemy => _enemy;
        public CombatTurnProcessor TurnSystem => _turnProcessor;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            _battleState = new BattleStateMachine();
            _initializer = new CombatInitializer();
            _turnProcessor = new CombatTurnProcessor();
            _uiUpdater = new CombatUIUpdater();

            _sessionDAO = new GameSessionDAO();
            _tournamentDAO = new TournamentProgressDAO();
        }

        private void Start()
        {
            InitializeBattle();
            BindEvents();
            _battleState.ChangeState(EBattleState.Pending);
        }

        private void Update()
        {
            if (_battleState.CurrentState != EBattleState.InProgress)
            {
                return;
            }

            _turnProcessor.Update(Time.deltaTime);
            _uiUpdater.UpdateEnergy(_player, _enemy);
            _uiUpdater.UpdateStats(_player, _enemy);
        }

        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
            _turnProcessor.Dispose();
            UnbindEvents();
        }

        private void InitializeBattle()
        {
            _initializer.Initialize(m_PlayerContainer, m_EnemyContainer, m_CharacterPoolName, out _player, out _enemy);

            if (_player == null || _enemy == null)
            {
                Debug.LogError($"[SceneCombatController.InitializeBattle error] Player or Enemy controller instance(s) is/are null");
                return;
            }

            _player.AnimationController.SetCombatPosition(m_PlayerCombatPos);
            _enemy.AnimationController.SetCombatPosition(m_EnemyCombatPos);

            _turnProcessor.Initialize(_player, _enemy);
            _uiUpdater.ShowCombatPreview(_player, _enemy);
        }

        private void BindEvents()
        {
            if (_player != null) _player.onEnergyFull += HandlePlayerTurn;
            if (_player != null) _player.onDead += HandlePlayerDefeated;

            if (_enemy != null) _enemy.onEnergyFull += HandleEnemyTurn;
            if (_enemy != null) _enemy.onDead += HandleEnemyDefeated;

            _battleState.OnStateChanged += state =>
            {
                OnStateChanged?.Invoke(state);
            };

            OnStateChanged += SceneCombatController_OnStateChanged;
        }

        private void UnbindEvents()
        {
            if (_player != null) _player.onEnergyFull -= HandlePlayerTurn;
            if (_player != null) _player.onDead -= HandlePlayerDefeated;

            if (_enemy != null) _enemy.onEnergyFull -= HandleEnemyTurn;
            if (_enemy != null) _enemy.onDead -= HandleEnemyDefeated;
        }

        public void StartBattle()
        {
            if (_player == null || _enemy == null)
            {
                Debug.LogError($"[SceneCombatController.StartBattle error] Player or Enemy controller instance(s) is/are null");
                return;
            }
            _battleState.ChangeState(EBattleState.InProgress);
            _uiUpdater.ShowCombatHUD(_player, _enemy);
        }

        public void PauseBattle() => _battleState.ChangeState(EBattleState.Pause);
        public void ResumeBattle() => _battleState.ChangeState(EBattleState.InProgress);

        private void HandlePlayerTurn(BaseBattleCharacterController character)
        {
            _turnProcessor.PlayerTurn();
            _uiUpdater.UpdateStats(_player, _enemy);
        }

        private void HandleEnemyTurn(BaseBattleCharacterController character)
        {
            _turnProcessor.EnemyTurn();
            _uiUpdater.UpdateStats(_player, _enemy);
        }

        private void HandleEnemyDefeated()
        {
            _battleState.ChangeState(EBattleState.Finish);
            OnVictorious();
        }

        private void HandlePlayerDefeated()
        {
            _battleState.ChangeState(EBattleState.Finish);
            OnDefeated();
        }

        private void SceneCombatController_OnStateChanged(EBattleState state)
        {
            switch(state)
            {
                case EBattleState.Pause:
                case EBattleState.Finish:
                    _enemy.PauseBar();
                    _player.PauseBar();
                    break;
            }
        }

        private void OnVictorious()
        {
            _uiUpdater.ShowVictory();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Session id in cache is null/empty");
                return;
            }

            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);

            if (sessionData == null)
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Session data with id {sessionId} does not exist in the database");
                return;
            }

            TournamentProgressData tournamentData = _tournamentDAO.GetById(sessionData.ActiveTournamentId);

            if (string.IsNullOrEmpty(sessionData.ActiveTournamentId))
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Active tournament id in session {sessionData.SessionId} is null/empty");
                return;
            }

            if (tournamentData == null)
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Tournament data with id {sessionData.ActiveTournamentId} does not exist in the database");
                return;
            }

            tournamentData.IsBattleFinished = true;
            tournamentData.IsPlayerWon = true;

            _tournamentDAO.Upsert(tournamentData);          
        }
        private void OnDefeated()
        {
            _uiUpdater.ShowDefeat();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Session id in cache is null/empty");
                return;
            }

            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);

            if (sessionData == null)
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Session data with id {sessionId} does not exist in the database");
                return;
            }

            TournamentProgressData tournamentData = _tournamentDAO.GetById(sessionData.ActiveTournamentId);

            if (string.IsNullOrEmpty(sessionData.ActiveTournamentId))
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Active tournament id in session {sessionData.SessionId} is null/empty");
                return;
            }

            if (tournamentData == null)
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Tournament data with id {sessionData.ActiveTournamentId} does not exist in the database");
                return;
            }

            tournamentData.IsBattleFinished = true;
            tournamentData.IsPlayerWon = false;

            _tournamentDAO.Upsert(tournamentData);
        }
    }
}