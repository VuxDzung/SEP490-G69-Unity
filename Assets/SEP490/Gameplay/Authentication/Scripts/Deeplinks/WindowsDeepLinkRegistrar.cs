namespace SEP490G69.Authentication
{
#if UNITY_STANDALONE_WIN
    using System.Diagnostics;

    public static class WindowsDeepLinkRegistrar
    {
        private const string ProtocolName = "mygame";

        public static void Register()
        {
            string exePath =
                Process.GetCurrentProcess().MainModule.FileName;

            using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                $@"Software\Classes\{ProtocolName}"))
            {
                key.SetValue("", "URL:MyGame Protocol");
                key.SetValue("URL Protocol", "");

                using (var cmd =
                    key.CreateSubKey(@"shell\open\command"))
                {
                    cmd.SetValue("", $"\"{exePath}\" \"%1\"");
                }
            }
        }
    }
#endif
}