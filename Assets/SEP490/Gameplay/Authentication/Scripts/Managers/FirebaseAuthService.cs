namespace SEP490G69.Authentication
{
    using Firebase;
    using Firebase.Auth;
    using Google;
    using System;
    using System.Threading.Tasks;
    using UnityEngine;

    public class FirebaseAuthService
    {
        private FirebaseAuth _auth;

        private const string WEB_CLIENT_ID = "849240330897-kblvrpuo44u3o785pjtfq9br4khi3h9f.apps.googleusercontent.com";

        private GoogleSignInConfiguration configuration;
        private WindowsGoogleAuthService _windowsAuthService;
        private FirebaseUser _currentUser;
        private bool _initialized = false;

        public event Action<FirebaseUser> OnAutoLoginSuccess;
        public event Action<FirebaseUser> OnLoginSuccess;
        public event Action OnLogout;
        public event Action OnPasswordChanged;

        public FirebaseAuthService()
        {
            _auth = FirebaseAuth.DefaultInstance;

            configuration = new GoogleSignInConfiguration
            {
                WebClientId = WEB_CLIENT_ID,
                RequestIdToken = true
            };

            if (Application.platform == RuntimePlatform.WindowsEditor || 
                Application.platform == RuntimePlatform.WindowsPlayer)
            {
                _windowsAuthService = new WindowsGoogleAuthService();
            }

            InitializeFirebase();
        }

        private async void InitializeFirebase()
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyStatus == DependencyStatus.Available)
            {
                _auth = FirebaseAuth.DefaultInstance;
                _auth.StateChanged += HandleAuthStateChanged;

                _initialized = true;

                // Trigger initial state check (auto login)
                HandleAuthStateChanged(this, null);
            }
            else
            {
                Debug.LogError("Firebase dependencies not available");
            }
        }

        public async Task<FirebaseUser> LoginAsync(string email, string password)
        {
            try
            {
                var result = await _auth.SignInWithEmailAndPasswordAsync(email, password);
                return result.User;
            }
            catch (System.Exception e)
            {
                return null;
            }
        }

        public async Task<FirebaseUser> RegisterAsync(string email, string password)
        {
            try
            {
                var result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
                return result.User;
            }
            catch (System.Exception e)
            {
                return null;
            }
        }

        public void SendPasswordResetEmail(string emailAddress, Action onCancelled, Action onError, Action onCompleted)
        {
            _auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    onCancelled?.Invoke();
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    // Handle specific errors like AuthError.UserNotFound
                    onError?.Invoke();
                    return;
                }

                Debug.Log("Password reset email sent successfully to " + emailAddress);
                onCompleted?.Invoke();
            });
        }

        public async Task<FirebaseUser> SignInWithGoogleAsync(string tokenId)
        {
            try
            {
                Credential credential =  GoogleAuthProvider.GetCredential(tokenId, null);

                FirebaseUser result = await _auth.SignInWithCredentialAsync(credential);

                return result;
            }
            catch(System.Exception e)
            {
                return null;
            }
        }

        public void SignInGoogleWindowsAsync()
        {
            _windowsAuthService.StartLogin();
        }

        public async Task<FirebaseUser> SignInGoogleAndroidAsync()
        {
            GoogleSignIn.Configuration = configuration;

            GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();
            if (googleUser == null)
                return null;
            return await SignInWithGoogleAsync(googleUser.IdToken);
        }
        

        public async Task<string> GetIdTokenAsync()
        {
            var user = _auth.CurrentUser;
            if (user == null) return null;

            return await user.TokenAsync(true); // true = force refresh
        }
        public string GetUID()
        {
            return _auth.CurrentUser.UserId;
        }

        public void Logout()
        {
            _auth.SignOut();
        }

        private void HandleAuthStateChanged(object sender, EventArgs eventArgs)
        {
            if (!_initialized) return;

            if (_auth.CurrentUser != _currentUser)
            {
                bool signedIn = _auth.CurrentUser != null;

                if (!signedIn && _currentUser != null)
                {
                    Debug.Log("User signed out");
                    OnLogout?.Invoke();
                }

                _currentUser = _auth.CurrentUser;

                if (signedIn)
                {
                    Debug.Log("User signed in: " + _currentUser.Email);

                    if (_currentUser != null)
                    {
                        OnAutoLoginSuccess?.Invoke(_currentUser);
                    }
                }
            }
        }
    }
}