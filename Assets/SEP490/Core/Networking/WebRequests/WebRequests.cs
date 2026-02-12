namespace SEP490G69.Addons.Networking
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Networking;

    public struct ResponsePacket
    {
        public UnityWebRequest.Result Result;
        public string Json;
        public string ErrorMessage;
    }

    public class WebRequests
    {
        private string _baseUrl;
        private string _jwt;
        private BackendUrlConfigSO _backendUrlConfig;
        public WebRequests()
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                _backendUrlConfig = Resources.Load<BackendUrlConfigSO>("BackendUrlConfig");
                if (_backendUrlConfig == null)
                {
                    Debug.LogError("Initialization error. Failed to get backend url config scriptable object");
                    return;
                }
                _baseUrl = _backendUrlConfig.BaseUrl;
            }
        }
        public WebRequests(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public void SetJwt(string jwt)
        {
            _jwt = jwt;
        }

        private void AttachJwt(UnityWebRequest request)
        {
            if (!string.IsNullOrEmpty(_jwt))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_jwt}");
            }
        }

        public async Task<Texture> GetTexture2DAsync(string url)
        {
            string endpointUrl = _baseUrl + url;
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(endpointUrl))
            {
                var operation = www.SendWebRequest();

                while (!www.isDone) await Task.Yield();
                Texture texture = null;
                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.DataProcessingError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    texture = DownloadHandlerTexture.GetContent(www);
                }
                return texture;
            }
        }

        public async Task GetJsonByEndpointAsync(string endpointName, string json, Action<ResponsePacket> onCompleted)
        {
            string url = _backendUrlConfig.GetEndpoint(endpointName);
            await GetJsonAsync(url, onCompleted);
        }
        public async Task GetEndpointByParam(string endpointName, string parameters, Action<ResponsePacket> onCompleted)
        {
            string url = _backendUrlConfig.GetEndpoint(endpointName) + "?" + parameters;
            await GetJsonAsync(url, onCompleted);
        }

        public async Task GetJsonAsync(string url, Action<ResponsePacket> onCompleted = null)
        {
            string endpointUrl = _baseUrl + url;
            Debug.Log($"Get url: {endpointUrl}");
            using (UnityWebRequest request = UnityWebRequest.Get(endpointUrl))
            {
                AttachJwt(request);
                var operation = request.SendWebRequest();

                while (!operation.isDone) await Task.Yield();

                ResponsePacket payload = new ResponsePacket();

                payload.Result = request.result;
                payload.Json = request.downloadHandler.text;

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.DataProcessingError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    payload.ErrorMessage = request.error;
                }
                LoggerUtils.Logging("Response", payload.ToString());
                onCompleted?.Invoke(payload);
            }
        }

        public async Task PostJsonByEndpointAsync(string endpointName, string json, Action<ResponsePacket> onCompleted = null)
        {
            string url = _backendUrlConfig.GetEndpoint(endpointName);
            await PostJsonAsync(url, json, onCompleted);
        }

        public async Task PostJsonAsync(string url, string json, Action<ResponsePacket> onCompleted = null)
        {
            string endpointUrl = _baseUrl + url;
            Debug.Log($"Post url: {endpointUrl}");

            using (UnityWebRequest request = new UnityWebRequest(endpointUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                AttachJwt(request);
                request.SetRequestHeader("Content-Type", "application/json");

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                ResponsePacket payload = new ResponsePacket
                {
                    Result = request.result,
                    Json = request.downloadHandler.text
                };

                if (request.result != UnityWebRequest.Result.Success)
                {
                    payload.ErrorMessage = request.error;
                }

                LoggerUtils.Logging("Response", string.IsNullOrEmpty(payload.Json) ? payload.ErrorMessage : payload.Json);
                onCompleted?.Invoke(payload);
            }
        }

        public async Task PutJsonAsync(string url, string json, Action<ResponsePacket> onCompleted = null)
        {
            string endpointUrl = _baseUrl + url;

            using (UnityWebRequest request = new UnityWebRequest(endpointUrl, "PUT"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                AttachJwt(request);
                request.SetRequestHeader("Content-Type", "application/json");

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                ResponsePacket payload = new ResponsePacket
                {
                    Result = request.result,
                    Json = request.downloadHandler.text
                };

                if (request.result != UnityWebRequest.Result.Success)
                    payload.ErrorMessage = request.error;

                LoggerUtils.Logging("Response", payload.ToString());
                onCompleted?.Invoke(payload);
            }
        }

        public async Task DeleteJsonAsync(string url, Action<ResponsePacket> onCompleted = null)
        {
            string endpointUrl = _baseUrl + url;

            using (UnityWebRequest request = UnityWebRequest.Delete(endpointUrl))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                AttachJwt(request);

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                ResponsePacket payload = new ResponsePacket
                {
                    Result = request.result,
                    Json = request.downloadHandler.text
                };

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.DataProcessingError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    payload.ErrorMessage = request.error;
                }

                LoggerUtils.Logging("Response received", payload.ToString());
                onCompleted?.Invoke(payload);
            }
        }

        public async Task UploadFiles(Action<UnityWebRequest> setHeaderAction, Dictionary<string, string> fileDataPack, Action onAllTaskFinished = null)
        {
            var uploadTasks = fileDataPack.Select(file => PutByteData(setHeaderAction, file.Key, file.Value));
            await Task.WhenAll(uploadTasks);
            onAllTaskFinished?.Invoke();
        }

        public async Task PutByteData(Action<UnityWebRequest> setHeaderAction, string url, string filePath, Action<ResponsePacket> onCompleted = null)
        {
            string endpointUrl = _baseUrl + url;

            byte[] fileData = await Task.Run(() => File.ReadAllBytes(filePath));
            await PutByteDataAsync(setHeaderAction, endpointUrl, fileData, onCompleted);
        }

        public async Task PutByteDataAsync(Action<UnityWebRequest> setHeaderAction, string url, byte[] fileData, Action<ResponsePacket> onCompleted = null)
        {
            string endpointUrl = _baseUrl + url;

            using (UnityWebRequest request = new UnityWebRequest(endpointUrl, "PUT"))
            {
                request.uploadHandler = new UploadHandlerRaw(fileData);
                request.downloadHandler = new DownloadHandlerBuffer();

                AttachJwt(request);
                request.SetRequestHeader("Content-Type", "application/octet-stream");

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                ResponsePacket payload = new ResponsePacket();
                payload.Json = request.downloadHandler.text;
                payload.Result = request.result;

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.DataProcessingError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    // Error
                    payload.ErrorMessage = request.downloadHandler.text;
                }
                LoggerUtils.Logging("Response", payload.ToString());
                onCompleted?.Invoke(payload);
            }
        }

        public static bool HasInternetConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }
            return true;
        }
    }
}