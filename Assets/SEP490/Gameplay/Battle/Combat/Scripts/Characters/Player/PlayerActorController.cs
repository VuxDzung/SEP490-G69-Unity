namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Tournament;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerActorController : BaseCombatActor
    {
        private ICombatCardsService _cardsService;

        private PlayerCharacterRepository _characterRepo;
        private GameDeckDAO _deckDAO;

        private SessionPlayerDeck _playerDeckData;
        private TestObtainedCardSO _test;

        private PlayerCharacterDataSO _playerSO;

        public PlayerCharacterDataSO CharacterSO
        {
            get
            {
                if (_playerSO == null)
                {
                    _playerSO = _baseDataSO.ConvertAs<PlayerCharacterDataSO>();
                }
                return _playerSO;
            }
        }
        public ICombatCardsService CardsService => _cardsService;

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

            _cardsService = GetComponent<ICombatCardsService>();

            if (_cardsService == null)
            {
                _cardsService = gameObject.AddComponent<CharacterCardsService>();
                if (_cardsService == null)
                {
                    Debug.LogError($"[BaseBattleCharacterController.Awake error] Failed to get the {nameof(ICombatCardsService)} in {gameObject.name}");
                    return;
                }
            }

            _cardsService.SetOwner(this);
        }

        public override void Initialize(BaseCharacterSO characterSO, SceneCombatController battleController)
        {
            _baseDataSO = characterSO;
            _playerSO = _baseDataSO.ConvertAs<PlayerCharacterDataSO>();

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

            InitializeDeck(_playerDeckData.CardIds);
            CreateStatsByProfile(new PlayerStatProfile());
            ICombatStatsInitializer initializer = new PlayerStatsInitializer(InventoryManager.GetAllRelics(), characterHolder);
            SetInitializer(initializer);
            InitializeStats();
        }

        public void SetCombatMode(bool isAuto)
        {
            _isAuto = isAuto;
        }

        #region Card APIs
        public bool IsCardUsable(CardSO card, SceneCombatController controller)
        {
            return _cardsService.IsCardUsable(card, controller);
        }

        public void InitializeDeck(string[] deckCardIdArray)
        {
            _cardsService.InitializeDeck(deckCardIdArray);
        }

        public float CalculateSelectedCardDmg(bool writeToOutputDmg)
        {
            return _cardsService.CalculateSelectedCardDmg(writeToOutputDmg);
        }

        public float CalculateBaseCardDMG(CardSO card)
        {
            return _cardsService.CalculateBaseDmg(card);
        }

        public void ExecuteCard(BaseCombatActor target)
        {
            TriggerTurnFlowEvent(ETurnFlowEvent.BeforeCardAction);
            EffectsManager.Trigger(ETurnFlowEvent.BeforeCardAction, target);

            if (!_cardsService.ExecuteCard(target))
            {
                TriggerAfterCardResolved(target);
            }
        }

        public void DrawCards(out IReadOnlyList<CardSO> cards)
        {
            _cardsService.DrawCards(5, out cards);
        }

        public void SelectRest()
        {
            _cardsService.SelectRest();
        }

        public void SelectCardById(string cardId)
        {
            _cardsService.SelectCardById(cardId);
        }

        public void SelectNoAction()
        {
            _cardsService.SelectNoAction();
        }
        #endregion

        #region Shield
        public void StackShield(float baseValue, float modifierValue)
        {
            StackShield(EStatusType.Vitality, baseValue, modifierValue);
        }

        #endregion

        public override void EndTurn()
        {
            // Discard cards.
            _cardsService.DiscardCurrentDraw();

            base.EndTurn();
        }
    }
}