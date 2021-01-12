using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Assignment1.Controller;
using Assignment1.Enum;
using Assignment1.POCO;
using SimpleHashing;

namespace Assignment1.Engine
{
    public class BankingEngineImpl : IBankingEngine
    {

        private const int FreeTransactionCount = 4;
        
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

        public async Task<Customer> LoginAttempt(int loginId, string password)
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

        public async Task<List<Transaction>> GetTransactions(Account account)
        {
            return await _databaseProxy.GetTransactions(account.AccountNumber);
        }

        public async Task<(bool success, Account updatedSourceAccount, Account updatedDestinationAccount)> MakeTransfer(
            Account sourceAccount, Account destinationAccount, decimal amount)
        {
            if (amount < 0) return (false, null, null);

            var serviceFee = await GetServiceFee(sourceAccount.AccountNumber, TransactionType.Transfer);

            var updatedSourceBalance = sourceAccount.Balance - amount - serviceFee;
            if (IsAboveMinimum(sourceAccount.AccountType, updatedSourceBalance)) return (false, null, null);

            await _databaseProxy.UpdateAccountBalance(updatedSourceBalance, sourceAccount.AccountNumber);

            await _databaseProxy.AddTransactionBulk(new List<Transaction>
            {
                new Transaction((char) TransactionType.Transfer,
                    sourceAccount.AccountNumber,
                    destinationAccount.AccountNumber, amount, null, DateTime.UtcNow),
                new Transaction((char) TransactionType.Deposit,
                    destinationAccount.AccountNumber, sourceAccount.AccountNumber, amount, null, DateTime.UtcNow)
            });

            if (serviceFee > 0)
            {
                await _databaseProxy.AddTransaction(new Transaction((char) TransactionType.ServiceCharge,
                    sourceAccount.AccountNumber,
                    sourceAccount.AccountNumber, serviceFee,
                    "transfer fee", DateTime.UtcNow));
            }

            await _databaseProxy.UpdateAccountBalance(destinationAccount.Balance + amount,
                destinationAccount.AccountNumber);

            sourceAccount.Balance = updatedSourceBalance;
            destinationAccount.Balance += amount;

            return (true, sourceAccount, destinationAccount);
        }

        public async Task<(bool wasSuccess, decimal endingBalance)> MakeTransaction(Account account,
            TransactionType transactionType, decimal amount)
        {
            var updatedBalance = account.Balance;

            if (amount < 0)
            {
                return (false, updatedBalance);
            }

            decimal serviceFee = 0;

            switch (transactionType)
            {
                case TransactionType.Deposit:
                    updatedBalance += amount;
                    break;
                case TransactionType.Withdraw:
                    serviceFee = await GetServiceFee(account.AccountNumber, TransactionType.Withdraw);
                    updatedBalance -= amount;
                    updatedBalance -= serviceFee;
                    if (IsAboveMinimum(account.AccountType, updatedBalance))
                    {
                        return (false, account.Balance);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null);
            }

            // Sadly the database requests need to be run synchronously due to the database not support multiple active result sets 
            await _databaseProxy.UpdateAccountBalance(updatedBalance, account.AccountNumber);

            await _databaseProxy.AddTransaction(new Transaction((char) transactionType,
                account.AccountNumber, account.AccountNumber, amount, null, DateTime.UtcNow));

            if (transactionType == TransactionType.Withdraw && serviceFee > 0)
            {
                await _databaseProxy.AddTransaction(new Transaction((char) TransactionType.ServiceCharge,
                    account.AccountNumber,
                    account.AccountNumber, serviceFee,
                    "withdrawal fee", DateTime.UtcNow));
            }

            return (true, updatedBalance);
        }

        public async Task<Account> GetAccount(int accountNumber)
        {
            return await _databaseProxy.GetAccount(accountNumber);
        }

        private async Task<decimal> GetServiceFee(int accountNumber, TransactionType transactionType)
        {
            var numberOfPreviousTransactions = await _databaseProxy.GetServiceFeeTransactionCounts(accountNumber);

            if (numberOfPreviousTransactions < FreeTransactionCount) return 0;
            
            return transactionType switch
            {
                // Value of the service fees are declared here because const initializers must be compile time (new decimal() isnt)
                TransactionType.Withdraw => new decimal(0.1),
                TransactionType.Transfer => new decimal(0.2),
                _ => 0
            };
        }

        public static bool IsAboveMinimum(char accountType, decimal balance)
        {
            return (accountType == 'S' && balance >= 0) || (accountType == 'C' && balance >= 200);
        }
    }
}