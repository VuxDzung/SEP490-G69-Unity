namespace SEP490G69
{
    public class GameConstants 
    {
        #region UI Frame Id
        /// <summary>
        /// Frame id for dialog UI frame.
        /// </summary>
        public const string FRAME_ID_DIALOG = "Frame.Dialog";
        public const string FRAME_ID_LOGIN = "Frame.Login";
        public const string FRAME_ID_REGISTER = "Frame.Register";
        public const string FRAME_ID_TITLE = "Frame.Title";
        public const string FRAME_ID_CREDIT = "Frame.Credit";
        public const string FRAME_ID_TITLE_SETTINGS = "Frame.TitleSettings";
        public const string FRAME_ID_MESSAGE_POPUP = "Frame.MessagePopup";
        public const string FRAME_ID_CHANGE_PW = "Frame.ChangePassword";
        public const string FRAME_ID_SET_LANG = "Frame.SetLanguage";
        public const string FRAME_ID_SET_NAME = "Frame.SetPlayerName";
        public const string FRAME_ID_CHAR_SELECT = "Frame.CharacterSelection";

        #endregion

        #region Scene names
        public const string SCENE_AUTH = "Scene.Auth";
        public const string SCENE_TITLE = "Scene.Title";
        public const string SCENE_MAIN_MENU = "Scene.MainMenu";

        #endregion

        #region Prefs keys
        public const string PREF_KEY_CURRENT_SESSION_ID = "CurrentSessionId";
        #endregion

        #region Localization categories

        /// <summary>
        /// Localize category for button's texts.
        /// </summary>
        public const string LOCALIZE_CATEGORY_BUTTON_TEXT = "UIButtonText";

        public const string LOCALIZE_UI_MESSAGE = "UIMessage";

        /// <summary>
        /// Localize category for dialogs
        /// </summary>
        public const string LOCALIZE_CATEGORY_DIALOG = "Dialogs";

        /// <summary>
        /// Localize category for dialog' choices.
        /// </summary>
        public const string LOCALIZE_CATEGORY_DIALOG_CHOICE = "DialogChoices";
        
        /// <summary>
        /// Localize category for item's name
        /// </summary>
        public const string LOCALIZE_CATEGORY_ITEM_NAMES = "ItemNames";

        /// <summary>
        /// Localize category for item's description
        /// </summary>
        public const string LOCALIZE_CATEGORY_ITEM_DESC = "ItemDescs";

        /// <summary>
        /// Localize category for character's descriptions.
        /// </summary>
        public const string LOCALIZE_CATEGORY_CHARACTER_DESCS = "CharacterDescs";
        #endregion

        #region Currencies id
        /// <summary>
        /// Id for gold currency
        /// </summary>
        public const string MONEY_ID_GOLD = "gold";

        /// <summary>
        /// Id for gem currency
        /// </summary>
        public const string MONEY_ID_GEM = "gem";
        #endregion

        #region Narrative Actions
        public const string ACTION_FADE_IN = "FADE_IN";
        public const string ACTION_FADE_OUT = "FADE_OUT";
        public const string ACTION_FADE_IN_2_OUT = "FADE_IN_2_OUT";
        #endregion

        #region Narrative Event parameters
        public const string PARAM_FADE_TIME = "fadeTime";
        public const string PARAM_IN_FADE_TIME = "inFadeTime";
        #endregion

        public const int TRAINING_STARTER_LEVEL = 1;
    }
}