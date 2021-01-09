using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Assignment1
{
    public static class ConfigurationProvider
    {
        private const string DefaultConfigurationSourceFile = "appsettings.json";

        /*
         * Stores the various configuration files being used
         */
        private static readonly Dictionary<string, IConfigurationRoot> ConfigurationRoots =
            new Dictionary<string, IConfigurationRoot>();

        /*
         * Keys for currently used connection strings and urls
         */
        private const string DatabaseConnectionStringKey = "ConnectionString";
        private const string CustomerDataSeedApiUrlKey = "CustomerDataSeedApiUrl";
        private const string LoginDataSeedApiUrlKey = "LoginDataSeedApiUrl";


        /*
         * Gets the database connection string from the config.
         */
        public static string GetDatabaseConnectionString(
            string configurationSourceFile = DefaultConfigurationSourceFile)
        {
            return GetConfigurationRoot(configurationSourceFile)[DatabaseConnectionStringKey];
        }

        /*
         * Gets the url for the customer data seed api
         */
        public static string GetCustomerDataSeedApiUrl(string configurationSourceFile = DefaultConfigurationSourceFile)
        {
            return GetConfigurationRoot(configurationSourceFile)[CustomerDataSeedApiUrlKey];
        }

        /*
         * Gets the url for the login data seed api
         */
        public static string GetLoginDataSeedApiUrl(string configurationSourceFile = DefaultConfigurationSourceFile)
        {
            return GetConfigurationRoot(configurationSourceFile)[LoginDataSeedApiUrlKey];
        }

        /*
         * Gets the config value for any key the user needs 
         */
        public static string GetConfig(string configKey,
            string configurationSourceFile = DefaultConfigurationSourceFile)
        {
            return GetConfigurationRoot(configurationSourceFile)[configKey];
        }

        /*
         * Initializes the config root if not already initialized, connecting to the file in the directory. 
         */
        private static IConfigurationRoot GetConfigurationRoot(string configurationSourceFile = DefaultConfigurationSourceFile)
        {
            if (!ConfigurationRoots.ContainsKey(configurationSourceFile)) // if config root not already initialized
            {
                // initialize from the json file provided
                ConfigurationRoots.Add(configurationSourceFile,
                    new ConfigurationBuilder().AddJsonFile(DefaultConfigurationSourceFile).Build());
            }

            return ConfigurationRoots[configurationSourceFile];
        }
    }
}