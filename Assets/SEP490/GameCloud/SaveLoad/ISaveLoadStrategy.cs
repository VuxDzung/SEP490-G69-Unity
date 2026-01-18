namespace SEP490G69
{
    public interface ISaveLoadStrategy 
    {
        public bool Save(string key, object value);
        public bool TryLoad<T>(string key, out T value);
    }
}