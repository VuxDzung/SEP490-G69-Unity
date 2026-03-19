namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class OverrideCloudDataRequest
    {
        [JsonProperty("player_data")]
        public PlayerInfoDTO PlayerData { get; set; } = new PlayerInfoDTO();

        [JsonProperty("session")]
        public PlayerTrainingSessionDTO Session { get; set; } = new PlayerTrainingSessionDTO();

        [JsonProperty("character")]
        public SessionCharacterDataDTO Character { get; set; } = new SessionCharacterDataDTO();

        [JsonProperty("exercises")]
        public List<SessionTrainingExerciseDTO> Exercises { get; set; } = new List<SessionTrainingExerciseDTO>();

        [JsonProperty("deck")]
        public SessionPlayerDeckDTO Deck { get; set; } = new SessionPlayerDeckDTO();

        [JsonProperty("cards")]
        public List<SessionCardDataDTO> Cards { get; set; } = new List<SessionCardDataDTO>();


        [JsonProperty("obtained_items")]
        public List<ItemDataDTO> ObtainedItems { get; set; } = new List<ItemDataDTO>();

        [JsonProperty("shop_items")]
        public List<ShopItemDataDTO> ShopItems { get; set; } = new List<ShopItemDataDTO>();

        [JsonProperty("tournament_progressions")]
        public List<TournamentProgressDataDTO> TournamentProgressions { get; set; } = new List<TournamentProgressDataDTO>();
    }
}