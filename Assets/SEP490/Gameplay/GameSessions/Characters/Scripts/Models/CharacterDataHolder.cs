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
        public Sprite GetFullBodyImg()
        {
            return _characterSO.FullBodyImg;
        }
        public string GetCharacterName()
        {
            return _characterSO.CharacterName;
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

        public void AddEnergy(float finalGain)
        {
            _characterData.CurrentEnergy += finalGain;
            _characterData.CurrentEnergy = Mathf.Clamp(_characterData.CurrentEnergy, 0, GameConstants.MAX_100);
        }

        public void AddMood(float finalGain)
        {
            _characterData.CurrentMood += finalGain;
            _characterData.CurrentMood = Mathf.Clamp(_characterData.CurrentMood, 0, GameConstants.MAX_100);
        }

        public void AddVit(float finalGain)
        {
            _characterData.CurrentMaxVitality += finalGain;
            _characterData.CurrentMaxVitality = Mathf.Clamp(_characterData.CurrentMaxVitality, 0, GameConstants.MAX_STAT_VALUE);
        }
        public void AddPower(float finalGain)
        {
            _characterData.CurrentPower += finalGain;
            _characterData.CurrentPower = Mathf.Clamp(_characterData.CurrentPower, 0, GameConstants.MAX_STAT_VALUE);
        }
        public void AddAgi(float finalGain)
        {
            _characterData.CurrentAgi += finalGain;
            _characterData.CurrentAgi = Mathf.Clamp(_characterData.CurrentAgi, 0, GameConstants.MAX_STAT_VALUE);
        }
        public void AddStamina(float finalGain)
        {
            _characterData.CurrentStamina += finalGain;
            _characterData.CurrentStamina = Mathf.Clamp(_characterData.CurrentStamina, 0, GameConstants.MAX_STAT_VALUE);
        }
        public void AddInt (float finalGain)
        {
            _characterData.CurrentIntelligence += finalGain;
            _characterData.CurrentIntelligence = Mathf.Clamp(_characterData.CurrentIntelligence, 0, GameConstants.MAX_STAT_VALUE);
        }
        public void AddDef(float finalGain)
        {
            _characterData.CurrentDef += finalGain;
            _characterData.CurrentDef = Mathf.Clamp(_characterData.CurrentDef, 0, GameConstants.MAX_STAT_VALUE);
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