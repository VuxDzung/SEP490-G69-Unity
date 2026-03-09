namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using UnityEngine;

    public class PlayerBattleCharaterController : BaseBattleCharacterController
    {
        private PlayerCharacterRepository _characterRepo;
        private GameDeckDAO _deckDAO;

        private SessionPlayerDeck _playerDeckData;
        private TestObtainedCardSO _test;
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
            _characterRepo = new PlayerCharacterRepository(LocalDBInitiator.GetDatabase());

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
            SetCharacterDataHolder(characterHolder);

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
        private void Update()
        {
            // testing
            // Decrease health.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //CurrentDataHolder.ModifyStat(EStatusType.Vitality, -20);
                //Debug.Log($"Curren health: {CurrentDataHolder.GetStatus(EStatusType.Vitality)}");
            }
        }
    }
}