using System.Diagnostics;
using UnityEngine;

public static class GoogleBrowserLauncher
{
    public static void Open(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}
