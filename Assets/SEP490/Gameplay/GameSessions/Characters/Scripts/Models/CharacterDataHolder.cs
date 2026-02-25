namespace SEP490G69
{
    using UnityEngine;

    public class CharacterDataHolder 
    {
        private BaseCharacterSO _characterSO;
        private SessionCharacterData _characterData;

        public Sprite GetAvatar()
        {
            return _characterSO.Thumbnail;
        }

        public float GetEnergy()
        {
            return _characterData.CurrentEnergy;
        }
        public float GetMood()
        {
            return (float)_characterData.CurrentMood;
        }

        public GameObject GetPrefab()
        {
            return _characterSO.Prefab;
        }

        public void ApplyMoodModifier(StatusModifierSO modifierSO)
        {
            if (modifierSO.StatType != EStatusType.Mood) return;

            _characterData.CurrentMood = modifierSO.GetModifierValue(_characterData.CurrentMood);
        }
        public void ApplyEnergyModifier(StatusModifierSO modifierSO)
        {
            if (modifierSO.StatType != EStatusType.Energy) return;

            _characterData.CurrentEnergy = modifierSO.GetModifierValue(_characterData.CurrentEnergy);
        }
        public void ApplyPowerModifier(StatusModifierSO modifierSO)
        {
            if (modifierSO.StatType != EStatusType.Power) return;

            _characterData.CurrentPower = modifierSO.GetModifierValue(_characterData.CurrentPower);
        }

        public class Builder
        {
            private BaseCharacterSO _so;
            private SessionCharacterData _data;

            public Builder WithCharacterSO(BaseCharacterSO characterSO)
            {
                _so = characterSO;
                return this;
            }
            public Builder WithCharacterData(SessionCharacterData characterData)
            {
                _data = characterData;
                return this;
            }
            public CharacterDataHolder Build()
            {
                CharacterDataHolder holder = new CharacterDataHolder
                {
                    _characterData = _data,
                    _characterSO = _so,
                };
                return holder;
            }
        }
    }
}