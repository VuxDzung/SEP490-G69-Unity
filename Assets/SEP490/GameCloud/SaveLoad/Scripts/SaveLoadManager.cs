namespace SEP490G69
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class SaveLoadManager : GlobalSingleton<SaveLoadManager>
    {
        private ISaveLoadStrategy _saveLoadStrategy;
        private IAsyncSaveLoadStategy _asyncStrategy;

        protected override void CreateNewInstance()
        {
            base.CreateNewInstance();
            _asyncStrategy = new CloudSaveLoadStrategy();
        }

        public bool Save(string key, object value)
        {
            if (_saveLoadStrategy == null)
            {
                Debug.LogError("Synchronous strategy is not initialized yet");
                return false;
            }
            return _saveLoadStrategy.Save(key, value);
        }

        public bool TryLoad(string key, out object value)
        {
            value = null;
            if (_saveLoadStrategy == null)
            {
                Debug.LogError("Synchronous strategy is not initialized yet");
                return false;
            }
            return _saveLoadStrategy.TryLoad(key, out value);
        }

        public async Task<bool> SaveAsync(string key, object value)
        {
            if (_asyncStrategy == null)
            {
                Debug.LogError("Async strategy is not initialized yet");
                return false;
            }
            return await _asyncStrategy.SaveAsync(key, value);
        }
        public async Task<T> LoadAsync<T>(string key)
        {
            if (_asyncStrategy == null)
            {
                Debug.LogError("Async strategy is not initialized yet");
                return default(T);
            }
            return await _asyncStrategy.LoadAsync<T>(key);
        }
    }
}