namespace SEP490G69.PlayerProfile
{
    using Firebase.Auth;
    using Newtonsoft.Json;
    using SEP490G69.Addons.Networking;
    using System.Threading.Tasks;
    using UnityEngine;

    public class PlayerProfileController : MonoBehaviour, IGameContext
    {
        private PlayerDataDAO _playerDAO;

        private WebRequests _webRequests;
        private ContextManager _contextManager;
        private GameAuthManager _authManager;

        private FirebaseAuth _firebase;

        private void Awake()
        {
            _playerDAO = new PlayerDataDAO(LocalDBInitiator.GetDatabase());
            _firebase = FirebaseAuth.DefaultInstance;
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;
            _authManager = _contextManager.ResolveGameContext<GameAuthManager>();
            _webRequests = _contextManager.ResolveGameContext<WebRequests>();
        }

        public async void UpdatePlayerName(string playerId, string playerName)
        {
            PlayerData playerData = _playerDAO.GetPlayerById(playerId);
            if (playerData == null)
            {
                return;
            }
            playerData.PlayerName = playerName;
            _playerDAO.UpdatePlayer(playerData);

            bool syncSuccess = await SyncPlayerName(playerId, playerName);
        }

        public async Task<bool> SyncPlayerName(string playerId, string playerName)
        {
            if (WebRequests.HasInternetConnection())
            {
                UpdatePlayerNameRequest requestModel = new UpdatePlayerNameRequest
                {
                    PlayerId = playerId,
                    PlayerName = playerName
                };
                string json = JsonConvert.SerializeObject(requestModel);

                bool success = false;
                await _webRequests.PutJsonByEndpointAsync("UpdatePlayerName", json, (response) =>
                {
                    if (response.Result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        Debug.Log("Update player name to cloud successfully.");
                        success = true;
                    }
                });

                return success;
            }
            else
            {
                Debug.LogError("No internet connection!");
                return false;
            }
        }

        public async Task<string> GetCloudPlayerName(string playerId)
        {
            GetPlayerNameResponse responseDTO = null;
            string param = $"playerId={playerId}";
            await _webRequests.GetEndpointByParam("GetPlayerName", param, (response) =>
            {
                if (response.Result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    responseDTO = JsonConvert.DeserializeObject<GetPlayerNameResponse>(response.Json); 
                }
            });

            return responseDTO != null ? responseDTO.PlayerName : string.Empty;
        }

        public string GetPlayerName(string playerId)
        {
            if (string.IsNullOrEmpty(playerId)) return string.Empty;

            PlayerData playerData = _playerDAO.GetPlayerById(playerId);
            if (playerData == null)
            {
                return string.Empty;
            }
            return playerData.PlayerName;
        }
        public string GetPlayerEmail(string playerId)
        {
            if (string.IsNullOrEmpty(playerId)) return string.Empty;

            PlayerData playerData = _playerDAO.GetPlayerById(playerId);
            if (playerData == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(playerData.PlayerEmail))
            {
                return _firebase.CurrentUser.Email;
            }
            return playerData.PlayerEmail;
        }
    }

    public class UpdatePlayerNameRequest
    {
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
    }

    public class GetPlayerNameRequest
    {
        public string PlayerId { get; set; }
    }
    public class GetPlayerNameResponse
    {
        public string PlayerName { get; set; } = string.Empty;
    }
}