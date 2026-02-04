namespace SEP490G69
{
    using System.Threading.Tasks;
    using Unity.Services.Authentication;
    using Unity.Services.Core;
    using UnityEngine;

    public class CloudAuthManager : MonoBehaviour, IGameContext
    {
        public bool IsInitialized { get; private set; }
        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;
        public string PlayerId => AuthenticationService.Instance.PlayerId;

        #region Initialize
        public void SetManager(ContextManager manager)
        {
            
        }

        public async Task InitializeAsync()
        {
            if (IsInitialized)
                return;

            await UnityServices.InitializeAsync();
            RegisterEvents();

            if (!IsSignedIn)
                await SignInAnonymously();

            IsInitialized = true;
            Debug.Log($"[Auth] Initialized | PlayerId: {PlayerId}");
        }

        private void RegisterEvents()
        {
            AuthenticationService.Instance.SignedIn += () =>
                Debug.Log($"[Auth] Signed In | PlayerId: {PlayerId}");

            AuthenticationService.Instance.SignedOut += () =>
                Debug.Log("[Auth] Signed Out");

            AuthenticationService.Instance.Expired += () =>
            {
                Debug.LogWarning("[Auth] Session expired → re-login");
                _ = SignInAnonymously();
            };
        }

        #endregion

        #region Anonymous

        public async Task SignInAnonymously()
        {
            if (IsSignedIn) return;

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        #endregion

        #region Username / Password

        /// <summary>
        /// Register new account
        /// </summary>
        public async Task<AuthResult> RegisterWithUsername(
            string username,
            string password,
            string displayName = null)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);

                if (!string.IsNullOrEmpty(displayName))
                {
                    await AuthenticationService.Instance.UpdatePlayerNameAsync(displayName);
                }

                return AuthResult.Success();
            }
            catch (AuthenticationException e)
            {
                return AuthResult.Fail(e.Message);
            }
        }

        /// <summary>
        /// Login existing account
        /// </summary>
        public async Task<AuthResult> LoginWithUsername(
            string username,
            string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);

                return AuthResult.Success();
            }
            catch (AuthenticationException e)
            {
                return AuthResult.Fail(e.Message);
            }
        }

        #endregion

        #region Google Login

        /// <summary>
        /// Google Sign-In (Android / iOS / WebGL)
        /// </summary>
        public async Task<AuthResult> LoginWithGoogle(string idToken)
        {
            try
            {
                await AuthenticationService.Instance
                    .SignInWithGoogleAsync(idToken);

                return AuthResult.Success();
            }
            catch (AuthenticationException e)
            {
                return AuthResult.Fail(e.Message);
            }
        }

        #endregion

        #region Account Linking
        /// <summary>
        /// Link anonymous → Google
        /// </summary>
        public async Task<AuthResult> LinkGoogleAccount(string idToken)
        {
            try
            {
                await AuthenticationService.Instance.LinkWithGoogleAsync(idToken);

                return AuthResult.Success();
            }
            catch (AuthenticationException e)
            {
                return AuthResult.Fail(e.Message);
            }
        }

        #endregion

        #region Open ID

        #endregion

        #region Logout

        public virtual void SignOut()
        {
            AuthenticationService.Instance.SignOut();
        }
        #endregion
    }

    public struct AuthResult
    {
        public bool IsSuccess;
        public string Error;

        public static AuthResult Success()
        {
            return new AuthResult { IsSuccess = true };
        }

        public static AuthResult Fail(string error)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}