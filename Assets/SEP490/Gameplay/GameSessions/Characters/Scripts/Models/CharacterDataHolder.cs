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

        public float GetVIT()
        {
            return _characterData.CurrentMaxVitality;
        }
        public float GetPower()
        {
            return _characterData.CurrentPower;
        }
        public float GetINT()
        {
            return _characterData.CurrentIntelligence;
        }
        public float GetStamina()
        {
            return _characterData.CurrentStamina;
        }
        public float GetDef()
        {
            return _characterData.CurrentDef;
        }
        public float GetAgi()
        {
            return _characterData.CurrentAgi;
        }

        public int GetRP()
        {
            return _characterData.CurrentRP;
        }

        public GameObject GetPrefab()
        {
            return _characterSO.Prefab;
        }

        public void ApplyMoodModifier(StatusModifierSO modifierSO)
        {
            if (modifierSO.StatType != EStatusType.Mood) return;

            _characterData.CurrentMood = modifierSO.GetModifierValue(_characterData.CurrentMood);
            _characterData.CurrentMood = Mathf.Clamp(_characterData.CurrentMood, 0, GameConstants.MAX_100);
        }
        public void ApplyEnergyModifier(StatusModifierSO modifierSO)
        {
            if (modifierSO.StatType != EStatusType.Energy) return;

            _characterData.CurrentEnergy = modifierSO.GetModifierValue(_characterData.CurrentEnergy);
            _characterData.CurrentEnergy = Mathf.Clamp(_characterData.CurrentEnergy, 0, GameConstants.MAX_100);
        }
        public void ApplyPowerModifier(StatusModifierSO modifierSO)
        {
            if (modifierSO.StatType != EStatusType.Power) return;

            _characterData.CurrentPower = modifierSO.GetModifierValue(_characterData.CurrentPower);
            _characterData.CurrentPower = Mathf.Clamp(_characterData.CurrentPower, 0, GameConstants.MAX_STAT_VALUE);
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