using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment1.Controller;
using Assignment1.Data;
using Assignment1.Enum;
using Microsoft.Data.SqlClient;
using SimpleHashing;

namespace Assignment1.Engine
{
    public class BankingEngineImpl : IBankingEngine
    {
        // The number of transactions an account can have before getting charged a service charge
        private const int FreeTransactionCount = 4;

        // The minimum balance for a savings account
        private const int MinimumSavingsAccountBalance = 0;

        // The minimum balance for the checking account
        private const int MinimumCheckingAccountBalance = 200;

        private BankingController _controller;

        private DatabaseProxy _databaseProxy;

        /// <summary>
        /// Prepares the engine by performing data seeding if needed and constructing needed classes
        /// </summary>
        /// <param name="controller">The controller the engine can communicate with</param>
        /// <returns>The tasks of the various operations used in data seeding</returns>
        public async Task Start(BankingController controller)
        {
            this._controller = controller;

            try
            {
                _databaseProxy = new DatabaseProxy();
            }
            catch (SqlException)
            {
                controller.StartUpFailed("Failed to connect to the database");
                return;
            }
            

            /*
             * Populates the database if there is no data in it using the data retrieved from the two data seeding APIs
             */

            if (!(await _databaseProxy.CustomersExist())) // Check that there are customers in the database
            {
                var customerDataTask = DataSeedApiProxy.RetrieveCustomerData();
                var loginDataTask = DataSeedApiProxy.RetrieveLoginData();

                var (customers, accounts, transactions) = await customerDataTask;

                if (customers == null || accounts == null || transactions == null) // If the API failed
                {
                    controller.StartUpFailed("Failed to retrieve customer data from API");
                }
                
                var loginData = await loginDataTask; // Await for the API here so database isn't populated if API is failing
                if (loginData == null) // If the API failed
                {
                    controller.StartUpFailed("Failed to retrieve login data from API");
                }

                
                // Due to table constraints these need to occur synchronously 
                await _databaseProxy.AddCustomerBulk(customers);
                await _databaseProxy.AddAccountBulk(accounts);
                await _databaseProxy.AddTransactionBulk(transactions);


                await _databaseProxy.AddLoginBulk(loginData);
            }
        }

        /// <summary>
        /// Verifies that a login attempt is valid
        /// </summary>
        /// <param name="loginId">The login id to use in the login attempt</param>
        /// <param name="password">The password to use in the login attempt</param>
        /// <returns>The task of the login attempt with result being the logged in customer if successful</returns>
        /// <exception cref="LoginFailedException">Thrown when the login attempt fails</exception>
        public async Task<Customer> LoginAttempt(int loginId, string password)
        {
            // Get the task for retrieving the needed login data
            var existingHashTask = _databaseProxy.GetPasswordHashAndCustomerId(loginId);

            int customerId;
            string passwordHash;
            try
            {
                (customerId, passwordHash) = await existingHashTask;
            }
            catch (RecordMissingException) // The login id doesn't exist. Treat this the same as an incorrect password
            {
                throw new LoginFailedException();
            }

            // Verify that the password is correct by hashing the input password and comparing it with the stored password
            if (PBKDF2.Verify(passwordHash, password))
            {
                return await _databaseProxy.GetCustomer(customerId);
            }
            else
            {
                throw new LoginFailedException();
            }
        }

        /// <summary>
        /// Gets all the accounts for the provided customer
        /// </summary>
        /// <param name="customer">The customer to get account for</param>
        /// <returns>The task of retrieving the accounts with the result being the list of associated accounts</returns>
        public async Task<List<Account>> GetAccounts(Customer customer)
        {
            return await _databaseProxy.GetAccounts(customer.CustomerId);
        }

        /// <summary>
        /// Gets the transactions linked to the provided account
        /// </summary>
        /// <param name="account">The account to get transactions for</param>
        /// <returns>The task of retrieving the transactions with the result being the associated transactions</returns>
        public async Task<List<Transaction>> GetTransactions(Account account)
        {
            return await _databaseProxy.GetTransactions(account.AccountNumber);
        }

        /// <summary>
        /// Makes a transfer between the source account and the destination account
        /// </summary>
        /// <param name="sourceAccount">The account to source funds from</param>
        /// <param name="destinationAccount">The account that the funds will go into</param>
        /// <param name="amount">The amount of $ to transfer between the accounts</param>
        /// <returns>The task of transfer with the resulting being whether the transfer was successful and the updated balances
        /// of the effected accounts</returns>
        public async Task<(bool success, Account updatedSourceAccount, Account updatedDestinationAccount)> MakeTransfer(
            Account sourceAccount, Account destinationAccount, decimal amount)
        {
            if (amount < 0) return (false, null, null); // Transfers must have a value more then 0

            // Get the service fee that needs to be charged. This keeps in mind if the user has used their 4 free transactions
            var serviceFee = await GetServiceFee(sourceAccount.AccountNumber, TransactionType.Transfer);

            var updatedSourceBalance = sourceAccount.Balance - amount - serviceFee;

            // Check if the transfer would result in a balance belong the minimum
            if (IsAboveMinimum(sourceAccount.AccountType, updatedSourceBalance)) return (false, null, null);

            await _databaseProxy.UpdateAccountBalance(updatedSourceBalance, sourceAccount.AccountNumber);

            // Add the transactions for the change in balance for the source and destination acocunts
            await _databaseProxy.AddTransactionBulk(new List<Transaction>
            {
                new Transaction((char) TransactionType.Transfer,
                    sourceAccount.AccountNumber,
                    destinationAccount.AccountNumber, amount, null, DateTime.UtcNow),
                new Transaction((char) TransactionType.Deposit,
                    destinationAccount.AccountNumber, sourceAccount.AccountNumber, amount, null, DateTime.UtcNow)
            });

            if (serviceFee > 0) // If this wasn't one of the users 4 free transactions
            {
                // Add the service fee transaction
                await _databaseProxy.AddTransaction(new Transaction((char) TransactionType.ServiceCharge,
                    sourceAccount.AccountNumber,
                    sourceAccount.AccountNumber, serviceFee,
                    "transfer fee", DateTime.UtcNow));
            }

            await _databaseProxy.UpdateAccountBalance(destinationAccount.Balance + amount,
                destinationAccount.AccountNumber);

            // Update the account objects with the balance changes
            sourceAccount.Balance = updatedSourceBalance;
            destinationAccount.Balance += amount;

            return (true, sourceAccount, destinationAccount);
        }

