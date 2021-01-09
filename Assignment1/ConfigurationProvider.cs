using Microsoft.Extensions.Configuration;

namespace Assignment1
{
    internal static class ConfigurationProvider
    {
        private const string ConfigurationSourceFile = "appsettings.json";

        private static IConfigurationRoot _configurationRoot;

        private const string DatabaseConnectionStringKey = "ConnectionString";
        private const string DataSeedApiUrlKey = "DataSeedApiUrl";

        public static string GetDatabaseConnectionString()
        {
            InitializeConfig();
            return _configurationRoot[DatabaseConnectionStringKey];
        }

        public static string GetDataSeedApiUrl()
        {
            InitializeConfig();
            return _configurationRoot[DataSeedApiUrlKey];
        }

        private static void InitializeConfig()
        {
            _configurationRoot ??= new ConfigurationBuilder().AddJsonFile(ConfigurationSourceFile).Build();
        }
    }
}
