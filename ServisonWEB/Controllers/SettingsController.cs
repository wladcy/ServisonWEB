namespace Admin.Services
{
    public class SettingsController
    {
        public class MailAccessModel
        {
            public static string Mail { get; set; }
            public static string Password { get; set; }
            public static string Host { get; set; }
            public static string Port { get; set; }
            public static bool EnableSSL { get; set; }
        }

        public class AppName
        {
            public static string Name { get; set; }
        }
    }
}
