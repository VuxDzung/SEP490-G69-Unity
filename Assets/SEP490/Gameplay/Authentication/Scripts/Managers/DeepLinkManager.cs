using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase.Auth;
using SEP490G69.Authentication;
using SEP490G69;

public class DeepLinkManager : MonoBehaviour
{
    //private const string BackendUrl = "https://your-api-domain.com/api/auth/google/complete";

    private EventManager _eventManager;

    public EventManager GameEventManager
    {
        get
        {
            if (_eventManager == null)
            {
                _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
            }
            return _eventManager;
        }
    }

    private void Awake()
    {
#if UNITY_STANDALONE_WIN
        WindowsDeepLinkRegistrar.Register();
#endif
        HandleCommandLineArgs();
    }

    private void HandleCommandLineArgs()
    {
        string[] args = Environment.GetCommandLineArgs();

        foreach (var arg in args)
        {
            if (arg.StartsWith("mygame://"))
            {
                Debug.Log("DeepLink received: " + arg);
                _ = HandleDeepLinkAsync(arg);
                break;
            }
        }
    }

    private async Task HandleDeepLinkAsync(string url)
    {
        try
        {
            Uri uri = new Uri(url);
            string query = uri.Query.TrimStart('?');

            string tokenId = null;

            foreach (var pair in query.Split('&'))
            {
                var kv = pair.Split('=');
                if (kv.Length == 2 && kv[0] == "googleTokenId")
                {
                    tokenId = Uri.UnescapeDataString(kv[1]);
                }
            }

            if (string.IsNullOrEmpty(tokenId))
            {
                Debug.LogError("TempToken missing in deep link.");
                return;
            }

            await Task.CompletedTask;

            // Send an event here.
            GameEventManager.Publish<ReceiveTokenIdEvent>(new ReceiveTokenIdEvent
            {
                TokenId = tokenId,
            });

            //string idToken = await CompleteLogin(tokenId);

            //await FirebaseLogin(idToken);
        }
        catch (Exception ex)
        {
            Debug.LogError("DeepLink error: " + ex.Message);
        }
    }

    //private async Task<string> CompleteLogin(string tempToken)
    //{
    //    using HttpClient client = new HttpClient();

    //    var response = await client.PostAsJsonAsync(
    //        BackendUrl,
    //        new { TempToken = tempToken });

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        Debug.LogError("Backend complete login failed.");
    //        return null;
    //    }

    //    var result =
    //        await response.Content.ReadFromJsonAsync<TokenResponse>();

    //    return result.idToken;
    //}

    //private async Task FirebaseLogin(string idToken)
    //{
    //    var credential =
    //        GoogleAuthProvider.GetCredential(idToken, null);

    //    var user =
    //        await FirebaseAuth.DefaultInstance
    //            .SignInWithCredentialAsync(credential);

    //    Debug.Log("Firebase login success: " + user.UserId);
    //}
}

[Serializable]
public class TokenResponse
{
    public string idToken;
}

public class ReceiveTokenIdEvent : IEvent
{
    public string TokenId { get; set; }
}