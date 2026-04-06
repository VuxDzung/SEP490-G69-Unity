namespace SEP490G69.Battle.Combat
{
    using SEP490G69.GameSessions;
    using SEP490G69.Tournament;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using System.Linq;
    using SEP490G69.Battle.Cards;

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
        public const bool IS_AUTO_COMBAT = true;
        public const bool IS_MANUAL_COMBAT = false;

        [Header("Scene References")]
        [SerializeField] private List<CharacterCombatPosition> m_EnemyCombatPositions;
        [SerializeField] private List<CharacterCombatPosition> m_PlayerCombatPositions;
        [SerializeField] private Transform m_PlayerContainer;
        [SerializeField] private Transform m_EnemyContainer;
        [SerializeField] private string m_CharacterPoolName = "CombatCharacter";

        private CombatInitializer _initializer;
        private CombatTurnProcessor _turnProcessor;
        private CombatUIUpdater _uiUpdater;

        private GameSessionDAO _sessionDAO;
        private TournamentProgressDAO _tournamentDAO;

        private PlayerActorController _player;
        private EnemyActorController _enemy;

        private Dictionary<string, IFinishCombatHandler> _finishResultHandlers = new Dictionary<string, IFinishCombatHandler>();

        public PlayerActorController Player => _player;
        public EnemyActorController Enemy => _enemy;
        public CombatTurnProcessor TurnProcessor => _turnProcessor;
        public CombatUIUpdater CombatUI => _uiUpdater;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            _initializer = new CombatInitializer();
            _turnProcessor = new CombatTurnProcessor();
            _uiUpdater = new CombatUIUpdater();

            _sessionDAO = new GameSessionDAO();
            _tournamentDAO = new TournamentProgressDAO();

            _finishResultHandlers = new Dictionary<string, IFinishCombatHandler>
            {
                { GameConstants.COMBAT_TYPE_TOURNAMENT, new FinishTournamentCombatHandler(_tournamentDAO) },
                { GameConstants.COMBAT_TYPE_EXPLORATION, new FinishExploreCombatHandler() },
                { GameConstants.COMBAT_TYPE_TESTING, new FinishTestingCombatHandler() },
            };
        }

        private void Start()
        {
            FadingController.Singleton.FadeOut(1f, Color.white);
            InitializeBattle();
            BindEvents();
        }

        private void Update()
        {
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
            _initializer.Initialize(this, m_PlayerContainer, m_EnemyContainer, m_CharacterPoolName, out _player, out _enemy);

            if (_player == null || _enemy == null)
            {
                Debug.LogError($"[SceneCombatController.InitializeBattle error] Player or Enemy controller instance(s) is/are null");
                return;
            }

            PlayerCharacterDataSO playerSO = _player.CharacterSO;
            EnemySO enemySO = _enemy.CharacterSO;

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

            _uiUpdater.ShowCombatPreview(_player, _enemy);

            _turnProcessor.Initialize(this, _player, _enemy);
        }

        private void BindEvents()
        {
            if (_player != null)
            {
                _player.onDead += HandlePlayerDefeated;
                _player.onFlowEventChanged += _player_onFlowEventChanged;
            }
            if (_enemy != null)
            {
                _enemy.onDead += HandleEnemyDefeated;
                _enemy.onFlowEventChanged += _enemy_onFlowEventChanged;
            }
        }

        private void UnbindEvents()
        {
            if (_player != null)
            {
                _player.onDead -= HandlePlayerDefeated;
                _player.onFlowEventChanged -= _player_onFlowEventChanged;
            }
            if (_enemy != null)
            {
                _enemy.onDead -= HandleEnemyDefeated;
                _enemy.onFlowEventChanged -= _enemy_onFlowEventChanged;
            }
        }

        public void StartBattle(bool isAutoCombat = false)
        {
            if (_player == null || _enemy == null)
            {
                Debug.LogError($"[SceneCombatController.StartBattle error] Player or Enemy controller instance(s) is/are null");
                return;
            }

            _uiUpdater.ShowCombatHUD(_player, _enemy);
            //_player.SetCombatMode(isAutoCombat);
            ChangeToPlayerTurn();
        }

        private void _enemy_onFlowEventChanged(ETurnFlowEvent ev, BaseCombatActor arg2)
        {
            CombatUI.ShowPlayerStatusEffects(_player);
            CombatUI.ShowEnemyStatusEffects(_enemy);

            CombatUI.UpdateStats(_player, _enemy);
        }

        private void _player_onFlowEventChanged(ETurnFlowEvent ev, BaseCombatActor arg2)
        {
            if (ev == ETurnFlowEvent.AfterCardAction)
            {
                CombatUI.ClearAllUICards();
                CombatUI.DisplayPlayerCards(_player.CardsService.GetInHandCards(), _player.CardsService, _player.StatsManager.GetValue(EStatusType.Stamina));
            }

            CombatUI.ShowPlayerStatusEffects(_player);
            CombatUI.ShowEnemyStatusEffects(_enemy);

            CombatUI.UpdateStats(_player, _enemy);
        }

        private void HandleEnemyDefeated()
        {
            OnVictorious();
        }

        private void HandlePlayerDefeated()
        {
            OnDefeated();
        }

        public void ChangeToPlayerTurn()
        {
            if (_turnProcessor != null)
            {
                _turnProcessor.ChangeToPlayerTurn();
            }
        }
        public void ChangeToEnemyTurn()
        {
            if (_turnProcessor != null)
            {
                _turnProcessor.ChangeToEnemyTurn();
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

            if (_finishResultHandlers.TryGetValue(combatType, out var finishHandler) == false)
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Unsupported combat finish handler type {combatType}");
                return;
            }

            _uiUpdater.ShowVictory(finishHandler);

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

            finishHandler.HandleCombatResult(sessionData, true);
        }
        private void OnDefeated()
        {
            string combatType = PlayerPrefs.GetString(GameConstants.PREF_KEY_COMBAT_TYPE, string.Empty);
            if (string.IsNullOrEmpty(combatType))
            {
                Debug.LogError("[SceneCombatController.OnVictorious fatal error] Combat type value string is empty");
                return;
            }
            if (_finishResultHandlers.TryGetValue(combatType, out var finishHandler) == false)
            {
                Debug.LogError($"[SceneCombatController.OnDefeated] Unsupported combat finish handler type {combatType}");
                return;
            }

            _uiUpdater.ShowDefeat(finishHandler);

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

            finishHandler.HandleCombatResult(sessionData, false);
        }

        #region APIs
        public bool IsCardUsable(CardSO card)
        {
            return _player.IsCardUsable(card, this);
        }
        #endregion
    }
}