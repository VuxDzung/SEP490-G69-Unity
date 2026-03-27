namespace SEP490G69.Battle.Combat
{
    using SEP490G69.GameSessions;
    using SEP490G69.Tournament;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using System.Linq;

    public enum EBattleState
    {
        Pending = 0,
        InProgress = 1,
        Pause = 2,
        Finish = 3,
    }

    public enum EAttackType
    {
        None = 0,
        Melee = 1,
        Ranged = 2,
        Both = 3,
    }

    [System.Serializable]
    public class CharacterCombatPosition
    {
        [SerializeField] private EAttackType m_AtkType;
        [SerializeField] private Transform m_CombatPosition;

        public EAttackType AtkType => m_AtkType;
        public Transform CombatPosition => m_CombatPosition;
    }

    public class SceneCombatController : MonoBehaviour, ISceneContext
    {
        public event Action<EBattleState> OnStateChanged;

        [Header("Scene References")]
        [SerializeField] private List<CharacterCombatPosition> m_EnemyCombatPositions;
        [SerializeField] private List<CharacterCombatPosition> m_PlayerCombatPositions;
        [SerializeField] private Transform m_PlayerContainer;
        [SerializeField] private Transform m_EnemyContainer;
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

            PlayerCharacterDataSO playerSO = _player.CharacterConfig.GetCharacterById(_player.ReadonlyDataHolder.GetRawId()).ConvertAs<PlayerCharacterDataSO>();
            EnemySO enemySO = _player.CharacterConfig.GetCharacterById(_enemy.ReadonlyDataHolder.GetRawId()).ConvertAs<EnemySO>();

            if (playerSO == null || enemySO == null)
            {
                return;
            }

            EAttackType playerAtkType = playerSO.AtkType;
            EAttackType enemyAtkType = enemySO.AtkType;

            Transform playerCombatPos = m_PlayerCombatPositions.FirstOrDefault(p => p.AtkType == playerAtkType).CombatPosition;
            Transform enemyCombatPos = m_EnemyCombatPositions.FirstOrDefault(p => p.AtkType == enemyAtkType).CombatPosition;

            _player.AnimationController.SetCombatPosition(playerCombatPos);
            _enemy.AnimationController.SetCombatPosition(enemyCombatPos);

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

            _turnProcessor.onPlayerEndTurn += _turnProcessor_onPlayerEndTurn;
            _turnProcessor.onEnemyEndTurn += _turnProcessor_onEnemyEndTurn;

            OnStateChanged += SceneCombatController_OnStateChanged;
        }

        private void UnbindEvents()
        {
            if (_player != null) _player.onEnergyFull -= HandlePlayerTurn;
            if (_player != null) _player.onDead -= HandlePlayerDefeated;

            if (_enemy != null) _enemy.onEnergyFull -= HandleEnemyTurn;
            if (_enemy != null) _enemy.onDead -= HandleEnemyDefeated;

            _turnProcessor.onPlayerEndTurn -= _turnProcessor_onPlayerEndTurn;
            _turnProcessor.onEnemyEndTurn -= _turnProcessor_onEnemyEndTurn;
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
            _uiUpdater.ShowPlayerStatusEffects(_player);
            _uiUpdater.ShowEnemyStatusEffects(_enemy);
        }

        private void HandleEnemyTurn(BaseBattleCharacterController character)
        {
            _turnProcessor.EnemyTurn();
            _uiUpdater.UpdateStats(_player, _enemy);
            _uiUpdater.ShowPlayerStatusEffects(_player);
            _uiUpdater.ShowEnemyStatusEffects(_enemy);
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
            string combatType = PlayerPrefs.GetString(GameConstants.PREF_KEY_COMBAT_TYPE, string.Empty);

            if (string.IsNullOrEmpty(combatType))
            {
                Debug.LogError("[SceneCombatController.OnVictorious fatal error] Combat type value string is empty");
                return;
            }

            _uiUpdater.ShowVictory(combatType);

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


            switch (combatType)
            {
                case GameConstants.COMBAT_TYPE_TOURNAMENT:
                    HandleCombatTournamentCompleted(sessionData, true);
                    break;
                case GameConstants.COMBAT_TYPE_EXPLORATION:
                    HandleCombatExploreCompleted(sessionData, true);
                    break;
            }
        }
        private void OnDefeated()
        {
            string combatType = PlayerPrefs.GetString(GameConstants.PREF_KEY_COMBAT_TYPE, string.Empty);
            if (string.IsNullOrEmpty(combatType))
            {
                Debug.LogError("[SceneCombatController.OnVictorious fatal error] Combat type value string is empty");
                return;
            }

            _uiUpdater.ShowDefeat(combatType);

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

            switch (combatType)
            {
                case GameConstants.COMBAT_TYPE_TOURNAMENT:
                    HandleCombatTournamentCompleted(sessionData, true);
                    break;
                case GameConstants.COMBAT_TYPE_EXPLORATION:
                    HandleCombatExploreCompleted(sessionData, true);
                    break;
            }
        }

        private void HandleCombatTournamentCompleted(PlayerTrainingSession sessionData, bool isPlayerWon)
        {
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
            tournamentData.IsPlayerWon = isPlayerWon;

            _tournamentDAO.Upsert(tournamentData);
        }

        private void HandleCombatExploreCompleted(PlayerTrainingSession sessionData, bool isPlayerWon)
        {
            PlayerPrefs.SetInt(GameConstants.PREF_KEY_IS_BATTLE_WON, isPlayerWon ? 1 : 0);
        }

        private void _turnProcessor_onEnemyEndTurn()
        {
            _uiUpdater.ShowPlayerStatusEffects(_player);
            _uiUpdater.ShowEnemyStatusEffects(_enemy);
        }

        private void _turnProcessor_onPlayerEndTurn()
        {
            _uiUpdater.ShowPlayerStatusEffects(_player);
            _uiUpdater.ShowEnemyStatusEffects(_enemy);
        }
    }
}