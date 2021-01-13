using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Assignment1.Data;
using Assignment1.Engine;
using Assignment1.Enum;
using Assignment1.View;

namespace Assignment1.Controller
{
    public class BankingControllerImpl : BankingController
    {
        private Customer _loggedInCustomer;
        
        public BankingControllerImpl(IBankingEngine engine, IBankingView view) : base(engine, view)
        {
        }


        /// <summary>
        /// Starts the application but starting the view and the engine then opening the login menu.
        /// </summary>
        public override void Start()
        {
            View.Start(this);
            PerformVoidWithLoading(Engine.Start(this));

            Login();
        }

        /// <summary>
        /// Starts the login process by running the view login method
        /// </summary>
        public override void Login()
        {
            View.Login();
            View.MainMenu(_loggedInCustomer);
        }

        /// <summary>
        /// Starts the transaction history method but calling the engine for accounts and giving it to the view
        /// </summary>
        public override void TransactionHistory()
        {
            View.ShowTransactions(PerformWithLoading(Engine.GetAccounts(_loggedInCustomer)));
            View.MainMenu(_loggedInCustomer);
        }

        /// <summary>
        /// Starts the transfer functionality but getting accounts from the engine and giving it to the front end
        /// </summary>
        public override void Transfer()
        {
            View.Transfer(PerformWithLoading(Engine.GetAccounts(_loggedInCustomer)));

            View.MainMenu(_loggedInCustomer);
        }
        
        /// <summary>
        /// Logs out the current user and returns to the login screen
        /// </summary>
        public override void Logout()
        {
            _loggedInCustomer = null;
            Login();
        }

        /// <summary>
        /// Starts the ATM transaction screen by getting the logged in user's accounts and giving to the view
        /// </summary>
        public override void AtmTransaction()
        {
            View.AtmTransaction(PerformWithLoading(Engine.GetAccounts(_loggedInCustomer)));

            View.MainMenu(_loggedInCustomer);
        }

        /// <summary>
        /// Terminals the application
        /// </summary>
        public override void Exit()
        {
            Environment.Exit(0);
        }
        
        /// <summary>
        /// Calls the relevant methods to validate that a user login is correct 
        /// </summary>
        /// <param name="loginId">The login id of the login to validate</param>
        /// <param name="password">The password of the login to validate</param>
        /// <returns>Whether the login attempt was successful</returns>
        public override bool ValidateLogin(int loginId, string password)
        {
            var loginAttempt = PerformWithLoading(Engine.LoginAttempt(loginId, password));
            _loggedInCustomer = loginAttempt;

            return true; // If the login failed the LoginFailedException will bubble up and this point won't be reached
        }

        /// <summary>
        /// Calls the relevant methods to validate that an ATM transaction is correct
        /// </summary>
        /// <param name="account">The account an atm transaction will be made from</param>
        /// <param name="transactionType">The ATM transaction type</param>
        /// <param name="amount">The amount of $ in the transaction</param>
        /// <returns></returns>
        public override (bool wasSuccess, decimal newBalance) MakeAtmTransaction(Account account,
            TransactionType transactionType,
            decimal amount)
        {
            var (wasSuccess, endingBalance) =
                PerformWithLoading(Engine.MakeTransaction(account, transactionType, amount));

            return (wasSuccess, endingBalance);
        }

        /// <summary>
        /// Calls the relevant methods to validate that a transfer is correct
        /// </summary>
        /// <param name="sourceAccount">The account to source the funds from</param>
        /// <param name="destinationAccount">The account the funds will land in</param>
        /// <param name="amount">The amount of $ that will be transferred from one account to the other</param>
        /// <returns></returns>
        public override (bool success, Account updatedSourceAccount, Account updatedDestinationAccount) MakeTransfer(
            Account sourceAccount,
            Account destinationAccount, decimal amount)
        {
            var (success, updatedSourceAccount, updatedDestinationAccount) =
                PerformWithLoading(Engine.MakeTransfer(sourceAccount, destinationAccount, amount));

            return (success, updatedSourceAccount,
                updatedDestinationAccount);
        }

        /// <summary>
        /// calls the relevant methods to get the transactions for the provided account
        /// </summary>
        /// <param name="account">The account to get transactions for</param>
        /// <returns>The transactions for the account</returns>
        public override List<Transaction> GetTransactions(Account account)
        {
            return PerformWithLoading(Engine.GetTransactions(account));
        }

        /// <summary>
        /// Calls the relevant methods to get an account based on an account number
        /// </summary>
        /// <param name="accountNumber">The account number to get an account for</param>
        /// <returns>The account that was retrieved</returns>
        public override Account GetAccount(int accountNumber)
        {
            return PerformWithLoading(Engine.GetAccount(accountNumber));
        }
        
        /// <summary>
        /// Helper method that performs an async task but calls the View Loading method so that the user knows that back end processing is occuring
        /// </summary>
        /// <param name="task">The async task to run while displaying a loading screen</param>
        /// <typeparam name="T">The return type of the task</typeparam>
        /// <returns>The result of the task</returns>
        /// <exception cref="Exception">Throws any exception created when the task was running</exception>
        private T PerformWithLoading<T>(Task<T> task)
        {
            View.Loading();

            try
            {
                task.Wait();
            }
            catch (AggregateException aggregateException)
            {
                // Throw every exception that occur within the task.
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    throw exception;
                }
            }

            return task.Result;
        }

        /// <summary>
        /// Helper method that performs an async task but calls the View Loading method so that the user knows that back end processing is occuring.
        /// Same as PerformWithLoadLoading() but allows for a task with no result
        /// </summary>
        /// <param name="task"></param>
        private void PerformVoidWithLoading(Task task)
        {
            View.Loading();
            task.Wait();
        }
        
        
        
        public override void StartUpFailed(string cause)
        {
            View.ApplicationFailed(cause);
            Exit();
        }

        
    }
}