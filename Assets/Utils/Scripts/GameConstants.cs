using SEP490G69.Addons.Localization.Enums;
using System.Collections.Generic;
using UnityEngine;

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
        public const string FRAME_ID_TRAINING_MENU = "Frame.TrainingMenu";
        public const string FRAME_ID_MAIN_MENU = "Frame.MainMenu";
        public const string FRAME_ID_CHAR_DETAILS = "Frame.CharacterDetail";
        public const string FRAME_ID_CALENDAR = "Frame.Calendar";
        public const string FRAME_ID_PLAYER_PROFILE = "Frame.PlayerProfile";
        public const string FRAME_ID_IN_GAME_SETTINGS = "Frame.InGameSettings";
        public const string FRAME_ID_COMBAT = "Frame.Combat";
        public const string FRAME_ID_COMBAT_DETAILS = "Frame.CombatDetails";
        public const string FRAME_ID_TRAINING_RESULT = "Frame.TrainingResult";
        public const string FRAME_ID_TOURNAMENT_BRACKET = "Frame.TournamentBracket";
        public const string FRAME_ID_STAT_EFFECT_DETAILS = "Frame.StatEffectDetails";

        public const string FRAME_ID_INVENTORY = "Frame.Inventory";
        public const string FRAME_ID_INVENTORY_ITEM_DETAILS = "Frame.InventoryItemDetails";

        public const string FRAME_ID_SHOP = "Frame.Shop";

        public const string FRAME_ID_TOURNAMENT_DETAILS = "Frame.TournamentDetails";

        public const string FRAME_ID_EDIT_DECK = "Frame.EditDeck";
        public const string FRAME_ID_CARD_DETAILS = "Frame.CardDetails";
        #endregion

        #region Scene names
        public const string SCENE_AUTH = "Scene.Auth";
        public const string SCENE_TITLE = "Scene.Title";
        public const string SCENE_MAIN_MENU = "Scene.MainMenu";
        public const string SCENE_COMBAT = "Scene.Combat";
        public const string SCENE_TOURNAMENT = "Scene.Tournament";
        public const string SCENE_DECK = "Scene.Deck";

        #endregion

        #region Prefs keys
        public const string PREF_KEY_CURRENT_SESSION_ID = "CurrentSessionId";
        public const string PREF_KEY_PLAYER_ID = "PlayerId";
        public const string PREF_KEY_AUTH_ACTION = "AuthAction";

        public const string PREF_KEY_TOURNAMENT_ID = "tournament_id";
        public const string PREF_KEY_TOURNAMENT_ENEMY_ID = "tournament_enemy_id";
        public const string PREF_KEY_COMBAT_TYPE = "combat_type";
        public const string PREF_KEY_TOURNAMENT_PLAYER_WIN = "tournament_player_win";
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

        public const string LOCALIZE_CATEGORY_CARD_NAMES = "CardNames";

        public const string LOCALIZE_CATEGORY_CARD_DESCS = "CardDescs";

        public const string LOCALIZE_CATEGORY_TOOL_TIPS = "Tooltips";

        public const string LOCALIZE_CATEGORY_EXERCISE_NAMES = "ExerciseNames";

        public const string LOCALIZE_CATEGORY_UI_MESSAGE = "UIMessage";

        public const string LOCALIZE_CATEGORY_UI_TEXT = "UIText";

        public const string LOCALIZE_CATEGORY_MONTH_NAMES = "MonthNames";

        public const string LOCALIZE_CATEGORY_TOUR_NAMES = "TournamentNames";
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

        #region Pool names
        public const string POOL_UI_STATUS_EFFECT_DETAILS = "UIStatEffectDetails";
        public const string POOL_UI_CARD = "UICard";
        public const string POOL_UI_INVENTORY_ITEM = "UIInventoryItem";
        public const string POOL_UI_SHOP_ITEM = "UIShopItem";
        #endregion

        public const string COMBAT_TYPE_TOURNAMENT = "Tournament";
        public const string COMBAT_TYPE_EXPLORATION = "Exploration";
        public const int TRAINING_STARTER_LEVEL = 1;
        public const int MAX_100 = 100;
        public const float BASE_FILL_SPEED = 50f;
        public const string MOOD_FORMAT = "Mood: {0}";
        public const float DEFAULT_CAM_ORTH_SIZE = 5f;

        public const int STARTER_MONEY_AMOUNT = 100000000;

        public static readonly int[] RP_CHECKPOINTS =
        {
            200,
            500,
            1000,
        };

        public static readonly List<ELocalizeLanguageType> LANGUAGES = new List<ELocalizeLanguageType>
        {
            ELocalizeLanguageType.English,
            ELocalizeLanguageType.Vietnamese,
        };

        public static int DetermineNextRPCheckpoint(int currentPoint)
        {
            for (int i = 0; i < RP_CHECKPOINTS.Length; i++)
            {
                if (RP_CHECKPOINTS[i] > currentPoint)
                {
                    return RP_CHECKPOINTS[i];
                }
            }
            return RP_CHECKPOINTS[RP_CHECKPOINTS.Length - 1];
        }

        public static readonly int[] FPS_LIMITS = new int[]
        {
            30,
            60,
            120,
            144,
            240,
            -1 
        };

        #region UI Rect Pivot Constants
        public static readonly Vector2 RECT_ANCHOR_TOP_LEFT = new Vector2(0, 1);
        public static readonly Vector2 RECT_ANCHOR_TOP_CENTER = new Vector2(0.5f, 1);
        public static readonly Vector2 RECT_ANCHOR_TOP_RIGHT = new Vector2(1, 1);

        public static readonly Vector2 RECT_ANCHOR_MIDDLE_LEFT = new Vector2(0f, 0.5f);
        public static readonly Vector2 RECT_ANCHOR_MIDDLE_CENTER = new Vector2(0.5f, 0.5f);
        public static readonly Vector2 RECT_ANCHOR_MIDDLE_RIGHT = new Vector2(1f, 0.5f);

        public static readonly Vector2 RECT_ANCHOR_BOTTOM_LEFT = new Vector2(0f, 0f);
        public static readonly Vector2 RECT_ANCHOR_BOTTOM_CENTER = new Vector2(0.5f, 0f);
        public static readonly Vector2 RECT_ANCHOR_BOTTOM_RIGHT = new Vector2(1f, 0f);

        public static Vector2 GetRectValue(ERectPivot pivot)
        {
            switch (pivot)
            {
                case ERectPivot.TopLeft:
                    return RECT_ANCHOR_TOP_LEFT;
                case ERectPivot.TopCenter:
                    return RECT_ANCHOR_TOP_CENTER;
                case ERectPivot.TopRight:
                    return RECT_ANCHOR_TOP_RIGHT;
                case ERectPivot.MiddleLeft:
                    return RECT_ANCHOR_MIDDLE_LEFT;
                case ERectPivot.MiddleCenter:
                    return RECT_ANCHOR_MIDDLE_CENTER;
                case ERectPivot.MiddleRight:
                    return RECT_ANCHOR_MIDDLE_RIGHT;
                case ERectPivot.BottomLef:
                    return RECT_ANCHOR_BOTTOM_LEFT;
                case ERectPivot.BottomCenter:
                    return RECT_ANCHOR_BOTTOM_CENTER;
                case ERectPivot.BottomRight:
                    return RECT_ANCHOR_BOTTOM_RIGHT;
                default:
                    return new Vector2(0f, 0f);
            }
        }
        #endregion
    }
    public enum ERectPivot
    {
        TopLeft, TopCenter, TopRight, MiddleLeft, MiddleRight, MiddleCenter, BottomLef, BottomCenter, BottomRight
    }
}