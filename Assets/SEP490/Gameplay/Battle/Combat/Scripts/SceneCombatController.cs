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

        private List<CardSO> _deck = new List<CardSO>();
        private List<CardSO> _discard = new List<CardSO>();
        private List<CardSO> _currentDraw = new List<CardSO>();
        private CardSO _selectedCard = null;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);

            LoadDAOs();
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);

            if (_playerCharacterCombat != null) _playerCharacterCombat.OnEnergyFull -= HandlePlayerEnergyFull;
            if (_enemyCharacterCombat != null) _enemyCharacterCombat.OnEnergyFull -= HandleEnemyEnergyFull;
        }

        private void Start()
        {
            InitializeBattle();

            if (_playerCharacterCombat != null) _playerCharacterCombat.OnEnergyFull += HandlePlayerEnergyFull;
            if (_enemyCharacterCombat != null) _enemyCharacterCombat.OnEnergyFull += HandleEnemyEnergyFull;
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
                             .SetPlayerCharSpeed(_playerCharacterCombat.GetCurrentEnergyValue(), _playerCharacterCombat.GetMaxEnergyValue());
            }
            if (_enemyCharacterCombat != null) 
            { 
                _enemyCharacterCombat.UpdateCharge(dt);
                GameUIManager.Singleton
                             .GetFrame(GameConstants.FRAME_ID_COMBAT)
                             .AsFrame<UICombatFrame>()
                             .SetEnemyCharSpeed(_enemyCharacterCombat.GetCurrentEnergyValue(), _enemyCharacterCombat.GetMaxEnergyValue());
            }
        }

        private void LoadDAOs()
        {
            _sessionDAO = new GameSessionDAO(LocalDBInitiator.GetDatabase());
        }

        public void InitializeBattle()
        {
            UseSampleData();
            InitializePlayerDeck();
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
            DrawThreeCards();
        }

        /// <summary>
        /// Perform card.
        /// </summary>
        /// <param name="character"></param>
        private void HandleEnemyEnergyFull(BaseBattleCharacterController character)
        {
            // Handle AI brain here.

            _playerCharacterCombat.OnTurnStart();
            UpdateToUI();
        }

        public void PerformSelectedPlayerCard()
        {
            if (_selectedCard == null)
            {
                Debug.LogWarning("No card selected");
                return;
            }

            // Execute effect
            _playerCharacterCombat.ReceiveCardEffect(_selectedCard, _playerCharacterCombat, _enemyCharacterCombat);

            // Put 2 remain cards to discard list.
            foreach (var card in _currentDraw)
            {
                if (card != _selectedCard)
                    _discard.Add(card);
            }

            _selectedCard = null;

            _playerCharacterCombat.OnTurnStart();

            _enemyCharacterCombat.UnpauseBar();

            UpdateToUI();
            DrawThreeCards();
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
                       .SetPlayerCharContent(_playerCharacterCombat.CharacterDataHolder.GetRawId(), _playerCharacterCombat.ReadonlyDataHolder.GetAvatar())
                       .SetPlayerCharVit(_playerCharacterCombat.CharacterDataHolder.GetVIT(), _playerCharacterCombat.ReadonlyDataHolder.GetVIT())
                       .SetPlayerCharDef(_playerCharacterCombat.CharacterDataHolder.GetDef(), _playerCharacterCombat.ReadonlyDataHolder.GetDef())
                       .SetPlayerCharSpeed(_playerCharacterCombat.GetCurrentEnergyValue(), _playerCharacterCombat.GetMaxEnergyValue())
                       .SetEnemyCharContent(_enemyCharacterCombat.CharacterDataHolder.GetRawId(), _enemyCharacterCombat.ReadonlyDataHolder.GetAvatar())
                       .SetEnemyCharVit(_enemyCharacterCombat.CharacterDataHolder.GetVIT(), _enemyCharacterCombat.ReadonlyDataHolder.GetVIT())
                       .SetEnemyCharDef(_enemyCharacterCombat.CharacterDataHolder.GetDef(), _enemyCharacterCombat.ReadonlyDataHolder.GetDef())
                       .SetEnemyCharSpeed(_enemyCharacterCombat.GetCurrentEnergyValue(), _enemyCharacterCombat.GetMaxEnergyValue());
        }
        #endregion

        private void UpdateToUI()
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_COMBAT)
                       .AsFrame<UICombatFrame>()
                       .SetPlayerCharContent(_playerCharacterCombat.CharacterDataHolder.GetRawId(), _playerCharacterCombat.CharacterDataHolder.GetAvatar())
                       .SetPlayerCharVit(_playerCharacterCombat.CharacterDataHolder.GetVIT(), _playerCharacterCombat.ReadonlyDataHolder.GetVIT())
                       .SetPlayerCharDef(_playerCharacterCombat.CharacterDataHolder.GetDef(), _playerCharacterCombat.ReadonlyDataHolder.GetDef())
                       .LoadPlayerStatEffects(_playerCharacterCombat.ActiveStatuses)
                       .SetEnemyCharContent(_enemyCharacterCombat.CharacterDataHolder.GetRawId(), _enemyCharacterCombat.ReadonlyDataHolder.GetAvatar())
                       .SetEnemyCharVit(_enemyCharacterCombat.CharacterDataHolder.GetVIT(), _enemyCharacterCombat.ReadonlyDataHolder.GetVIT())
                       .SetEnemyCharDef(_enemyCharacterCombat.CharacterDataHolder.GetDef(), _enemyCharacterCombat.ReadonlyDataHolder.GetDef())
                       .LoadEnemyStatEffects(_enemyCharacterCombat.ActiveStatuses);
        }

        #region Card & Deck logic
        private void InitializePlayerDeck()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            SessionPlayerDeck deckData = new SessionPlayerDeck
            {
                CardIds = m_ObtainedCardConfig.Ids,
            };

            if (deckData == null || deckData.CardIds == null || deckData.CardIds.Length == 0)
            {
                Debug.LogError("Deck is empty");
                return;
            }

            _deck.Clear();
            _discard.Clear();
            _currentDraw.Clear();

            foreach (var id in deckData.CardIds)
            {
                CardSO card = CardConfig.GetCardById(id);
                if (card != null)
                    _deck.Add(card);
            }

            Shuffle(_deck);
        }

        private void Shuffle(List<CardSO> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

        private void DrawThreeCards()
        {
            if (_deck.Count == 0)
            {
                ReshuffleFromDiscard();
            }

            _currentDraw.Clear();

            int drawCount = Mathf.Min(3, _deck.Count);

            for (int i = 0; i < drawCount; i++)
            {
                CardSO card = _deck[0];
                _deck.RemoveAt(0);
                _currentDraw.Add(card);
            }

            GameUIManager.Singleton
                .GetFrame(GameConstants.FRAME_ID_COMBAT)
                .AsFrame<UICombatFrame>()
                .DisplayDrawnCards(_currentDraw);
        }

        private void ReshuffleFromDiscard()
        {
            if (_discard.Count == 0)
                return;

            _deck.AddRange(_discard);
            _discard.Clear();
            Shuffle(_deck);
        }

        public void SelectCardById(string cardId)
        {
            CardSO cardSO = _currentDraw.FirstOrDefault(c => c.CardId.Equals(cardId));
            if (cardSO == null)
            {
                Debug.LogError("CardSO with id is not in the current draw");
                return;
            }
            SelectCard(cardSO);
        }

        public void SelectCard(CardSO card)
        {
            _selectedCard = card;
        }
        public void DeselectCurrentCard()
        {
            _selectedCard = null;
        }
        #endregion
    }
}