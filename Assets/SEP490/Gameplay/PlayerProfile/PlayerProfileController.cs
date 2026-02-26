namespace SEP490G69.PlayerProfile
{
    using Firebase.Auth;
    using Newtonsoft.Json;
    using SEP490G69.Addons.Networking;
    using System.Threading.Tasks;
    using System;
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
            _webRequests = new WebRequests();
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;
            _authManager = _contextManager.ResolveGameContext<GameAuthManager>();
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

            if (WebRequests.HasInternetConnection())
            {
                UpdatePlayerNameRequest requestModel = new UpdatePlayerNameRequest
                {
                    PlayerId = playerId,
                    PlayerName = playerName
                };
                string json = JsonConvert.SerializeObject(requestModel);
                await _webRequests.PutJsonByEndpointAsync("UpdatePlayerName", json, (response) =>
                {
                    if (response.Result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        Debug.Log("Update player name to cloud successfully.");
                    }
                });
            }
            else
            {
                Debug.LogError("No internet connection!");
            }
        }

        public async Task<string> GetCloudPlayerName(string playerId)
        {
            GetPlayerNameRequest request = new GetPlayerNameRequest { PlayerId = playerId };
            string json = JsonConvert.SerializeObject(request);
            string playerName = "";
            await _webRequests.GetJsonByEndpointAsync("GetPlayerName", json, (response) =>
            {
                if (response.Result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    playerName = response.Json;
                }
            });
            return playerName;
        }
    }

    public class UpdatePlayerNameRequest
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
    }

    public class GetPlayerNameRequest
    {
        public string PlayerId { get; set; }
    }
}