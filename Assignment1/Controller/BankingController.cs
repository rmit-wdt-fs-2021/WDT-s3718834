using System.Collections.Generic;
using Assignment1.Data;
using Assignment1.Engine;
using Assignment1.Enum;
using Assignment1.View;

namespace Assignment1.Controller
{
    /// <summary>
    /// The main controller for the application. Connects the View and the Engine together while directing user flow.
    /// </summary>
    public abstract class BankingController
    {
        protected IBankingEngine Engine { get; }
        protected IBankingView View { get; }

        protected BankingController(IBankingEngine engine, IBankingView view)
        {
            this.Engine = engine;
            this.View = view;
        }

        /// <summary>
        /// Starts the application. Must be called before any other method otherwise you can expect unexpected behaviour
        /// </summary>
        public abstract void Start();
        
        /// <summary>
        /// Starts and manages user login
        /// </summary>
        public abstract void Login();
        
        
        /// <summary>
        /// Method called by the view to validate a login is correct
        /// </summary>
        /// <param name="loginId">The user's login id</param>
        /// <param name="password">The password the user input</param>
        /// <returns>If the login was successful</returns>
        public abstract bool ValidateLogin(int loginId, string password);
        
        /// <summary>
        /// Starts and manages atm transaction functionality 
        /// </summary>
        public abstract void AtmTransaction();
        
        /// <summary>
        /// Starts and manages transfer functionality
        /// </summary>
        public abstract void Transfer();

        /// <summary>
        /// Method used by the view to perform and validate a transfer between accounts
        /// </summary>
        /// <param name="sourceAccount">The account where funds will be sourced</param>
        /// <param name="destinationAccount">The account where the funds will go to</param>
        /// <param name="amount">The $ amount to transfer between the accounts</param>
        /// <returns>Whether the transfer was successful and the updated balances of both the source and destination accounts</returns>
        public abstract (bool success, Account updatedSourceAccount, Account updatedDestinationAccount) MakeTransfer(
            Account sourceAccount, Account destinationAccount, decimal amount);

        /// <summary>
        /// Method used by the view to perform and validate an ATM transaction.
        /// </summary>
        /// <param name="account">The account that the atm transaction is being performed on</param>
        /// <param name="transactionType">The type of atm transaction (deposit or withdraw)</param>
        /// <param name="amount">The amount to be deposited/withdrawn in the atm transaction</param>
        /// <returns>IF the transaction was successful and the resulting balance for the account</returns>
        public abstract (bool wasSuccess, decimal newBalance) MakeAtmTransaction(Account account, TransactionType transactionType, decimal amount);

        /// <summary>
        /// Starts and manages the menu that shows transactions history 
        /// </summary>
        public abstract void TransactionHistory();
        
        /// <summary>
        /// Logs the current user out of the application
        /// </summary>
        public abstract void Logout();
        
        /// <summary>
        /// Terminals/Exits the application. Safely exiting any classes used
        /// </summary>
        public abstract void Exit();
        
        /// <summary>
        /// Method used by the view to get the transactions for an Account
        /// </summary>
        /// <param name="account">The account to get transactions for</param>
        /// <returns>The transactions for the account</returns>
        public abstract List<Transaction> GetTransactions(Account account);
        
        /// <summary>
        /// Method used by the view to retrieve an account based on the account number
        /// </summary>
        /// <param name="accountNumber">The account number for the account to retrieve</param>
        /// <returns>The account retrieved</returns>
        public abstract Account GetAccount(int accountNumber);

        /// <summary>
        /// Called when the startup of either the view or the engine has failed for whatever reason
        /// </summary>
        /// <param name="cause">A string explaining the cause</param>
        public abstract void StartUpFailed(string cause);
    }
}