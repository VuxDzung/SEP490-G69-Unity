namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using UnityEngine;

    public class EnemyCombatController : CharacterCombatController
    {
        private CharacterDataHolder _characterHolder;

        /// <summary>
        /// Because the enemy only exists in battle, the system must manually create CharacterDataHolder for the enemy by itself.
        /// </summary>
        /// <param name="characterId"></param>
        public override void Initialize(string characterId)
        {
            BaseCharacterSO _characterSO = CharacterConfig.GetCharacter(characterId);
            SessionCharacterData _characterData = new SessionCharacterData();
            _characterData.Id = characterId;
            _characterData.CurrentMaxVitality = _characterSO.BaseVit;
            _characterData.CurrentPower = _characterSO.BasePow;
            _characterData.CurrentIntelligence = _characterSO.BaseInt;
            _characterData.CurrentStamina = _characterSO.BaseSta;
            _characterData.CurrentDef = _characterSO.BaseDef;
            _characterData.CurrentAgi = _characterSO.BaseAgi;

            _characterHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(_characterSO)
                                                      .WithCharacterData(_characterData)
                                                      .Build();
        }
    }
}