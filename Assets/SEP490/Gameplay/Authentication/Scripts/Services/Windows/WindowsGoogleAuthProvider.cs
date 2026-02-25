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

    public class WindowsGoogleAuthProvider : IGoogleAuthProvider
    {
        private const string AUTH_URL = "https://accounts.google.com/o/oauth2/v2/auth";

        private string _codeVerifier;
        private WebRequests _webRequests;
        private GoogleClientConfig clientConfig;

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
            string authorizationCode = await ListenForAuthorizationCode();

            if (string.IsNullOrEmpty(authorizationCode))
                return string.Empty;

            WindowsLoginByGGRequest request = new WindowsLoginByGGRequest
            {
                AuthorizationCode = authorizationCode,
                CodeVerifier = _codeVerifier
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
            _codeVerifier = GenerateCodeVerifier();
            string codeChallenge = GenerateCodeChallenge(_codeVerifier);

            string url =
                $"{AUTH_URL}?" +
                $"client_id={clientConfig.ClientId}" +
                $"&redirect_uri={clientConfig.RedirectUri}" +
                $"&response_type=code" +
                $"&scope=openid%20email%20profile" +
                $"&code_challenge={codeChallenge}" +
                $"&code_challenge_method=S256";

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        // ===============================
        // STEP 2: LISTEN REDIRECT
        // ===============================

        private async Task<string> ListenForAuthorizationCode()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(clientConfig.RedirectUri);
            listener.Start();

            var context = await listener.GetContextAsync();
            var request = context.Request;

            string code = request.QueryString["code"];

            byte[] responseBytes = Encoding.UTF8.GetBytes(
                "Login successful. You can close this window.");

            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.Close();

            listener.Stop();

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
    }

    public class WindowsLoginByGGRequest
    {
        public string AuthorizationCode { get; set; }
        public string CodeVerifier { get; set; }
    }

    public class WindowsLoginByGGResponse
    {
        public string TokenId { get; set; }
    }
}