namespace SEP490G69
{
    using Firebase.Auth;
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Addons.Networking;
    using SEP490G69.Authentication;
    using SEP490G69.Shared;
    using System;
    using System.Threading.Tasks;
    using UnityEngine;

    public class GameAuthManager : MonoBehaviour, IGameContext
    {
        private FirebaseAuthService firebaseAuth;
        private UnityAuthService unityAuth;
        private ContextManager _contextManager;
        private WebRequests _webRequests;
        private EventManager _eventManager;

        public event Action<string> OnLoginByGGWindowsChanged;

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

            //await unityAuth.InitializeAsync();
            //await unityAuth.SignInWithFirebaseAsync(idToken);

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

                // Step 3: initialize unity services
                //await unityAuth.InitializeAsync();

                // Step 4: Sign in Unity Authentication by Firebase token
                //await unityAuth.SignInWithFirebaseAsync(idToken);
                //Debug.Log("Sign in with open id success");

                // Step 5: Register by User id at backend.
                bool success = await LoginToGameBackend(idToken);

                return success;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Register Failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> SignInWithGoogleAndroid()
        {
            FirebaseUser user = await firebaseAuth.SignInGoogleAndroidAsync();
            if (user == null)
                return false;

            string idToken = await firebaseAuth.GetIdTokenAsync();
            if (string.IsNullOrEmpty(idToken))
                return false;

            await unityAuth.InitializeAsync();

            await unityAuth.SignInWithFirebaseAsync(idToken);

            // Step 5: Login/Sign up by user id.
            bool success = await LoginToGameBackend(idToken);

            return success;
        }

        public bool LoginByGuest()
        {
            try
            {
                string deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;
                string username = "";
                bool isSynced = false;
                PlayerData playerData = new PlayerData();
                playerData.PlayerId = deviceId;
                playerData.PlayerName = username;
                playerData.IsSynced = isSynced;

                if (_playerDataDAO.GetPlayerById(deviceId) != null)
                {
                    return false;
                }

                _playerDataDAO.InsertNewPlayer(playerData);

                return true;
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public void SignInByGoogleWindows()
        {
            firebaseAuth.SignInGoogleWindowsAsync();
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

        public string GetUnityPlayerId()
        {
            return unityAuth.GetUnityPlayerId();
        }

        public void LogoutAsync()
        {
            firebaseAuth.Logout();
            unityAuth.SignOut();
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

        private void FirebaseAuth_OnAutoLoginStarted()
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_LOADING).AsFrame<UILoadingScreen>().SetText("Auto login...");
        }

        private async void OnAutoLoginSuccess(FirebaseUser user)
        {
            string idToken = await firebaseAuth.GetIdTokenAsync();
            if (string.IsNullOrEmpty(idToken))
            {
                GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_LOADING);

                Debug.LogError("No token recevied");
                return;
            }

            GameUIManager.Singleton.GetActiveFrame(GameConstants.FRAME_ID_LOADING).AsFrame<UILoadingScreen>().SetText("Login to backend...");

            bool success = await LoginToGameBackend(idToken);
            if (success)
            {
                GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_LOADING);

                SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TITLE);
            }
        }

        private void FirebaseAuth_OnAutoLoginFailed(AuthErrorInfo obj)
        {
            GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_LOADING);
        }
    }
}