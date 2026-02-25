namespace SEP490G69
{
    using Firebase.Auth;
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
            _webRequests = new WebRequests();
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
            _playerDataDAO = new PlayerDataDAO(LocalDBInitiator.GetDatabase());

            firebaseAuth = new FirebaseAuthService();
            unityAuth = new UnityAuthService();
        }

        private void OnEnable()
        {
            _eventManager.Subscribe<ReceiveTokenIdEvent>(HandleWindowsLoginByGoogle);
            firebaseAuth.OnAutoLoginStarted += FirebaseAuth_OnAutoLoginStarted;
            firebaseAuth.OnAutoLoginSuccess += OnAutoLoginSuccess;
            firebaseAuth.OnAutoLoginFailed += FirebaseAuth_OnAutoLoginFailed;
        }

        private void OnDisable()
        {
            _eventManager.Unsubscribe<ReceiveTokenIdEvent>(HandleWindowsLoginByGoogle);
            firebaseAuth.OnAutoLoginStarted -= FirebaseAuth_OnAutoLoginStarted;
            firebaseAuth.OnAutoLoginSuccess -= OnAutoLoginSuccess;
            firebaseAuth.OnAutoLoginFailed -= FirebaseAuth_OnAutoLoginFailed;
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            FirebaseUser user = await firebaseAuth.LoginAsync(email, password);

            if (user == null) return false;

            string idToken = await firebaseAuth.GetIdTokenAsync();
            if (string.IsNullOrEmpty(idToken)) return false;
            Debug.Log($"FirebaseTokenId: {idToken}");

            if (m_UseUGS) await SignInToUnityAuth(idToken);

            bool success = await LoginToGameBackend(idToken);

            return success;
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
                bool success = await LoginToGameBackend(idToken);

                if (success)
                {
                    string playerId = firebaseAuth.GetUID();
                    return TryCreateNewLocalUser(playerId, true) != null;
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
                return TryCreateNewLocalUser(deviceId, false) != null;
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public async Task<bool> SignInByGoogle()
        {
            FirebaseUser user = await firebaseAuth.SignInWithGoogleAsync();

            if (user == null)
                return false;

            string idToken = await firebaseAuth.GetIdTokenAsync();

            if (string.IsNullOrEmpty(idToken))
                return false;

            if (m_UseUGS) await SignInToUnityAuth(idToken);

            bool success = await LoginToGameBackend(idToken);

            return success;
        }

        private async void HandleWindowsLoginByGoogle(ReceiveTokenIdEvent ev)
        {
            if (ev == null)
            {
                OnLoginByGGWindowsChanged?.Invoke("failed");
                return;
            }

            string googleTokenId = ev.TokenId;

            if (string.IsNullOrEmpty(googleTokenId))
            {
                OnLoginByGGWindowsChanged?.Invoke("failed");
                return;
            }

            FirebaseUser user = await firebaseAuth.SignInWithGoogleAsync(googleTokenId);

            if (user == null)
            {
                OnLoginByGGWindowsChanged?.Invoke("failed");
                return;
            }

            // Step 2: Get Firebase ID Token
            string idToken = await firebaseAuth.GetIdTokenAsync();
            if (string.IsNullOrEmpty(idToken))
            {
                OnLoginByGGWindowsChanged?.Invoke("failed");
                return;
            }

            // Step 3: Init Unity Services
            await unityAuth.InitializeAsync();

            // Step 4: Unity OpenID login -> No longer use this.
            //await unityAuth.SignInWithFirebaseAsync(idToken);

            // Step 5: Login/Sign up by user id.
            bool success = await LoginToGameBackend(idToken);
            if (!success) 
            {
                OnLoginByGGWindowsChanged?.Invoke("failed");
                return;
            }
            OnLoginByGGWindowsChanged?.Invoke("success");
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
            if (m_UseUGS) unityAuth.SignOut();

            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_AUTH);
        }

        public async Task<bool> LoginToGameBackend(string tokenId)
        {
            bool success = true;
            _webRequests.SetJwt(tokenId);
            await _webRequests.PostJsonByEndpointAsync("LoginByFirebaseToken", "", (response) =>
            {
                success = response.Result == UnityEngine.Networking.UnityWebRequest.Result.Success;
            });
            return success;
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

            bool success = await LoginToGameBackend(idToken);
            if (success)
            {
                LoadingHandler.Singleton.Hide();
                Debug.Log("OnAutoLoginSuccess");
                PlayerData playerData = _playerDataDAO.GetPlayerById(user.UserId);

                if (playerData == null)
                {
                    PlayerData player = TryCreateNewLocalUser(user.UserId, true);
                    if (player == null)
                    {
                        Debug.LogError("Failed to create new local user!");
                        return;
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
                }
            }
            else
            {
                LoadingHandler.Singleton.Hide();
                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP).AsFrame<UIMessagePopup>().SetContent("title_error", "msg_fail_connect_be");
                OnLoginBEFailed?.Invoke();
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

        private PlayerData TryCreateNewLocalUser(string playerId, bool isSynced)
        {
            PlayerData _existedData = _playerDataDAO.GetPlayerById(playerId);
            if (_existedData != null)
            {
                Debug.Log($"Account existed!\nId: {playerId}");
                return _existedData;
            }

            string username = "";

            PlayerData playerData = new PlayerData();
            playerData.PlayerId = playerId;
            playerData.PlayerName = username;
            playerData.IsSynced = isSynced;

            _playerDataDAO.InsertNewPlayer(playerData);

            return playerData;
        }
    }
}