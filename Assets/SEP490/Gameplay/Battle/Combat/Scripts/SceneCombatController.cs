namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;
    using System.Linq;
    using SEP490G69.Battle.Cards;
    using SEP490G69.GameSessions;
    using UnityEngine;

    public class SceneCombatController : MonoBehaviour, ISceneContext
    {
        [SerializeField] private string m_CharacterPoolName = "CombatCharacter";
        [SerializeField] private Transform m_PlayerContainer;
        [SerializeField] private Transform m_EnemyContainer;

        [Header("Testing")]
        [SerializeField] private TestObtainedCardSO m_ObtainedCardConfig;

        private PlayerBattleCharaterController _playerCharacterCombat;
        private EnemyCombatController _enemyCharacterCombat;

        private string _playerCharacterId;
        private string _enemyCharacterId;

        private GameSessionDAO _sessionDAO;

        private CharacterConfigSO _characterConfig;
        protected CharacterConfigSO CharacterConfig
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

        private CardConfigSO _cardConfig;
        protected CardConfigSO CardConfig
        {
            get
            {
                if (_cardConfig == null)
                {
                    _cardConfig = ContextManager.Singleton.GetDataSO<CardConfigSO>();
                }
                return _cardConfig;
            }
        }

        public PlayerBattleCharaterController PlayerCharController => _playerCharacterCombat;
        public EnemyCombatController EnemyCombatController => _enemyCharacterCombat;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            LoadDAOs();
        }
        private void Start()
        {
            InitializeBattle();

            if (_playerCharacterCombat != null) _playerCharacterCombat.OnEnergyFull += HandlePlayerEnergyFull;
            if (_enemyCharacterCombat != null) _enemyCharacterCombat.OnEnergyFull += HandleEnemyEnergyFull;
        }

        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);

            if (_playerCharacterCombat != null) _playerCharacterCombat.OnEnergyFull -= HandlePlayerEnergyFull;
            if (_enemyCharacterCombat != null) _enemyCharacterCombat.OnEnergyFull -= HandleEnemyEnergyFull;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (_playerCharacterCombat != null)
            {
                _playerCharacterCombat.UpdateCharge(dt);
                GameUIManager.Singleton
                             .GetFrame(GameConstants.FRAME_ID_COMBAT)
                             .AsFrame<UICombatFrame>()
                             .SetPlayerCharGauge(_playerCharacterCombat.GetCurrentEnergyValue(), _playerCharacterCombat.GetMaxEnergyValue());
            }
            if (_enemyCharacterCombat != null) 
            { 
                _enemyCharacterCombat.UpdateCharge(dt);
                GameUIManager.Singleton
                             .GetFrame(GameConstants.FRAME_ID_COMBAT)
                             .AsFrame<UICombatFrame>()
                             .SetEnemyCharGauge(_enemyCharacterCombat.GetCurrentEnergyValue(), _enemyCharacterCombat.GetMaxEnergyValue());
            }
        }

        private void LoadDAOs()
        {
            _sessionDAO = new GameSessionDAO(LocalDBInitiator.GetDatabase());
        }

        public void InitializeBattle()
        {
            UseSampleData();
        }

        public void StartBattle()
        {

        }

        /// <summary>
        /// Pause timeline and draw 3 random cards.
        /// </summary>
        /// <param name="character"></param>
        private void HandlePlayerEnergyFull(BaseBattleCharacterController character)
        {
            _enemyCharacterCombat.PauseBar();

            PlayerCharController.OnTurnStart();

            PlayerCharController.DrawThreeCards(out IReadOnlyList<CardSO> cards);

            GameUIManager.Singleton
                .GetFrame(GameConstants.FRAME_ID_COMBAT)
                .AsFrame<UICombatFrame>()
                .DisplayDrawnCards(cards);
        }

        /// <summary>
        /// Perform card.
        /// </summary>
        /// <param name="character"></param>
        private void HandleEnemyEnergyFull(BaseBattleCharacterController character)
        {
            // Handle AI brain here.
            //_enemyCharacterCombat.DetermineCards(_playerCharacterCombat);
            UpdateToUI();
        }

        public void PerformSelectedPlayerCard()
        {
            if (PlayerCharController.SelectedCard == null)
            {
                Debug.LogWarning("No card selected");
                return;
            }

            // Execute effect
            _playerCharacterCombat.ExecuteCard(_playerCharacterCombat, _enemyCharacterCombat);
            _playerCharacterCombat.EndCurrentTurn();

            _enemyCharacterCombat.UnpauseBar();

            UpdateToUI();
            //PlayerCharController.DrawThreeCards(out IReadOnlyList<CardSO> cards);
            GameUIManager.Singleton
                .GetFrame(GameConstants.FRAME_ID_COMBAT)
                .AsFrame<UICombatFrame>()
                .ClearAllCards();
        }

        #region Testing only (Delete when the tournament or explore combat event are available)
        private void UseSampleData()
        {
            // Initialize player character
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError("Session id is empty");
                return;
            }
            PlayerTrainingSession sessionData = _sessionDAO.GetSession(sessionId);
            if (sessionData == null)
            {
                Debug.LogError($"Session with id {sessionId} does not exist!");
                return;
            }
            BaseCharacterSO _playerCharSO = CharacterConfig.GetCharacterById(sessionData.CharacterId);
            if (_playerCharSO == null)
            {
                Debug.LogError($"BaseCharacterSO with id {sessionData.CharacterId} is not configred yet!");
                return;
            }

            Transform playerCharTrans = PoolManager.Pools[m_CharacterPoolName].Spawn(_playerCharSO.CombatPrefab, m_PlayerContainer);
            _playerCharacterCombat = playerCharTrans.GetComponent<PlayerBattleCharaterController>();
            if (_playerCharacterCombat != null)
            {
                _playerCharacterCombat.SetSampleDeck(m_ObtainedCardConfig);
                _playerCharacterCombat.Initialize(_playerCharSO);
            }
            else
            {
                Debug.LogError($"PlayerCombatController does not exist in character {_enemyCharacterId} prefab");
            }

            // Initialize enemy
            _enemyCharacterId = "ch_0010";
            BaseCharacterSO enemySO = CharacterConfig.GetCharacterById(_enemyCharacterId);
            Transform enemyTrans = PoolManager.Pools[m_CharacterPoolName].Spawn(enemySO.CombatPrefab, m_EnemyContainer);
            _enemyCharacterCombat = enemyTrans.GetComponent<EnemyCombatController>();
            if (_enemyCharacterCombat != null)
            {
                _enemyCharacterCombat.Initialize(enemySO);
            }
            else
            {
                Debug.LogError($"EnemyCombatController does not exist in character {_enemyCharacterId} prefab");
            }
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_COMBAT)
                       .AsFrame<UICombatFrame>()
                       .SetPlayerCharContent(_playerCharacterCombat.CurrentDataHolder.GetRawId(), _playerCharacterCombat.ReadonlyDataHolder.GetAvatar())
                       .SetPlayerCharVit(_playerCharacterCombat.CurrentDataHolder.GetVIT(), _playerCharacterCombat.ReadonlyDataHolder.GetVIT())
                       .SetPlayerCharStamina(_playerCharacterCombat.CurrentDataHolder.GetStamina(), _playerCharacterCombat.ReadonlyDataHolder.GetStamina())
                       .SetPlayerCharGauge(_playerCharacterCombat.GetCurrentEnergyValue(), _playerCharacterCombat.GetMaxEnergyValue())
                       .SetEnemyCharContent(_enemyCharacterCombat.CurrentDataHolder.GetRawId(), _enemyCharacterCombat.ReadonlyDataHolder.GetAvatar())
                       .SetEnemyCharVit(_enemyCharacterCombat.CurrentDataHolder.GetVIT(), _enemyCharacterCombat.ReadonlyDataHolder.GetVIT())
                       .SetEnemyCharStamina(_enemyCharacterCombat.CurrentDataHolder.GetStamina(), _enemyCharacterCombat.ReadonlyDataHolder.GetStamina())
                       .SetEnemyCharGauge(_enemyCharacterCombat.GetCurrentEnergyValue(), _enemyCharacterCombat.GetMaxEnergyValue());
        }
        #endregion

        private void UpdateToUI()
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_COMBAT)
                       .AsFrame<UICombatFrame>()
                       .SetPlayerCharContent(_playerCharacterCombat.ReadonlyDataHolder.GetRawId(), _playerCharacterCombat.ReadonlyDataHolder.GetAvatar())
                       .SetPlayerCharVit(_playerCharacterCombat.CurrentDataHolder.GetVIT(), _playerCharacterCombat.ReadonlyDataHolder.GetVIT())
                       .SetPlayerCharStamina(_playerCharacterCombat.CurrentDataHolder.GetStamina(), _playerCharacterCombat.ReadonlyDataHolder.GetStamina())
                       .LoadPlayerStatEffects(_playerCharacterCombat.StatEffectManager.ActiveStatuses)
                       .SetEnemyCharContent(_enemyCharacterCombat.CurrentDataHolder.GetRawId(), _enemyCharacterCombat.ReadonlyDataHolder.GetAvatar())
                       .SetEnemyCharVit(_enemyCharacterCombat.CurrentDataHolder.GetVIT(), _enemyCharacterCombat.ReadonlyDataHolder.GetVIT())
                       .SetEnemyCharStamina(_enemyCharacterCombat.CurrentDataHolder.GetStamina(), _enemyCharacterCombat.ReadonlyDataHolder.GetStamina())
                       .LoadEnemyStatEffects(_enemyCharacterCombat.StatEffectManager.ActiveStatuses);
        }
    }
}