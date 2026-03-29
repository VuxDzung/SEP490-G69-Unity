namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using System.Collections;

    public class PlayerBattleCharaterController : BaseBattleCharacterController
    {
        private PlayerCharacterRepository _characterRepo;
        private GameDeckDAO _deckDAO;

        private SessionPlayerDeck _playerDeckData;
        private TestObtainedCardSO _test;

        private GameInventoryManager _inventoryManager;
        private GameInventoryManager InventoryManager
        {
            get
            {
                if (_inventoryManager == null)
                {
                    _inventoryManager = ContextManager.Singleton.ResolveGameContext<GameInventoryManager>();
                }
                return _inventoryManager;
            }
        }

        private ISelectCardStrategy _selectionStrategy = new RandomSelectCardStrategy();

        private bool _isAuto;
        public bool IsAuto => _isAuto;

        public void SetSampleDeck(TestObtainedCardSO obtainedCards)
        {
            _test = obtainedCards;
        }

        protected override void Awake()
        {
            base.Awake();
            _deckDAO = new GameDeckDAO();
        }

        public override void Initialize(BaseCharacterSO characterSO)
        {
            _characterRepo = new PlayerCharacterRepository();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError($"Session {sessionId} does not exist in local PlayerPrefs");
                return;
            }
            _playerDeckData = _deckDAO.GetById(sessionId);

            if (_playerDeckData == null)
            {
                Debug.LogError($"Session {sessionId} does not have deck data");
                return;
            }

            SessionCharacterData _characterData = _characterRepo.GetCharacterData(sessionId, characterSO.CharacterId);

            if (_characterData == null)
            {
                Debug.LogError($"Failed to get character data with id {characterSO.CharacterId} of session {sessionId}");
                return;
            }

            SessionCharacterData runtimeCharData = _characterData.Clone() as SessionCharacterData;
            SessionCharacterData readonlyCharData = _characterData.Clone() as SessionCharacterData;

            CharacterDataHolder characterHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(characterSO)
                                                      .WithCharacterData(runtimeCharData).Build();

            CharacterDataHolder readonlyDataHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(characterSO)
                                                      .WithCharacterData(readonlyCharData).Build();

            SetReadonlyDataHolder(readonlyDataHolder);
            SetCharacterDataHolder(characterHolder, InventoryManager.GetAllRelics());

            InitializeEnergySystem();

            InitializeDeck(_playerDeckData.CardIds);
        }

        private void GenerateSampleDeck()
        {
            _playerDeckData = new SessionPlayerDeck
            {
                CardIds = _test.Ids
            };
        }

        public void SetCombatMode(bool isAuto)
        {
            _isAuto = isAuto;
        }

        /// <summary>
        /// All of enemy brain are handled here.
        /// Step 1: Draw 3 random cards.
        /// Step 2: Pick a card (Randomly or use specific logic)
        /// Step 3
        /// </summary>
        public void DetermineCards(BaseBattleCharacterController opponent, Action<string> onCardSelected)
        {
            StartTurn();

            CombatCardsProcessor.DrawThreeCards(out IReadOnlyList<CardSO> cards);

            if (_selectionStrategy == null)
            {
                Debug.Log("Failed to fetch card selection strategy!");
                return;
            }

            if (_selectionStrategy.TrySelectCard(this, cards, out CardSO card))
            {
                if (CombatCardsProcessor.CalculateCardCost(card) > GetCombatStatus(EStatusType.Stamina).Value)
                {
                    onCardSelected?.Invoke("REST");
                    CombatCardsProcessor.SelectRest();
                }
                else
                {
                    onCardSelected?.Invoke(card.CardId);
                    CombatCardsProcessor.SelectCard(card);
                }

                StartCoroutine(DelayExecute(opponent));
            }
            else
            {
                Debug.LogError("Failed to select card. Choose rest as default!");
                onCardSelected?.Invoke("REST");
                CombatCardsProcessor.SelectRest();
                StartCoroutine(DelayExecute(opponent));
            }
        }

        private IEnumerator DelayExecute(BaseBattleCharacterController opponent)
        {
            yield return new WaitForSeconds(GameConstants.DELAY_PERFORM_ACTION);
            ExecuteCard(opponent);
        }
    }
}