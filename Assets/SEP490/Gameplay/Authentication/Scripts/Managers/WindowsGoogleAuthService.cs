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

    public class WindowsGoogleAuthService
    {
        private WebRequests _webRequests;

        public WindowsGoogleAuthService()
        {
            _webRequests = new WebRequests();
        }

        public void StartLogin()
        {
            string url = _webRequests.BaseUrl + "api/auth/google/start";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}