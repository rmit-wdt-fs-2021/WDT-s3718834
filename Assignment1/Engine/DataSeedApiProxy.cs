using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Assignment1.Data;
using ConfigurationLibrary;
using Newtonsoft.Json;

namespace Assignment1.Engine
{
    /// <summary>
    /// Proxy for the two data seed APIs required for the assignment. Retrieves and returns converted api data
    /// </summary>
    public static class DataSeedApiProxy
    {
        /// <summary>
        /// Retrieves data from the customer data API and converts it to the relevant DTO. 
        /// </summary>
        /// <returns>The task of the API call with the result being the customer data in DTO form</returns>
        public static async Task<(IEnumerable<Customer> customers, IEnumerable<Account> accounts, IEnumerable<Transaction> transactions)> RetrieveCustomerData()
        {
            // Getting the API data
            var httpClient = new HttpClient();
            var rawJson = await httpClient.GetStringAsync(ConfigurationProvider.GetCustomerDataSeedApiUrl());

            // Converts the api data to object made for this data below. Uses DateFormatString to properly read the dates in the api data
            var convertedJson = JsonConvert.DeserializeObject<List<CustomerData>>(rawJson, new JsonSerializerSettings()
            {
                DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
            });

            
            // Theoretically shouldn't happen. Should only happen if the API is returning no data which are not expected to handle.
            if (convertedJson == null)
            {
                return (null, null, null);
            }

            var customers = new List<Customer>();
            var accounts = new List<Account>();
            var transactions = new List<Transaction>();
            foreach (var customerData in convertedJson)
            {
                customers.Add(new Customer(customerData.CustomerID, customerData.Name, customerData.Address,
                    customerData.City, customerData.PostCode));
                foreach (var account in customerData.Accounts)
                {
                    accounts.Add(new Account(account.AccountNumber,
                        account.AccountType,
                        customerData.CustomerID,
                        account.Balance));

                    // Uses the date provided to create a valid opening account transaction
                    transactions.Add(new Transaction('D',
                        account.AccountNumber,
                        account.AccountNumber,
                        account.Balance,
                        "Account creation",
                        account.Transactions[0].TransactionTimeUtc));
                }
            }

            return (customers, accounts, transactions);
        }

        /// <summary>
        /// Retrieves data from the login data API and converts it to the login DTO. 
        /// </summary>
        /// <returns>The operation of accessing the API with the results being the API data converted to login DTO</returns>
        public static async Task<IEnumerable<Login>> RetrieveLoginData()
        {
            var httpClient = new HttpClient();
            
            var rawJson = await httpClient.GetStringAsync(ConfigurationProvider.GetLoginDataSeedApiUrl());
            return JsonConvert.DeserializeObject<List<Login>>(rawJson); // Can convert straight to DTO here
        }

        
        /// <summary>
        /// Object used to represent data given by the API. Only slightly different from the DTO 
        /// </summary>
        private class CustomerData
        {
            public int CustomerID { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
            public List<CustomerDataAccount> Accounts { get; set; }

            public CustomerData(int customerId, string name, string address, string city, string postCode,
                List<CustomerDataAccount> accounts)
            {
                CustomerID = customerId;
                Name = name;
                Address = address;
                City = city;
                PostCode = postCode;
                Accounts = accounts;
            }
        }

        /// <summary>
        /// Object used to represent data given by the API.
        /// </summary>
        private class CustomerDataAccount
        {
            public int AccountNumber { get; set; }
            public char AccountType { get; set; }
            public int CustomerID { get; set; }
            public decimal Balance { get; set; }
            public List<TransactionData> Transactions { get; set; }

            public CustomerDataAccount(int accountNumber, char accountType, int customerId, decimal balance,
                List<TransactionData> transactions)
            {
                AccountNumber = accountNumber;
                AccountType = accountType;
                CustomerID = customerId;
                Balance = balance;
                Transactions = transactions;
            }
        }

        /// <summary>
        /// Object used to represent data given by the API. 
        /// </summary>
        private class TransactionData
        {
            public DateTime TransactionTimeUtc { get; set; }

            public TransactionData(DateTime transactionTimeUtc)
            {
                TransactionTimeUtc = transactionTimeUtc;
            }
        }
    }
}