        /// <summary>
        /// Performs an ATM transactions on the provided account
        /// </summary>
        /// <param name="account">The account to do the atm transaction on</param>
        /// <param name="transactionType">The type of the ATM transaction (deposit/withdraw)</param>
        /// <param name="amount">The amount of the deposit/withdraw</param>
        /// <returns>The task of transaction with the result being if the transaction was successful
        /// and the ending balance of the account</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid transaction type is provided</exception>
        public async Task<(bool wasSuccess, decimal endingBalance)> MakeTransaction(Account account,
            TransactionType transactionType, decimal amount)
        {
            // Transactions must be more then 0
            if (amount < 0)
            {
                return (false, account.Balance);
            }

            var updatedBalance = account.Balance;

            decimal serviceFee = 0;

            switch (transactionType)
            {
                case TransactionType.Deposit:
                    updatedBalance += amount;
                    break;
                case TransactionType.Withdraw:
                    // Withdrawals involve a service fee. This gets the service fee and returns 0 if this is one of the users first 4 transactions
                    serviceFee = await GetServiceFee(account.AccountNumber, TransactionType.Withdraw);
                    updatedBalance -= amount;
                    updatedBalance -= serviceFee;

                    // Ensure that the transaction + service fee doesn't result in the balance going below the minimum
                    if (!IsAboveMinimum(account.AccountType, updatedBalance))
                    {
                        return (false, account.Balance);
                    }

                    break;
                default:
                    // Invalid transaction type was received
                    throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null);
            }

            // Sadly the database requests need to be run synchronously due to the database not support multiple active result sets 
            await _databaseProxy.UpdateAccountBalance(updatedBalance, account.AccountNumber);

            await _databaseProxy.AddTransaction(new Transaction((char) transactionType,
                account.AccountNumber, account.AccountNumber, amount, null, DateTime.UtcNow));

            // Add the service fee transaction if needed
            if (transactionType == TransactionType.Withdraw && serviceFee > 0)
            {
                await _databaseProxy.AddTransaction(new Transaction((char) TransactionType.ServiceCharge,
                    account.AccountNumber,
                    account.AccountNumber, serviceFee,
                    "withdrawal fee", DateTime.UtcNow));
            }

            return (true, updatedBalance);
        }

        /// <summary>
        /// Gets the account with the account number provided
        /// </summary>
        /// <param name="accountNumber">The account number of the account to retrieve</param>
        /// <returns>The task of retrieve with the result being the retrieved account</returns>
        public async Task<Account> GetAccount(int accountNumber)
        {
            try
            {
                return await _databaseProxy.GetAccount(accountNumber);
            }
            catch (RecordMissingException)
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates the service fee needed for specific transactions types.
        /// If the user has their first 4 free transactions still then the service fee is always 0
        /// </summary>
        /// <param name="accountNumber">The account number that the transaction is sourced from</param>
        /// <param name="transactionType">The type of the transaction</param>
        /// <returns>The task of database operation with the result being the service fee</returns>
        private async Task<decimal> GetServiceFee(int accountNumber, TransactionType transactionType)
        {
            // Get the number of transactions that should have incurred a service fee
            var numberOfPreviousTransactions = await _databaseProxy.GetServiceFeeTransactionCounts(accountNumber);

            if (numberOfPreviousTransactions < FreeTransactionCount) return 0; // User has free transactions still

            return transactionType switch
            {
                // Value of the service fees are declared here because const initializers must be compile time (new decimal() isnt)
                TransactionType.Withdraw => new decimal(0.1),
                TransactionType.Transfer => new decimal(0.2),
                _ => 0 // Only withdrawals and transfers have a service fee
            };
        }

        /// <summary>
        /// Checks that an account with the provided account type isn't below the minimum balance if it is updated to the provided balance
        /// </summary>
        /// <param name="accountType">The account type of the account being updated. Should only be S or C</param>
        /// <param name="balance">The account's new balance that needs to be checked</param>
        /// <returns>Whether the balance is below the minimum balance for that account type</returns>
        private static bool IsAboveMinimum(char accountType, decimal balance)
        {
            return (accountType == 'S' && balance >= MinimumSavingsAccountBalance) ||
                   (accountType == 'C' && balance >= MinimumCheckingAccountBalance);
        }
    }
}