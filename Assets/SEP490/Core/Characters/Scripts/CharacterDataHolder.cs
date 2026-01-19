namespace SEP490G69
{
    using SEP490G69.Addons.Localization;
    using UnityEngine;

    public class CharacterDataHolder 
    {
        private CharacterData _data;
        private CharacterDataSO _so;

        private LocalizationManager _localization;

        public string GetCharacterDescription()
        {
            if (_localization == null) return string.Empty;

            return _localization.GetText(GameConstants.LOCALIZE_CATEGORY_CHARACTER_DESCS, _so.CharacterID);
        }

        public class Builder
        {
            private CharacterData _runtimeData;
            private CharacterDataSO _soData;
            private LocalizationManager _localization;

            public Builder WithRuntimeData(CharacterData runtimeData)
            {
                _runtimeData = runtimeData;
                return this;
            }
            public Builder WithSoData(CharacterDataSO soData)
            {
                _soData = soData;
                return this;
            }
            public Builder WithLocalization(LocalizationManager localization)
            {
                _localization = localization;
                return this;
            }
            public CharacterDataHolder Build()
            {
                return new CharacterDataHolder
                {
                    _data = _runtimeData,
                    _so = _soData,
                    _localization = _localization
                };
            }
        }
    }
}