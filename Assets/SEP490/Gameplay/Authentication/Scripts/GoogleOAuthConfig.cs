using UnityEngine;

[System.Serializable]
public class GoogleOAuthConfig
{
    public string ClientId;
    public string ClientSecret;
    public string RedirectUri = "http://localhost:5000/";
    public string Scope = "openid email profile";
}
