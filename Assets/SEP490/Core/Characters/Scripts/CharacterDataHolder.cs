namespace SEP490G69
{
    using SEP490G69.Addons.Localization;
    using UnityEngine;

    public class CharacterDataHolder 
    {
        private CharacterData _data;
        private CharacterDataSO _so;

        private LocalizationManager _localization;

        public CharacterDataHolder(CharacterData data, CharacterDataSO sO, LocalizationManager localization)
        {
            _data = data;
            _so = sO;
            _localization = localization;
        }

        public string GetCharacterDescription()
        {
            if (_localization == null) return string.Empty;

            return _localization.GetText(GameConstants.LOCALIZE_CATEGORY_CHARACTER_DESCS, _so.CharacterID);
        }
    }
}