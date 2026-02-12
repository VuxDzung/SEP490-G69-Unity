namespace SEP490G69
{
    using Firebase.Auth;
    using SEP490G69.Addons.Networking;
    using SEP490G69.Authentication;
    using System;
    using System.Threading.Tasks;
    using UnityEngine;

    public class GameAuthManager : MonoBehaviour, IGameContext
    {
        private FirebaseAuthService firebaseAuth;
        private UnityAuthService unityAuth;
        private ContextManager _contextManager;
        private WebRequests _webRequests;

        private void Awake()
        {
            _webRequests = new WebRequests();
        }

        private void Start()
        {
            firebaseAuth = new FirebaseAuthService();
            unityAuth = new UnityAuthService();
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
            await unityAuth.InitializeAsync();
            await unityAuth.SignInWithFirebaseAsync(idToken);

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
                await unityAuth.InitializeAsync();

                // Step 4: Sign in Unity Authentication by Firebase token
                await unityAuth.SignInWithFirebaseAsync(idToken);
                Debug.Log("Sign in with open id success");

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

        public async Task<bool> SignInWithGoogleAsync()
        {
            //try
            //{
                // Step 1: Google + Firebase login
#if UNITY_ANDROID
                FirebaseUser user = await firebaseAuth.SignInGoogleAndroidAsync();
#else
                FirebaseUser user = await firebaseAuth.SignInGoogleWindowsAsync();
#endif

                if (user == null)
                    return false;

                // Step 2: Get Firebase ID Token
                string idToken = await firebaseAuth.GetIdTokenAsync();
                if (string.IsNullOrEmpty(idToken))
                    return false;

                // Step 3: Init Unity Services
                await unityAuth.InitializeAsync();

                // Step 4: Unity OpenID login
                await unityAuth.SignInWithFirebaseAsync(idToken);

                // Step 5: Login/Sign up by user id.
                bool success = await LoginToGameBackend(idToken);

                return success;
            //}
            //catch (System.Exception e)
            //{
            //    UnityEngine.Debug.LogError($"Google SignIn Failed: {e.Message}");
            //    return false;
            //}
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
    }
}