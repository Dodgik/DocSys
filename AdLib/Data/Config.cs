using System.Configuration;

namespace AdLib.Data
{
    public class Config
    {
        public const string SESSION_ERROR_MESSAGE = "ErrorMessage";

        public static string ConnectionString = (string)new AppSettingsReader().GetValue("ConnectStringAsync", typeof(string));
        public static string ConnectionStringAsync = ConfigurationManager.ConnectionStrings["SqlServices"].ConnectionString;

        public static string RootURL = (string)new AppSettingsReader().GetValue("RootURL", typeof(string));
        public static string SubFolder = (string)new AppSettingsReader().GetValue("SubFolder", typeof(string));


        public static bool CACHE_SLIDING = (bool)new AppSettingsReader().GetValue("CacheSlidingExpiration", typeof(bool));
        public static int CACHE_MINUTES = (int)new AppSettingsReader().GetValue("CacheTimeMinutes", typeof(int));
    }
}