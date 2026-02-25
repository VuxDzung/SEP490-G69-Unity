namespace SEP490G69.Authentication
{
    using Google;
    using System.Threading.Tasks;
    using UnityEngine;

    public class AndroidGoogleAuthProvider : IGoogleAuthProvider
    {
        private GoogleSignInConfiguration _config;

        public AndroidGoogleAuthProvider(string webClientId)
        {
            _config = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true
            };
        }

        public async Task<string> GetIdTokenAsync()
        {
            GoogleSignIn.Configuration = _config;

            GoogleSignInUser user =
                await GoogleSignIn.DefaultInstance.SignIn();

            return user?.IdToken;
        }
    }
}