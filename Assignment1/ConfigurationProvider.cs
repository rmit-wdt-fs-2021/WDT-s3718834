using Microsoft.Extensions.Configuration;

namespace Assignment1
{
    public static class ConfigurationProvider
    {
        private const string ConfigurationSourceFile = "appsettings.json";

        private static IConfigurationRoot _configurationRoot;

        private const string DatabaseConnectionStringKey = "ConnectionString";
        private const string CustomerDataSeedApiUrlKey = "CustomerDataSeedApiUrl";
        private const string LoginDataSeedApiUrlKey = "LoginDataSeedApiUrl";


        public static string GetDatabaseConnectionString()
        {
            InitializeConfig();
            return _configurationRoot[DatabaseConnectionStringKey];
        }

        public static string GetCustomerDataSeedApiUrl()
        {
            InitializeConfig();
            return _configurationRoot[CustomerDataSeedApiUrlKey];
        }
        
        public static string GetLoginDataSeedApiUrl()
        {
            InitializeConfig();
            return _configurationRoot[LoginDataSeedApiUrlKey];
        }

        private static void InitializeConfig()
        {
            _configurationRoot ??= new ConfigurationBuilder().AddJsonFile(ConfigurationSourceFile).Build();
        }
    }
}
