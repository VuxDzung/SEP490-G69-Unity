namespace SEP490G69.Authentication
{
    using System.Drawing.Drawing2D;
    using UnityEngine;

    public class AuthResponse 
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public AuthData Data { get; set; }
    }

    public class AuthData
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int LegacyPoints { get; set; }
    }
}