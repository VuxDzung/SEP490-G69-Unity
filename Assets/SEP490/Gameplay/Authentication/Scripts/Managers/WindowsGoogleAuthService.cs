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
    using SEP490G69.Addons.Networking;
    using System.Security.Policy;

    public class WindowsGoogleAuthService : IGoogleAuthProvider
    {
        string clientId = "YOUR_CLIENT_ID";
        string redirectUri = "http://localhost:5001";

        private WebRequests _webRequests;

        public WindowsGoogleAuthService()
        {
            _webRequests = new WebRequests();
        }

        public Task<string> GetIdTokenAsync()
        {
            throw new NotImplementedException();
        }

        public void StartLogin()
        {
            Application.OpenURL(GetUrl());
        }

        public async Task<string> ListenForCodeAsync()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5001/");
            listener.Start();

            var context = await listener.GetContextAsync();
            var code = context.Request.QueryString["code"];

            byte[] responseBytes = Encoding.UTF8.GetBytes("Login success. You can close this window.");
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.Close();
            listener.Stop();

            return code;
        }

        private string GetUrl()
        {
            string codeVerifier = PkceUtil.GenerateCodeVerifier();
            string codeChallenge = PkceUtil.GenerateCodeChallenge(codeVerifier);

            string url =
                "https://accounts.google.com/o/oauth2/v2/auth" +
                "?client_id=" + clientId +
                "&redirect_uri=" + redirectUri +
                "&response_type=code" +
                "&scope=openid email profile" +
                "&code_challenge=" + codeChallenge +
                "&code_challenge_method=S256";

            return url;
        }
    }
}