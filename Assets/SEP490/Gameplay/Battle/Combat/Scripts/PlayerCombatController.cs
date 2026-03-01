namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using UnityEngine;

    public class PlayerCombatController : CharacterCombatController
    {
        private CharacterDataHolder _characterHolder;

        private PlayerCharacterRepository _characterRepo;

        private SessionPlayerDeck _playerDeckData;

        public override void Initialize(string characterId)
        {
            _characterRepo = new PlayerCharacterRepository(LocalDBInitiator.GetDatabase());

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError($"Session {sessionId} does not exist in local PlayerPrefs");
                return;
            }

            SessionCharacterData _characterData = _characterRepo.GetCharacterData(sessionId, characterId);

            if (_characterData == null)
            {
                Debug.LogError($"Failed to get character data with id {characterId} of session {sessionId}");
                return;
            }

            _characterData = _characterData.Clone() as SessionCharacterData;
            BaseCharacterSO _characterSO = CharacterConfig.GetCharacterById(characterId);

            if ( _characterSO == null)
            {
                Debug.LogError($"CharacterSO data of charater {characterId} has not been cofigured yet!");
                return;
            }

            _characterHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(_characterSO)
                                                      .WithCharacterData(_characterData).Build();
        }
    }
}