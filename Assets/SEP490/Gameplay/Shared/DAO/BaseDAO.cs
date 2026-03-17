namespace SEP490G69
{
    using LiteDB;

    public class BaseDAO 
    {
        protected ILiteCollection<T> GetCollection<T>(LiteDatabase db, string collectionName)
        {
            return db.GetCollection<T>(collectionName);
        }
    }
}