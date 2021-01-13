using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment1.Controller;
using Assignment1.Data;
using Assignment1.Enum;

namespace Assignment1.Engine
{
    public interface IBankingEngine
    {
        /// <summary>
        /// Starts and creates any resources need for the engine. Must be called before any other methods or
        /// expected behaviours may occur
        /// </summary>
        /// <param name="controller">The controller can use to contact</param>
        /// <returns>The task of the start up operations</returns>
        public Task Start(BankingController controller);

        /// <summary>
        /// Verifies that a login attempt is valid
        /// </summary>
        /// <param name="loginId">The login id to use in the login attempt</param>
        /// <param name="password">The password to use in the login attempt</param>
        /// <returns>The task of the login attempt with result being the logged in customer if successful</returns>
        public Task<Customer> LoginAttempt(int loginId, string password);

        /// <summary>
        /// Gets all the accounts for the provided customer
        /// </summary>
        /// <param name="customer">The customer to get account for</param>
        /// <returns>The task of retrieving the accounts with the result being the list of associated accounts</returns>
        public Task<List<Account>> GetAccounts(Customer customer);
        
        /// <summary>
        /// Gets the transactions linked to the provided account
        /// </summary>
        /// <param name="account">The account to get transactions for</param>
        /// <returns>The task of retrieving the transactions with the result being the associated transactions</returns>
        public Task<List<Transaction>> GetTransactions(Account account);

        /// <summary>
        /// Makes a transfer between the source account and the destination account
        /// </summary>
        /// <param name="sourceAccount">The account to source funds from</param>
        /// <param name="destinationAccount">The account that the funds will go into</param>
        /// <param name="amount">The amount of $ to transfer between the accounts</param>
        /// <returns>The task of transfer with the resulting being whether the transfer was successful and the updated balances
        /// of the effected accounts</returns>
        public Task<(bool success, Account updatedSourceAccount, Account updatedDestinationAccount)> MakeTransfer(
            Account sourceAccount, Account destinationAccount, decimal amount);

        /// <summary>
        /// Performs an ATM transactions on the provided account
        /// </summary>
        /// <param name="account">The account to do the atm transaction on</param>
        /// <param name="transactionType">The type of the ATM transaction (deposit/withdraw)</param>
        /// <param name="amount">The amount of the deposit/withdraw</param>
        /// <returns>The task of transaction with the result being if the transaction was successful
        /// and the ending balance of the account</returns>
        public Task<(bool wasSuccess, decimal endingBalance)> MakeTransaction(Account account,
            TransactionType transactionType, decimal amount);

        /// <summary>
        /// Gets the account with the account number provided
        /// </summary>
        /// <param name="accountNumber">The account number of the account to retrieve</param>
        /// <returns>The task of retrieve with the result being the retrieved account</returns>
        public Task<Account> GetAccount(int accountNumber);
    }
    
    
    /// <summary>
    /// Thrown by the LoginAttempt method. Symbolizes the success of the login attempt 
    /// </summary>
    public class LoginFailedException : Exception
    {
        public LoginFailedException() : base("Login attempt failed")
        {
        }
    }
}