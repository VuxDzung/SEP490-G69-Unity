namespace SEP490G69.Authentication
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Security.Cryptography;
    using System.IO;
    using UnityEngine;
    using Firebase.Auth;
    using Newtonsoft.Json.Linq;
    using UnityEngine.Networking;

    public class WindowsGoogleAuthService 
    {
        private const string AUTH_ENDPOINT = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TOKEN_ENDPOINT = "https://oauth2.googleapis.com/token";

        private const string CLIENT_ID = "849240330897-d701uq0uqd8ocs2tvurtp0h933hn8dlg.apps.googleusercontent.com";

        private const int Port = 5000;
        private string RedirectUri => $"http://localhost:{Port}/";

        private string _codeVerifier;

        public async Task<FirebaseUser> SignInWithGoogle()
        {
            _codeVerifier = GenerateCodeVerifier();
            string codeChallenge = GenerateCodeChallenge(_codeVerifier);

            string authorizationUrl =
                $"{AUTH_ENDPOINT}" +
                $"?client_id={CLIENT_ID}" +
                $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                $"&response_type=code" +
                $"&scope=openid%20email%20profile" +
                $"&code_challenge={codeChallenge}" +
                $"&code_challenge_method=S256";

            Process.Start(new ProcessStartInfo
            {
                FileName = authorizationUrl,
                UseShellExecute = true
            });

            string code = await WaitForAuthorizationCode();

            string idToken = await ExchangeCodeForToken(code);

            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

            var result = await FirebaseAuth.DefaultInstance
                .SignInWithCredentialAsync(credential);

            return result;
        }

        private async Task<string> WaitForAuthorizationCode()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(RedirectUri);
            listener.Start();

            var context = await listener.GetContextAsync();
            var code = context.Request.QueryString["code"];

            byte[] responseBytes = Encoding.UTF8.GetBytes(
                "<html><body>You can close this window.</body></html>");

            context.Response.ContentLength64 = responseBytes.Length;
            await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            context.Response.OutputStream.Close();

            listener.Stop();

            if (string.IsNullOrEmpty(code))
                throw new Exception("Authorization code missing");

            return code;
        }

        private async Task<string> ExchangeCodeForToken(string code)
        {
            WWWForm form = new WWWForm();
            form.AddField("client_id", CLIENT_ID);
            //form.AddField("client_secret", CLIENT_SECRET);
            form.AddField("code", code);
            form.AddField("code_verifier", _codeVerifier);
            form.AddField("redirect_uri", RedirectUri);
            form.AddField("grant_type", "authorization_code");

            using (UnityWebRequest www = UnityWebRequest.Post(TOKEN_ENDPOINT, form))
            {
                await www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    UnityEngine.Debug.LogError(www.downloadHandler.text); 
                    throw new Exception(www.error);
                }

                JObject json = JObject.Parse(www.downloadHandler.text);

                return json["id_token"].ToString();
            }
        }

        private string GenerateCodeVerifier()
        {
            byte[] bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            return Base64UrlEncode(bytes);
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
                return Base64UrlEncode(bytes);
            }
        }

        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}