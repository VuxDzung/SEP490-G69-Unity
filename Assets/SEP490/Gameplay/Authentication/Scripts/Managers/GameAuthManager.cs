namespace SEP490G69
{
    using Firebase.Auth;
    using Newtonsoft.Json;
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Addons.Networking;
    using SEP490G69.Authentication;
    using SEP490G69.Shared;
    using System;
    using System.Threading.Tasks;
    using Unity.Services.Authentication.PlayerAccounts;
    using UnityEngine;

    public class GameAuthManager : MonoBehaviour, IGameContext
    {
        [SerializeField] private bool m_UseUGS = false;

        private FirebaseAuthService firebaseAuth;
        private UnityAuthService unityAuth;
        private ContextManager _contextManager;
        private WebRequests _webRequests;
        private EventManager _eventManager;

        public event Action<string> OnLoginByGGWindowsChanged;
        public event Action OnLoginBEFailed;

        private PlayerDataDAO _playerDataDAO;

        public FirebaseAuthService FirebaseAuthService => firebaseAuth;

        private void Awake()
        {
            _playerDataDAO = new PlayerDataDAO(LocalDBInitiator.GetDatabase());

            firebaseAuth = new FirebaseAuthService();
            unityAuth = new UnityAuthService();
        }

        private void OnEnable()
        {
            firebaseAuth.OnAutoLoginStarted += FirebaseAuth_OnAutoLoginStarted;
            firebaseAuth.OnAutoLoginSuccess += OnAutoLoginSuccess;
            firebaseAuth.OnAutoLoginFailed += FirebaseAuth_OnAutoLoginFailed;
        }

        private void OnDisable()
        {
            firebaseAuth.OnAutoLoginStarted -= FirebaseAuth_OnAutoLoginStarted;
            firebaseAuth.OnAutoLoginSuccess -= OnAutoLoginSuccess;
            firebaseAuth.OnAutoLoginFailed -= FirebaseAuth_OnAutoLoginFailed;
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;

            _webRequests = _contextManager.ResolveGameContext<WebRequests>();
            _eventManager = _contextManager.ResolveGameContext<EventManager>();
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            FirebaseUser user = await firebaseAuth.LoginAsync(email, password);

            if (user == null) return false;

            string idToken = await firebaseAuth.GetIdTokenAsync();
            if (string.IsNullOrEmpty(idToken)) return false;
            Debug.Log($"FirebaseTokenId: {idToken}");

            if (m_UseUGS) await SignInToUnityAuth(idToken);

            var response = await LoginToGameBackend(idToken);
            
            return response != null;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            try
            {
                // Step 1: Create firebase account
                FirebaseUser user = await firebaseAuth.RegisterAsync(email, password);
                if (user == null)
                    return false;
                Debug.Log("Create account success");

                // Step 2: Get token id
                string idToken = await firebaseAuth.GetIdTokenAsync();
                if (string.IsNullOrEmpty(idToken))
                    return false;

                Debug.Log("Get token id success");

                // Step 4: Sign in Unity Authentication by Firebase token
                if (m_UseUGS) await SignInToUnityAuth(idToken);

                // Step 5: Register by User id at backend.
                var response = await LoginToGameBackend(idToken);

                if (response != null)
                {
                    string playerId = firebaseAuth.GetUID();
                    PlayerData playerData = TryCreateNewLocalUser(playerId, "", firebaseAuth.GetUser().Email, 0, true);
                    
                    if (playerData == null)
                    {
                        return false;
                    }

                    GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_SET_NAME);
                }

                return false;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Register Failed: {e.Message}");
                return false;
            }
        }

        public bool LoginByGuest()
        {
            try
            {
                string deviceId = GetDeviceId();
                return TryCreateNewLocalUser(deviceId, "", "", 0, false) != null;
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public async void SignInByGoogle()
        {
            FirebaseUser user = await firebaseAuth.SignInWithGoogleAsync();

            if (user == null)
                return;

            string idToken = await firebaseAuth.GetIdTokenAsync();

            if (string.IsNullOrEmpty(idToken))
                return;

            if (m_UseUGS) await SignInToUnityAuth(idToken);

            var response = await LoginToGameBackend(idToken);

            HandleAccessBEComplete(response, user);
        }

        public void SendPasswordResetEmail(string email, Action onCancelled, Action onError, Action onComplete)
        {
            firebaseAuth.SendPasswordResetEmail(email, onCancelled, onError, onComplete);
        }

        public string GetFirebaseUid()
        {
            return firebaseAuth != null
                ? FirebaseAuth.DefaultInstance.CurrentUser?.UserId
                : null;
        }

        public string GetUserId()
        {
            string userId = GetFirebaseUid();
            if (string.IsNullOrEmpty(userId))
            {
                Debug.Log("No firebase cloud id. Use device id by default.");
                userId = GetDeviceId();
            }
            return userId;
        }

        public string GetDeviceId()
        {
            return UnityEngine.SystemInfo.deviceUniqueIdentifier;
        }

        public string GetUnityPlayerId()
        {
            if (!m_UseUGS) return string.Empty;

            return unityAuth.GetUnityPlayerId();
        }

        public void Logout()
        {
            firebaseAuth.Logout();
            PlayerPrefs.SetString(GameConstants.PREF_KEY_PLAYER_ID, "");
            PlayerPrefs.SetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID, "");
            if (m_UseUGS) unityAuth.SignOut();

            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_AUTH);
        }

        public async Task<AuthResponse> LoginToGameBackend(string tokenId)
        {
            bool success = true;
            _webRequests.SetJwt(tokenId);
            AuthResponse _authResponse = null;
            await _webRequests.PostJsonByEndpointAsync("LoginByFirebaseToken", "", (response) =>
            {
                success = response.Result == UnityEngine.Networking.UnityWebRequest.Result.Success;

                if (success)
                {
                    _authResponse = JsonConvert.DeserializeObject<AuthResponse>(response.Json);
                    _webRequests.SetJwt(_authResponse.Data.AccessToken);
                }
            });

            return _authResponse;
        }

        public async Task SignInToUnityAuth(string tokenId)
        {
            await unityAuth.InitializeAsync();
            await unityAuth.SignInWithFirebaseAsync(tokenId);
        }

        private void FirebaseAuth_OnAutoLoginStarted()
        {
            LoadingHandler.Singleton.Show().SetText("Auto login...");
        }

        private async void OnAutoLoginSuccess(FirebaseUser user)
        {
            string idToken = await firebaseAuth.GetIdTokenAsync();

            if (string.IsNullOrEmpty(idToken))
            {
                LoadingHandler.Singleton.Hide();
                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                                       .AsFrame<UIMessagePopup>()
                                       .SetContent("title_error", "msg_firebase_connect_fail", true, false);
                Debug.LogError("No token recevied");
                return;
            }

            LoadingHandler.Singleton.Show().SetText("Login to backend...");
            var response = await LoginToGameBackend(idToken);

            HandleAccessBEComplete(response, user);
        }

        /// <summary>
        /// After receive the user data from backend and firebase, check if this is the new device or old device.
        /// If it is the new device, 
        /// </summary>
        /// <param name="authResponse"></param>
        /// <param name="user"></param>
        private bool HandleAccessBEComplete(AuthResponse authResponse, FirebaseUser user)
        {
            if (authResponse != null)
            {
                LoadingHandler.Singleton.Hide();
                Debug.Log("OnAutoLoginSuccess");

                PlayerData playerData = _playerDataDAO.GetPlayerById(user.UserId);

                if (playerData == null)
                {
                    PlayerData player = TryCreateNewLocalUser(user.UserId, authResponse.Data.PlayerName, user.Email, authResponse.Data.LegacyPoints, true);
                    if (player == null)
                    {
                        Debug.LogError("Failed to create new local user!");
                        return false;
                    }
                }

                playerData = _playerDataDAO.GetPlayerById(user.UserId);

                if (playerData != null)
                {
                    if (string.IsNullOrEmpty(playerData.PlayerName))
                    {
                        GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_SET_NAME);
                    }
                    else
                    {
                        SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TITLE);
                    }
                    PlayerPrefs.SetString(GameConstants.PREF_KEY_PLAYER_ID, playerData.PlayerId);
                    return true;
                }

                return false;
            }
            else
            {
                LoadingHandler.Singleton.Hide();
                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP).AsFrame<UIMessagePopup>().SetContent("title_error", "msg_fail_connect_be");
                OnLoginBEFailed?.Invoke();

                return false;
            }
        }

        private void FirebaseAuth_OnAutoLoginFailed(AuthErrorInfo obj)
        {
            LoadingHandler.Singleton.Hide();
            GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_LOGIN);

            string deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            PlayerData playerData = _playerDataDAO.GetPlayerById(deviceId);

            if (_playerDataDAO.GetPlayerById(deviceId) != null)
            {
                if (string.IsNullOrEmpty(playerData.PlayerName))
                {
                    GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_SET_NAME);
                }
            }
            else
            {
                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_SET_LANG);
            }
        }

        private PlayerData TryCreateNewLocalUser(string playerId, string username, string playerEmail = "", int legacyPoints = 0, bool isSynced = true)
        {
            PlayerData _existedData = _playerDataDAO.GetPlayerById(playerId);

            if (_existedData != null)
            {
                Debug.Log($"Account existed!\nId: {playerId}");
                return _existedData;
            }

            PlayerData playerData = new PlayerData();
            playerData.PlayerId = playerId;
            playerData.PlayerName = username;
            playerData.PlayerEmail = playerEmail;
            playerData.IsSynced = isSynced;
            playerData.LegacyPoints = legacyPoints;

            _playerDataDAO.InsertNewPlayer(playerData);

            return playerData;
        }
    }
}