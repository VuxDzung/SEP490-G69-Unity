namespace SEP490G69
{
    using LiteDB;
    using System;
    using System.IO;
    using UnityEngine;

    public class LocalDBInitiator : GlobalSingleton<LocalDBInitiator>
    {
        private LiteDatabase _database;

        /// <summary>
        /// Get current database instance.
        /// If the database instance is null, create a new instance.
        /// </summary>
        /// <returns></returns>

        //public static LiteDatabase GetDatabase()
        //{
        //    if (Singleton._database == null)
        //    {
        //        string folderPath = Path.Combine(Application.persistentDataPath, "database");

        //        if (!Directory.Exists(folderPath))
        //        {
        //            Directory.CreateDirectory(folderPath);
        //        }

        //        string dbPath = Path.Combine(folderPath, "GameData.db");

        //        Singleton._database = new LiteDatabase(dbPath);
        //    }

        //    return Singleton._database;
        //}

        private static readonly object _dbLock = new object();

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