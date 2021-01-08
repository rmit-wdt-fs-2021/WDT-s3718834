using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Assignment1
{
    class ConfigurationProvider
    {
        private static readonly string configurationSourceFile = "appsettings.json";

        private static IConfigurationRoot configurationRoot;

        private const string databaseConnectionStringKey = "ConnectionString";
        private const string dataSeedApiUrlKey = "DataSeedApiUrl";

        public static String GetDatabaseConnectionString()
        {
            InitializeConfig();
            return configurationRoot[databaseConnectionStringKey];
        }

        public static String GetDataSeedApiUrl()
        {
            InitializeConfig();
            return configurationRoot[dataSeedApiUrlKey];
        }

        private static void InitializeConfig()
        {
            if(configurationRoot == null)
            {
                configurationRoot = new ConfigurationBuilder().AddJsonFile(configurationSourceFile).Build();
            }
        }
    }
}
