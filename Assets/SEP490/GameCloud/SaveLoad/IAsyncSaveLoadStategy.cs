namespace SEP490G69
{
    using System.Threading.Tasks;
    public interface IAsyncSaveLoadStategy 
    {
        public Task<bool> SaveAsync(string key, object value);
        public Task<T> LoadAsync<T>(string key);
    }
}