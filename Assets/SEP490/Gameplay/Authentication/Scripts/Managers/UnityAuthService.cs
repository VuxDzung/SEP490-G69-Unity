namespace SEP490G69.Authentication
{
    using Unity.Services.Core;
    using Unity.Services.Authentication;
    using System.Threading.Tasks;

    public class UnityAuthService 
    {
        private const string AndroidProviderName = "oidc-firebaseandroid";
        private const string WebProviderName = "oidc-firebaseweb";

        public async Task InitializeAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
            }
        }

        public async Task SignInWithFirebaseAsync(string firebaseIdToken)
        {
            string providerName = WebProviderName;
#if UNITY_ANDROID
            providerName = AndroidProviderName;
#endif
            await AuthenticationService.Instance.SignInWithOpenIdConnectAsync(providerName, firebaseIdToken);
        }

        public string GetUnityPlayerId()
        {
            return AuthenticationService.Instance.PlayerId;
        }

        public void SignOut()
        {
            AuthenticationService.Instance.SignOut();
        }
    }
}