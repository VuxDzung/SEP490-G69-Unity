namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using UnityEngine;

    public class PlayerBattleCharaterController : BaseBattleCharacterController
    {
        private PlayerCharacterRepository _characterRepo;

        private SessionPlayerDeck _playerDeckData;

        public override void Initialize(BaseCharacterSO characterSO)
        {
            _characterRepo = new PlayerCharacterRepository(LocalDBInitiator.GetDatabase());

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError($"Session {sessionId} does not exist in local PlayerPrefs");
                return;
            }

            SessionCharacterData _characterData = _characterRepo.GetCharacterData(sessionId, characterSO.CharacterId);

            if (_characterData == null)
            {
                Debug.LogError($"Failed to get character data with id {characterSO.CharacterId} of session {sessionId}");
                return;
            }

            _characterData = _characterData.Clone() as SessionCharacterData;

            CharacterDataHolder _characterHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(characterSO)
                                                      .WithCharacterData(_characterData).Build();

            CharacterDataHolder readonlyDataHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(characterSO)
                                                      .WithCharacterData(_characterData).Build();
            SetReadonlyDataHolder(readonlyDataHolder);
            SetCharacterDataHolder(_characterHolder);
            InitializeEnergySystem();
        }
    }
}