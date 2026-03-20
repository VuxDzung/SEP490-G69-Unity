namespace SEP490G69
{
    using LiteDB;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Economy;
    using SEP490G69.Legacy;
    using SEP490G69.Tournament;
    using SEP490G69.Training;
    using System;
    using System.IO;
    using UnityEngine;

    public class LocalDBOrchestrator : GlobalSingleton<LocalDBOrchestrator>
    {
        private static readonly object _dbLock = new object();

        private GameAuthManager _authManager;
        private GameAuthManager AuthManager
        {
            get
            {
                if (_authManager == null)
                {
                    _authManager = ContextManager.Singleton.ResolveGameContext<GameAuthManager>();
                }
                return _authManager;
            }
        }

        protected override void CreateNewInstance()
        {
            base.CreateNewInstance();
            MappingDBIndexs();
        }

        private void MappingDBIndexs()
        {
            Execute(db =>
            {
                var charactersCol = db.GetCollection<SessionCharacterData>(PlayerCharacterDAO.COLLECTION_NAME);
                charactersCol.EnsureIndex(x => x.SessionId);
            });

            Execute(db =>
            {
                var exercisesCol = db.GetCollection<SessionTrainingExercise>(TrainingExerciseDAO.COLLECTION_NAME);
                exercisesCol.EnsureIndex(x => x.SessionId);
            });

            Execute(db =>
            {
                var itemsCol = db.GetCollection<ItemData>(GameInventoryDAO.COLLECTION_NAME);
                itemsCol.EnsureIndex(x => x.SessionId);
            });

            Execute(db =>
            {
                var itemsCol = db.GetCollection<ShopItemData>(GameShopDAO.COLLECTION_NAME);
                itemsCol.EnsureIndex(x => x.SessionId);
            });

            Execute(db =>
            {
                var cardsCol = db.GetCollection<SessionCardData>(GameCardsDAO.COLLECTION_NAME);
                cardsCol.EnsureIndex(x => x.SessionId);
                cardsCol.EnsureIndex(x => x.RawCardId);
            });

            Execute(db =>
            {
                var tournamentsCol = db.GetCollection<TournamentProgressData>(TournamentProgressDAO.COLLECTION_NAME);
                tournamentsCol.EnsureIndex(x => x.SessionId);
            });

            Execute(db =>
            {
                var legaciesCol = db.GetCollection<LegacyStatData>(GameLegacyDAO.COLLECTION_NAME);
                legaciesCol.EnsureIndex(x => x.PlayerId);
            });

            Execute(db =>
            {
                var supportItemsCol = db.GetCollection<TrainingSupportItem>(TrainingSupportItemsDAO.COLLECTION_NAME);
                supportItemsCol.EnsureIndex(x => x.SessionId);
            });
        }

        public static T Execute<T>(Func<LiteDatabase, T> action)
        {
            lock (_dbLock)
            {
                using (var db = GetDatabase())
                {
                    return action(db);
                }
            }
        }

        public static void Execute(Action<LiteDatabase> action)
        {
            lock (_dbLock)
            {
                using (var db = GetDatabase())
                {
                    action(db);
                }
            }
        }

        /// <summary>
        /// Get a new database instance
        /// </summary>
        /// <returns></returns>
        public static LiteDatabase GetDatabase()
        {
            string folderPath = Path.Combine(Application.persistentDataPath, "database");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string dbPath = Path.Combine(folderPath, "GameData.db");

            return new LiteDatabase(dbPath);
        }

        public static bool UpdateDBChangeTime()
        {
            return Execute(db => UpdateDBChangeTime(db));
        }

        public static bool UpdateDBChangeTime(LiteDatabase db)
        {
            string playerId = Singleton.AuthManager.GetUserId();

            try
            {
                if (string.IsNullOrEmpty(playerId))
                {
                    Debug.LogError("[LocalDBOrchestrator.UpdateDBChangeTime] Player id is empty");
                    return false;
                }

                var playerCol = db.GetCollection<PlayerData>(PlayerDataDAO.COLLECTION_NAME);

                var playerData = playerCol.FindById(playerId);

                if (playerData == null)
                {
                    Debug.LogError($"[LocalDBOrchestrator.UpdateDBChangeTime] Player data with id {playerId} not found");
                    return false;
                }

                playerData.LastUpdatedTime = DateTime.UtcNow;

                bool result = playerCol.Update(playerData);

                if (result)
                {
                    Debug.Log($"<color=green>[LocalDBOrchestrator.UpdateDBChangeTime]</color> Updated time: {playerData.LastUpdatedTime}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError("[LocalDBOrchestrator.UpdateDBChangeTime] Exception occurs. See next log for more details");
                Debug.LogException(ex);
                return false;
            }
        }
    }
}