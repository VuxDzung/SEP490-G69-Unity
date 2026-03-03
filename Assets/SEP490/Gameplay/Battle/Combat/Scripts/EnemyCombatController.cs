namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using UnityEngine;

    public class EnemyCombatController : BaseBattleCharacterController
    {
        private CharacterDataHolder _characterHolder;

        /// <summary>
        /// Because the enemy only exists in battle, the system must manually create CharacterDataHolder for the enemy by itself.
        /// </summary>
        /// <param name="characterId"></param>
        public override void Initialize(BaseCharacterSO characterSO)
        {
            SessionCharacterData _characterData = new SessionCharacterData();
            _characterData.Id = characterSO.CharacterId;
            _characterData.CurrentMaxVitality = characterSO.BaseVit;
            _characterData.CurrentPower = characterSO.BasePow;
            _characterData.CurrentIntelligence = characterSO.BaseInt;
            _characterData.CurrentStamina = characterSO.BaseSta;
            _characterData.CurrentDef = characterSO.BaseDef;
            _characterData.CurrentAgi = characterSO.BaseAgi;

            _characterHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(characterSO)
                                                      .WithCharacterData(_characterData)
                                                      .Build();

            CharacterDataHolder readonlyDataHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(characterSO)
                                                      .WithCharacterData(_characterData).Build();
            SetReadonlyDataHolder(readonlyDataHolder);
            SetCharacterDataHolder(_characterHolder);
            InitializeEnergySystem();
        }
    }
}