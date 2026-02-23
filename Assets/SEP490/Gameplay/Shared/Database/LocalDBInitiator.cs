namespace SEP490G69
{
    using LiteDB;
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
        public static LiteDatabase GetDatabase()
        {
            if (Singleton._database == null)
            {
                string folderPath = Path.Combine(Application.persistentDataPath, "database");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string dbPath = Path.Combine(folderPath, "GameData.db");

                Singleton._database = new LiteDatabase(dbPath);
            }

            return Singleton._database;
        }
    }
}