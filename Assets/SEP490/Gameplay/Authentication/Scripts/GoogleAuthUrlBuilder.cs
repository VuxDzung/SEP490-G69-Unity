using UnityEngine;
using System.Web;
public static class GoogleAuthUrlBuilder
{
    public static string Build(GoogleOAuthConfig config)
    {
        string baseUrl = "https://accounts.google.com/o/oauth2/v2/auth";

        string url =
            baseUrl +
            "?client_id=" + config.ClientId +
            "&redirect_uri=" + config.RedirectUri +
            "&response_type=code" +
            "&scope=" +//+ HttpUtility.UrlEncode(config.Scope) +
            "&access_type=offline" +
            "&prompt=select_account";

        return url;
    }
}
