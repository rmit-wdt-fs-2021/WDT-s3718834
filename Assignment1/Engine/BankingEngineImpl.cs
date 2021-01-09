﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment1.Controller;
using Assignment1.Enum;
using Assignment1.POCO;
using SimpleHashing;

namespace Assignment1.Engine
{
    public class BankingEngineImpl : IBankingEngine
    {
        private BankingController _controller;

        private DatabaseProxy _databaseProxy;

        public async Task Start(BankingController controller)
        {
            this._controller = controller;
            _databaseProxy = new DatabaseProxy();

            if (!(await _databaseProxy.CustomersExist()))
            {
                var customerDataTask = DataSeedApiProxy.RetrieveCustomerData();
                var loginDataTask = DataSeedApiProxy.RetrieveLoginData();
                
                var (customers, accounts, transactions) = await customerDataTask;
                
                // Due to table constraints these need to occur synchronously 
                await _databaseProxy.AddCustomerBulk(customers);
                await _databaseProxy.AddAccountBulk(accounts);
                await _databaseProxy.AddTransactionBulk(transactions);


                var loginData = await loginDataTask;
                _databaseProxy.AddLoginBulk(loginData);
            }
        }

        public async Task<Customer> LoginAttempt(string loginId, string password)
        {
            var existingHashTask = _databaseProxy.GetPasswordHashAndCustomerId(loginId);

            var (customerId, passwordHash) = await existingHashTask;
            
            if (SimpleHashing.PBKDF2.Verify(passwordHash, password))
            {
                return await _databaseProxy.GetCustomer(customerId);
            }
            else
            {
                throw new LoginFailedException();
            }
            
        }

        public async Task<List<Account>> GetAccounts(Customer customer)
        {
            return await _databaseProxy.GetAccounts(customer.CustomerId);
        }

        public List<Transaction> GetTransactions(Account account)
        {
            var transactions = new List<Transaction>
            {
                new Transaction('D', 987654321, 123012302, new decimal(10.01), "deposit money", DateTime.Now),
                new Transaction('S', 987654321, 987654321, new decimal(0.1), "withdraw charge", DateTime.Now),
                new Transaction('W', 987654321, 987654321, new decimal(20.02), "withdraw money", DateTime.Now),
                new Transaction('S', 987654321, 987654321, new decimal(0.2), "transfer charge", DateTime.Now),
                new Transaction('T', 987654321, 123456789, new decimal(40.03), "transfer to savings", DateTime.Now),
                new Transaction('D', 987654321, 123012302, new decimal(10.01), "deposit money", DateTime.Now),
                new Transaction('S', 987654321, 987654321, new decimal(0.1), "withdraw charge", DateTime.Now),
                new Transaction('W', 987654321, 987654321, new decimal(20.02), "withdraw money", DateTime.Now),
                new Transaction('S', 987654321, 987654321, new decimal(0.2), "transfer charge", DateTime.Now)
            };


            return transactions;
        }

        public bool MakeTransfer(Account sourceAccount, Account destinationAccount, decimal amount)
        {
            return amount <= sourceAccount.Balance;
        }

        public (bool wasSuccess, decimal endingBalance) MakeTransaction(Account account,
            TransactionType transactionType, decimal amount)
        {
            return amount == new decimal(2.5) ? (false, 10000) : (true, 10000);
        }
    }
}