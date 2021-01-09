using System;
using System.Collections.Generic;
using System.Net.Http;
using Assignment1.POCO;
using ConfigurationLibrary;
using Newtonsoft.Json;

namespace Assignment1.Engine
{
    public static class DataSeedApiProxy
    {
        public static (IEnumerable<Customer> customers, IEnumerable<Account> accounts, IEnumerable<Transaction> transactions) RetrieveCustomerData()
        {
            var rawJson =  new HttpClient().GetStringAsync(ConfigurationProvider.GetCustomerDataSeedApiUrl()).Result;

            var convertedJson = JsonConvert.DeserializeObject<List<CustomerData>>(rawJson, new JsonSerializerSettings()
            {
                DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
            }); 

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

        public static IEnumerable<Login> RetrieveLoginData()
        {
            var rawJson = new HttpClient().GetStringAsync(ConfigurationProvider.GetLoginDataSeedApiUrl()).Result;
            return JsonConvert.DeserializeObject<List<Login>>(rawJson);
        }


        /*
         * Classes used to interpret API data
         */

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