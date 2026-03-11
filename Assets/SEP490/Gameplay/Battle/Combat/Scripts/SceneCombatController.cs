namespace SEP490G69.Battle.Combat
{
    using System;
    using System.Collections.Generic;
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Battle.Cards;
    using SEP490G69.GameSessions;
    using SEP490G69.Shared;
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
        [SerializeField] private string m_CharacterPoolName = "CombatCharacter";

        private BattleStateMachine _battleState;
        private CombatInitializer _initializer;
        private CombatTurnSystem _turnSystem;
        private CombatUIUpdater _uiUpdater;

        private PlayerBattleCharaterController _player;
        private EnemyCombatController _enemy;

        public PlayerBattleCharaterController Player => _player;
        public EnemyCombatController Enemy => _enemy;
        public CombatTurnSystem TurnSystem => _turnSystem;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            _battleState = new BattleStateMachine();
            _initializer = new CombatInitializer();
            _turnSystem = new CombatTurnSystem();
            _uiUpdater = new CombatUIUpdater();
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

            _turnSystem.Update(Time.deltaTime);
            _uiUpdater.UpdateEnergy(_player, _enemy);
        }

        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
            UnbindEvents();
        }

        private void InitializeBattle()
        {
            (_player, _enemy) = _initializer.Initialize(
                m_PlayerContainer,
                m_EnemyContainer,
                m_CharacterPoolName);

            _turnSystem.Initialize(_player, _enemy);
            _uiUpdater.ShowCombatPreview(_player, _enemy);
        }

        private void BindEvents()
        {
            _player.OnEnergyFull += HandlePlayerTurn;
            _enemy.OnEnergyFull += HandleEnemyTurn;

            _player.OnDead += HandlePlayerDefeated;
            _enemy.OnDead += HandleEnemyDefeated;

            _battleState.OnStateChanged += state =>
            {
                OnStateChanged?.Invoke(state);
            };

            OnStateChanged += SceneCombatController_OnStateChanged;
        }

        private void UnbindEvents()
        {
            _player.OnEnergyFull -= HandlePlayerTurn;
            _enemy.OnEnergyFull -= HandleEnemyTurn;

            _player.OnDead -= HandlePlayerDefeated;
            _enemy.OnDead -= HandleEnemyDefeated;
        }

        public void StartBattle()
        {
            _battleState.ChangeState(EBattleState.InProgress);
            _uiUpdater.ShowCombatHUD(_player, _enemy);
        }

        public void PauseBattle() => _battleState.ChangeState(EBattleState.Pause);
        public void ResumeBattle() => _battleState.ChangeState(EBattleState.InProgress);

        private void HandlePlayerTurn(BaseBattleCharacterController character)
        {
            _turnSystem.PlayerTurn();
        }

        private void HandleEnemyTurn(BaseBattleCharacterController character)
        {
            _turnSystem.EnemyTurn();
            _uiUpdater.UpdateStats(_player, _enemy);
        }

        private void HandleEnemyDefeated()
        {
            _battleState.ChangeState(EBattleState.Finish);
            _uiUpdater.ShowVictory();
        }

        private void HandlePlayerDefeated()
        {
            _battleState.ChangeState(EBattleState.Finish);
            _uiUpdater.ShowDefeat();
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
    }
}