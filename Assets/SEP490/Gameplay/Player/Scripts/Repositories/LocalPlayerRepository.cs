namespace SEP490G69
{
    using LiteDB;
    using UnityEngine;

    public class LocalPlayerRepository 
    {
        private PlayerDataDAO _dao;

        public LocalPlayerRepository(LiteDatabase db)
        {
            _dao = new PlayerDataDAO(db);
        }
    }
}