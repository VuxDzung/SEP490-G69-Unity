using SEP490G69.Economy;
using SEP490G69.Exploration;

namespace SEP490G69.GameSessions
{
    public static class SnapshotModelConverter
    {
        public static SnapshotCharacterData FromEntity2Snapshot(SessionCharacterData character)
        {
            return new SnapshotCharacterData
            {
                Id = character.Id,
                SessionId = character.SessionId,

                CurrentMaxVitality = character.CurrentMaxVitality,
                CurrentPower = character.CurrentPower,
                CurrentIntelligence = character.CurrentIntelligence,
                CurrentStamina = character.CurrentStamina,
                CurrentDef = character.CurrentDef,
                CurrentAgi = character.CurrentAgi,
                CurrentEnergy = character.CurrentEnergy,
                CurrentMood = character.CurrentMood,
                CurrentRP = character.CurrentRP,
            };
        }

        public static SnapshotExerciseData FromEntity2Snapshot(SessionTrainingExercise data)
        {
            return new SnapshotExerciseData
            {
                Id = data.Id,
                SessionId = data.SessionId,
                ExerciseId = data.ExerciseId,
                Level = data.Level
            };
        }

        public static SnapshotDeckData FromEntity2Snapshot(SessionPlayerDeck data)
        {
            return new SnapshotDeckData
            {
                SessionId = data.SessionId,
                CardIds = data.CardIds,
            };
        }

        public static SnapshotCardData FromEntity2Snapshot(SessionCardData data)
        {
            return new SnapshotCardData
            {
                SessionCardId = data.SessionCardId,
                SessionId = data.SessionId,
                RawCardId = data.RawCardId,
                ObtainedAmount = data.ObtainedAmount,
            };
        }

        public static SnapshotInventoryItem FromEntity2Snapshot(ItemData data)
        {
            return new SnapshotInventoryItem
            {
                SessionItemId = data.SessionItemId,
                SessionId = data.SessionId,
                RawItemId = data.RawItemId,
                RemainAmount = data.RemainAmount,
            };
        }

        public static SnapshotShopItem FromEntity2Snapshot(ShopItemData data)
        {
            return new SnapshotShopItem
            {
                SessionItemId = data.SessionItemId,
                SessionId = data.SessionId,
                RawItemId = data.RawItemId,
                RemainAmount = data.RemainAmount,
            };
        }

        public static SnapshotExploreLocationData FromEntity2Snapshot(ExploreLocationData data)
        {
            return new SnapshotExploreLocationData
            {
                EntityId = data.EntityId,
                SessionId = data.SessionId,
                ExplorationCount = data.ExplorationCount,
                IsBossDefeated = data.IsBossDefeated,
            };
        }


        public static SessionCharacterData FromSnapshot2Entity(SnapshotCharacterData character)
        {
            return new SessionCharacterData
            {
                Id = character.Id,
                SessionId = character.SessionId,

                CurrentMaxVitality = character.CurrentMaxVitality,
                CurrentPower = character.CurrentPower,
                CurrentIntelligence = character.CurrentIntelligence,
                CurrentStamina = character.CurrentStamina,
                CurrentDef = character.CurrentDef,
                CurrentAgi = character.CurrentAgi,
                CurrentEnergy = character.CurrentEnergy,
                CurrentMood = character.CurrentMood,
                CurrentRP = character.CurrentRP,
            };
        }
        public static SessionTrainingExercise FromSnapshot2Entity(SnapshotExerciseData data)
        {
            return new SessionTrainingExercise
            {
                Id = data.Id,
                SessionId = data.SessionId,
                ExerciseId = data.ExerciseId,
                Level = data.Level
            };
        }
        public static SessionPlayerDeck FromSnapshot2Entity(SnapshotDeckData data)
        {
            return new SessionPlayerDeck
            {
                SessionId = data.SessionId,
                CardIds = data.CardIds,
            };
        }
        public static SessionCardData FromSnapshot2Entity(SnapshotCardData data)
        {
            return new SessionCardData
            {
                SessionCardId = data.SessionCardId,
                SessionId = data.SessionId,
                RawCardId = data.RawCardId,
                ObtainedAmount = data.ObtainedAmount,
            };
        }
        public static ItemData FromSnapshot2Entity(SnapshotInventoryItem data)
        {
            return new ItemData
            {
                SessionItemId = data.SessionItemId,
                SessionId = data.SessionId,
                RawItemId = data.RawItemId,
                RemainAmount = data.RemainAmount,
            };
        }
        public static ShopItemData FromSnapshot2Entity(SnapshotShopItem data)
        {
            return new ShopItemData
            {
                SessionItemId = data.SessionItemId,
                SessionId = data.SessionId,
                RawItemId = data.RawItemId,
                RemainAmount = data.RemainAmount,
            };
        }
        public static ExploreLocationData FromSnapshot2Entity(SnapshotExploreLocationData data)
        {
            return new ExploreLocationData
            {
                EntityId = data.EntityId,
                SessionId = data.SessionId,
                ExplorationCount = data.ExplorationCount,
                IsBossDefeated = data.IsBossDefeated,
            };
        }
    }
}
