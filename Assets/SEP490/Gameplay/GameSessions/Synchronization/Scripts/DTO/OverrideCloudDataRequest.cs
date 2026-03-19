namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    using SEP490G69.Economy;
    using SEP490G69.Tournament;
    using System.Collections.Generic;

    public class OverrideCloudDataRequest 
    {
        [JsonProperty("player_data")]
        public PlayerInfoDTO PlayerData { get; set; } = new PlayerInfoDTO();

        [JsonProperty("session")]
        public PlayerTrainingSession Session { get; set; } = new PlayerTrainingSession();

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

        [JsonProperty("tournament_progressions")]
        public List<TournamentProgressData> TournamentProgressions { get; set; } = new List<TournamentProgressData>();
    }
}