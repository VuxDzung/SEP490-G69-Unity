namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    using SEP490G69.Addons.Networking;
    using System.Threading.Tasks;
    using System;
    using UnityEngine.Networking;

    public class SyncApiService
    {
        private readonly WebRequests _web;

        public SyncApiService(WebRequests web)
        {
            _web = web;
        }

        public async Task<PlayerMetadataResponse> GetMetadata(string playerId, string sessionId)
        {
            string param = $"playerId={playerId}&sessionId={sessionId}";

            return await Request<PlayerMetadataResponse>("GetPlayerMetadata", param);
        }

        public async Task<GetPlayerGameDataResponse> GetFullData(string playerId, string sessionId)
        {
            string param = $"playerId={playerId}&sessionId={sessionId}";

            return await Request<GetPlayerGameDataResponse>("GetPlayerGameData", param);
        }

        public async Task<bool> OverrideCloud(OverrideCloudDataRequest req, SyncLocalService local)
        {
            OverrideCloudDataResponse response = await Put<OverrideCloudDataResponse>("OverrideCloudData", req);

            if (response?.Success == true)
            {
                local.UpdateLastSync(req.PlayerData.PlayerId);
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateLastSync(string playerId, DateTime time)
        {
            DeviceInfo deviceInfo = new DeviceInfo
            {
                PlayerId = playerId,
                DeviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier,
                DeviceType = UnityEngine.SystemInfo.deviceType.ToString(),  
            };

            return await Put<BaseAPIResponse>("UpdateLastSyncTime",
                new UpdateLastSyncTimeRequest
                {
                    PlayerId = playerId,
                    LastSyncTime = time,
                    SyncDevice = deviceInfo
                }) != null;
        }

        // Generic helpers
        private async Task<T> Request<T>(string endpoint, string param)
        {
            var tcs = new TaskCompletionSource<T>();

            await _web.GetEndpointByParam(endpoint, param, res =>
            {
                if (res.Result == UnityWebRequest.Result.Success)
                {
                    tcs.SetResult(JsonConvert.DeserializeObject<T>(res.Json));
                }
                else
                {
                    tcs.SetResult(default);
                }
            });

            return await tcs.Task;
        }

        private async Task<T> Put<T>(string endpoint, object body)
        {
            var tcs = new TaskCompletionSource<T>();

            string json = JsonConvert.SerializeObject(body);

            await _web.PutJsonByEndpointAsync(endpoint, json, res =>
            {
                if (res.Result == UnityWebRequest.Result.Success)
                {
                    tcs.SetResult(JsonConvert.DeserializeObject<T>(res.Json));
                }
                else
                {
                    tcs.SetResult(default);
                }
            });

            return await tcs.Task;
        }
    }
}