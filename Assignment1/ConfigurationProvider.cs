using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Assignment1
{
    public static class ConfigurationProvider
    {
        private const string DefaultConfigurationSourceFile = "appsettings.json";

        private static readonly Dictionary<string, IConfigurationRoot> ConfigurationRoots =
            new Dictionary<string, IConfigurationRoot>();

        private const string DatabaseConnectionStringKey = "ConnectionString";
        private const string CustomerDataSeedApiUrlKey = "CustomerDataSeedApiUrl";
        private const string LoginDataSeedApiUrlKey = "LoginDataSeedApiUrl";


        public static string GetDatabaseConnectionString(
            string configurationSourceFile = DefaultConfigurationSourceFile)
        {
            InitializeConfig();
            return ConfigurationRoots[configurationSourceFile][DatabaseConnectionStringKey];
        }

        public static string GetCustomerDataSeedApiUrl(string configurationSourceFile = DefaultConfigurationSourceFile)
        {
            InitializeConfig();
            return ConfigurationRoots[configurationSourceFile][CustomerDataSeedApiUrlKey];
        }

        public static string GetLoginDataSeedApiUrl(string configurationSourceFile = DefaultConfigurationSourceFile)
        {
            InitializeConfig();
            return ConfigurationRoots[configurationSourceFile][LoginDataSeedApiUrlKey];
        }

        public static string GetConfig(string configKey,
            string configurationSourceFile = DefaultConfigurationSourceFile)
        {
            return ConfigurationRoots[configurationSourceFile][configKey];
        }

        private static void InitializeConfig(string configurationSourceFile = DefaultConfigurationSourceFile)
        {
            if (!ConfigurationRoots.ContainsKey(configurationSourceFile))
            {
                ConfigurationRoots.Add(configurationSourceFile,
                    new ConfigurationBuilder().AddJsonFile(DefaultConfigurationSourceFile).Build());
            }
        }
    }
}