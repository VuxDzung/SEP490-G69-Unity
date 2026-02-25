using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Threading.Tasks;

[System.Serializable]
public class GoogleTokenResponse
{
    public string access_token;
    public string id_token;
    public string refresh_token;
    public int expires_in;
}

public static class GoogleTokenExchanger
{
    public static async Task<GoogleTokenResponse> ExchangeAsync(
        GoogleOAuthConfig config,
        string code)
    {
        string url = "https://oauth2.googleapis.com/token";

        Dictionary<string, string> body = new Dictionary<string, string>()
        {
            { "client_id", config.ClientId },
            { "client_secret", config.ClientSecret },
            { "code", code },
            { "redirect_uri", config.RedirectUri },
            { "grant_type", "authorization_code" }
        };

        UnityWebRequest request = UnityWebRequest.Post(url, body);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return null;
        }

        return JsonUtility.FromJson<GoogleTokenResponse>(request.downloadHandler.text);
    }
}
