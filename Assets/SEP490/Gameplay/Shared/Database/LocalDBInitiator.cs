namespace SEP490G69
{
    using LiteDB;
    using SEP490G69.Battle.Cards;
    using System;
    using System.IO;
    using UnityEngine;

    public class LocalDBInitiator : GlobalSingleton<LocalDBInitiator>
    {
        private static readonly object _dbLock = new object();

        protected override void CreateNewInstance()
        {
            base.CreateNewInstance();

            Execute(db =>
            {
                var cards = db.GetCollection<SessionCardData>(GameCardsDAO.COLLECTION_NAME);
                cards.EnsureIndex(x => x.SessionId);
                cards.EnsureIndex(x => x.RawCardId);
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
    }
}