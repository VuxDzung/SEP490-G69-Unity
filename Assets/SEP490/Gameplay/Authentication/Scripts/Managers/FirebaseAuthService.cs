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
        private FirebaseUser _currentUser;
        private bool _initialized = false;

        private bool _isAutoLoginChecking = false;

        public event Action OnAutoLoginStarted;
        public event Action<FirebaseUser> OnAutoLoginSuccess;
        public event Action<FirebaseUser> OnLoginSuccess;
        public event Action<FirebaseUser> OnRegisterSuccess;
        public event Action OnLogout;
        public event Action OnPasswordChanged;

        public event Action<AuthErrorInfo> OnRegisterFailed;
        public event Action<AuthErrorInfo> OnLoginFailed;
        public event Action<AuthErrorInfo> OnAutoLoginFailed;

        private AuthFlowState _flowState;

        private IGoogleAuthProvider _authProvider;

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
                _authProvider = new WindowsGoogleAuthService();
            }
            else
            {
                _authProvider = new AndroidGoogleAuthProvider(WEB_CLIENT_ID);
            }

            InitializeFirebase();
        }

        private async void InitializeFirebase()
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyStatus == DependencyStatus.Available)
            {
                _auth = FirebaseAuth.DefaultInstance;

                _initialized = true;

                _flowState = AuthFlowState.AutoLogin;
                OnAutoLoginStarted?.Invoke();

                _auth.StateChanged += HandleAuthStateChanged;
            }
        }

        public async Task<FirebaseUser> LoginAsync(string email, string password)
        {
            try
            {
                _flowState = AuthFlowState.ManualLogin;

                var result = await _auth.SignInWithEmailAndPasswordAsync(email, password);
                return result.User;
            }
            catch (Exception e)
            {
                _flowState = AuthFlowState.None;

                var errorInfo = ParseFirebaseException(e);
                OnLoginFailed?.Invoke(errorInfo);
                return null;
            }
        }

        public async Task<FirebaseUser> RegisterAsync(string email, string password)
        {
            try
            {
                _flowState = AuthFlowState.Register;

                var result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
                return result.User;
            }
            catch (Exception e)
            {
                _flowState = AuthFlowState.None;

                var errorInfo = ParseFirebaseException(e);
                OnRegisterFailed?.Invoke(errorInfo);
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

        public async Task<FirebaseUser> SignInWithGoogleAsync()
        {
            string tokenId = await _authProvider.GetIdTokenAsync();

            return await SignInWithGoogleAsync(tokenId);
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
                Debug.LogException(e);
                return null;
            }
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
            try
            {
                var user = _auth.CurrentUser;

                if (user == null) return null;

                return await user.TokenAsync(true); // true = force refresh
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
        public string GetUID()
        {
            return _auth.CurrentUser.UserId;
        }

        public FirebaseUser GetUser()
        {
            return _auth.CurrentUser;
        }

        public void Logout()
        {
            _auth.SignOut();
        }

        private void HandleAuthStateChanged(object sender, EventArgs eventArgs)
        {
            if (!_initialized)
            {
                Debug.Log("Failed to init");
                return;
            }

            if (_auth.CurrentUser == null)
            {
                Debug.Log("No login session!");
                OnAutoLoginFailed?.Invoke(new AuthErrorInfo(AuthErrorType.UserNotFound, "No login session!"));

                return;
            }

            if (_auth.CurrentUser == _currentUser)
            {
                Debug.Log("same user!");
                return;
            }

            bool signedIn = _auth.CurrentUser != null;
            _currentUser = _auth.CurrentUser;

            switch (_flowState)
            {
                case AuthFlowState.AutoLogin:

                    if (signedIn)
                        OnAutoLoginSuccess?.Invoke(_currentUser);
                    else
                        OnAutoLoginFailed?.Invoke(new AuthErrorInfo(AuthErrorType.UserNotFound, "No login session!"));
                    break;

                case AuthFlowState.ManualLogin:

                    if (signedIn)
                        OnLoginSuccess?.Invoke(_currentUser);
                    break;

                case AuthFlowState.Register:

                    if (signedIn)
                        OnRegisterSuccess?.Invoke(_currentUser);
                    break;

                case AuthFlowState.None:

                    if (!signedIn)
                        OnLogout?.Invoke();

                    break;
            }

            _flowState = AuthFlowState.None;
        }

        private AuthErrorInfo ParseFirebaseException(Exception exception)
        {
            if (exception is FirebaseException firebaseEx)
            {
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                switch (errorCode)
                {
                    case AuthError.InvalidEmail:
                        return new AuthErrorInfo(AuthErrorType.InvalidEmail, "Invalid email");

                    case AuthError.UserNotFound:
                        return new AuthErrorInfo(AuthErrorType.UserNotFound, "Email does not exist");

                    case AuthError.WrongPassword:
                        return new AuthErrorInfo(AuthErrorType.WrongPassword, "Wrong password");

                    case AuthError.EmailAlreadyInUse:
                        return new AuthErrorInfo(AuthErrorType.EmailAlreadyInUse, "Email is used");

                    case AuthError.WeakPassword:
                        return new AuthErrorInfo(AuthErrorType.WeakPassword, "Password too week");

                    case AuthError.NetworkRequestFailed:
                        return new AuthErrorInfo(AuthErrorType.NetworkError, "Network connection failed");

                    case AuthError.UserDisabled:
                        return new AuthErrorInfo(AuthErrorType.UserDisabled, "Account is disabled");

                    default:
                        return new AuthErrorInfo(AuthErrorType.Unknown, firebaseEx.Message);
                }
            }

            return new AuthErrorInfo(AuthErrorType.Unknown, exception.Message);
        }
    }

    public enum AuthErrorType
    {
        None,

        // Common
        InvalidEmail,
        UserNotFound,
        WrongPassword,
        EmailAlreadyInUse,
        WeakPassword,
        NetworkError,
        UserDisabled,

        // Fallback
        Unknown
    }

    public enum AuthFlowState
    {
        None,
        AutoLogin,
        ManualLogin,
        Register
    }

    public class AuthErrorInfo
    {
        public AuthErrorType ErrorType;
        public string Message;

        public AuthErrorInfo(AuthErrorType type, string message)
        {
            ErrorType = type;
            Message = message;
        }
    }
}