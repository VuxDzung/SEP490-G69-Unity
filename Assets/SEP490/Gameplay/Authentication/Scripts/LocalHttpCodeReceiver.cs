using UnityEngine;

using System.Net;
using System.Text;
using System.Threading.Tasks;

public class LocalHttpCodeReceiver
{
    private readonly string _redirectUri;

    public LocalHttpCodeReceiver(string redirectUri)
    {
        _redirectUri = redirectUri;
    }

    public async Task<string> WaitForCodeAsync()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(_redirectUri);
        listener.Start();

        var context = await listener.GetContextAsync();
        string code = context.Request.QueryString["code"];

        string response = "<html><body>Login success. You can close this window.</body></html>";
        byte[] buffer = Encoding.UTF8.GetBytes(response);

        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();

        listener.Stop();
        return code;
    }
}
