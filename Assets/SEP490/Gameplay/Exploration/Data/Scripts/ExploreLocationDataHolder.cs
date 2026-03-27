namespace SEP490G69.Exploration
{
    public class ExploreLocationDataHolder
    {
        private ExplorationSO _dataSO;
        private ExploreLocationData _data;

        public string GetEntityId() => _data != null ? _data.EntityId : string.Empty;
        public string GetRawId() => _dataSO != null ? _dataSO.ExplorationId : string.Empty;

        public string GetLocationName() => _dataSO != null ? _dataSO.LocationName : string.Empty;
        public string GetBossId() => _dataSO != null ? _dataSO.BossId : string.Empty;
        public EDifficulty GetDifficulty() => _dataSO != null ? _dataSO.Difficulty : EDifficulty.Easy;

        public class Builder
        {
            private ExplorationSO dataSO;
            private ExploreLocationData data;

            public Builder WithData(ExploreLocationData data)
            {
                this.data = data;
                return this;
            }
            public Builder WithSO(ExplorationSO so)
            {
                this.dataSO = so;
                return this;
            }
            public ExploreLocationDataHolder Build()
            {
                return new ExploreLocationDataHolder
                {
                    _data = data,
                    _dataSO = dataSO
                };
            }
        }
    }
}