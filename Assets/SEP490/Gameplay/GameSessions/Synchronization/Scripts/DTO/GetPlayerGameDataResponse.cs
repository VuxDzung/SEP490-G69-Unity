namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    using SEP490G69.Economy;
    using SEP490G69.Tournament;
    using System;
    using System.Collections.Generic;

    public class GetPlayerGameDataResponse
    {
        #region Response handler
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; } = string.Empty;

        #endregion

        #region Player info
        [JsonProperty("player_id")]
        public string PlayerId { get; set; } = string.Empty;
        [JsonProperty("player_name")]
        public string PlayerName { get; set; } = string.Empty;
        [JsonProperty("legacy_points")]
        public int LegacyPoints { get; set; }
        [JsonProperty("last_sync_time")]
        public DateTime LastSyncedTime { get; set; }
        [JsonProperty("current_run")]
        public int CurrentRun { get; set; }
        #endregion

        #region Game data
        [JsonProperty("session")]
        public PlayerTrainingSession Session { get; set; }

        [JsonProperty("character")]
        public SessionCharacterData Character { get; set; } = new SessionCharacterData();

        [JsonProperty("exercises")]
        public List<SessionTrainingExercise> Exercises { get; set; } = new List<SessionTrainingExercise>();

        [JsonProperty("deck")]
        public SessionPlayerDeck Deck { get; set; } = new SessionPlayerDeck();

        [JsonProperty("cards")]
        public List<SessionCardData> Cards { get; set; } = new List<SessionCardData>();


        [JsonProperty("obtained_items")]
        public List<ItemData> ObtainedItems { get; set; } = new List<ItemData>();

        [JsonProperty("shop_items")]
        public List<ShopItemData> ShopItems { get; set; } = new List<ShopItemData>();

        [JsonProperty("tournaments")]
        public List<TournamentProgressData> TournamentProgressions { get; set; } = new List<TournamentProgressData>();

        #endregion
    }
}