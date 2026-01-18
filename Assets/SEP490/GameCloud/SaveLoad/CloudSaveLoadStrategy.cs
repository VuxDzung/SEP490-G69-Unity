namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using Unity.Services.CloudSave;
    using UnityEngine;

    public class CloudSaveLoadStrategy : IAsyncSaveLoadStategy
    {
        public async Task<bool> SaveAsync(string key, object value)
        {
            try
            {
                var data = new Dictionary<string, object>
                {
                    { key, value }
                };

                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[CloudSave] SaveAsync failed: {e}");
                return false;
            }
        }

        public async Task<T> LoadAsync<T>(string key)
        {
            try
            {
                var keys = new HashSet<string> { key };
                var result = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                if (result.TryGetValue(key, out var item))
                {
                    return item.Value.GetAs<T>();
                }

                Debug.LogWarning($"[CloudSave] Key not found: {key}");
                return default;
            }
            catch (Exception e)
            {
                Debug.LogError($"[CloudSave] LoadAsync failed: {e}");
                return default;
            }
        }
    }
}