namespace SEP490G69.Training
{
    using System.Security.Cryptography;
    using System.Text;
    using System;
    using System.Threading.Tasks;
    using UnityEngine;
    using System.Diagnostics;
    using LiteDB;
    using System.Collections.Generic;
    using System.Net.Http;
    using Mono.Cecil.Cil;
    using SEP490G69.Addons.Networking;
    using Newtonsoft.Json;
    using System.Net;
    using SEP490G69.Authentication;
    using System.Linq;

    public class WindowsGoogleAuthProvider : IGoogleAuthProvider
    {
        private const string AUTH_URL = "https://accounts.google.com/o/oauth2/v2/auth";

        private string _codeVerifier;
        private WebRequests _webRequests;
        private GoogleClientConfig clientConfig;
        private HttpListener _httpListener;

        public WindowsGoogleAuthProvider()
        {
            _webRequests = new WebRequests();
            clientConfig = Resources.Load<GoogleClientConfig>("GoogleClientConfig");
        }

        // ===============================
        // ENTRY POINT
        // ===============================
        public async Task<string> GetIdTokenAsync()
        {
            _codeVerifier = GenerateCodeVerifier();
            string codeChallenge = GenerateCodeChallenge(_codeVerifier);
            int port = GetAvailablePort();
            string redirectUri = $"http://127.0.0.1:{port}/";

            string authUrl = BuildAuthUrl(codeChallenge, redirectUri);
            Application.OpenURL(authUrl);

            string authorizationCode = await ListenForAuthCode(redirectUri);

            if (string.IsNullOrEmpty(authorizationCode))
                return string.Empty;

            WindowsLoginByGGRequest request = new WindowsLoginByGGRequest
            {
                AuthorizationCode = authorizationCode,
                CodeVerifier = _codeVerifier,
                RedirectUri = redirectUri
            };

            string json = JsonConvert.SerializeObject(request);

            WindowsLoginByGGResponse result = null;

            await _webRequests.PostJsonAsync("api/auth/google", json, (response) =>
            {
                if (response.Result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    result = JsonConvert.DeserializeObject<WindowsLoginByGGResponse>(response.Json);
                }
            });

            UnityEngine.Debug.Log($"Token id: {result?.TokenId}");
            return result?.TokenId ?? string.Empty;
        }

        // ===============================
        // STEP 1: START LOGIN
        // ===============================

        public void StartLogin()
        {
            //_codeVerifier = GenerateCodeVerifier();
            //string codeChallenge = GenerateCodeChallenge(_codeVerifier);
            //int port = GetAvailablePort();
            //string redirectUri = $"http://127.0.0.1:{port}/";

            //string authUrl = BuildAuthUrl(codeChallenge, redirectUri);
            //Application.OpenURL(authUrl);

            //string authCode = await ListenForAuthCode(redirectUri);

            //string url =
            //    $"{AUTH_URL}?" +
            //    $"client_id={clientConfig.ClientId}" +
            //    $"&redirect_uri={clientConfig.RedirectUri}" +
            //    $"&response_type=code" +
            //    $"&scope=openid%20email%20profile" +
            //    $"&code_challenge={codeChallenge}" +
            //    $"&code_challenge_method=S256";

            //Process.Start(new ProcessStartInfo
            //{
            //    FileName = url,
            //    UseShellExecute = true
            //});
        }

        private string BuildAuthUrl(string codeChallenge, string redirectUri)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["client_id"] = clientConfig.ClientId,
                ["redirect_uri"] = redirectUri,
                ["response_type"] = "code",
                ["scope"] = clientConfig.Scope,
                ["code_challenge"] = codeChallenge,
                ["code_challenge_method"] = "S256",
                ["access_type"] = "offline",
                ["prompt"] = "consent"
            };

            var queryString = string.Join("&",
                queryParams.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

            return $"https://accounts.google.com/o/oauth2/v2/auth?{queryString}";
        }

        // ===============================
        // STEP 2: LISTEN REDIRECT
        // ===============================

        private async Task<string> ListenForAuthCode(string redirectUri)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(redirectUri);
            _httpListener.Start();

            UnityEngine.Debug.Log($"Listening on {redirectUri}...");

            // Hiện trang chờ
            var context = await _httpListener.GetContextAsync();
            var request = context.Request;

            // Trả về HTML response cho user
            string responseHtml = "<html><body><h2>Login successful! You can close this window.</h2></body></html>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseHtml);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();

            _httpListener.Stop();

            // Parse authorization code từ query string
            string code = request.QueryString["code"];
            string error = request.QueryString["error"];

            if (!string.IsNullOrEmpty(error))
                throw new Exception($"Google auth error: {error}");

            if (string.IsNullOrEmpty(code))
                throw new Exception("No authorization code received");

            UnityEngine.Debug.Log("Authorization code received!");
            return code;
        }

        // ===============================
        // PKCE
        // ===============================
        private string GenerateCodeVerifier()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Base64UrlEncode(bytes);
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
            return Base64UrlEncode(bytes);
        }

        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private int GetAvailablePort()
        {
            var listener = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }

    public class WindowsLoginByGGRequest
    {
        public string AuthorizationCode { get; set; }
        public string CodeVerifier { get; set; }
        public string RedirectUri { get; set; }
    }

    public class WindowsLoginByGGResponse
    {
        public string TokenId { get; set; }
    }
}