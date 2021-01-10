using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment1.Engine;
using Assignment1.Enum;
using Assignment1.POCO;
using Assignment1.View;

namespace Assignment1.Controller
{
    public class BankingControllerImpl : BankingController
    {

        public Customer LoggedInCustomer { get; private set; }

        public BankingControllerImpl(IBankingEngine engine, IBankingView view) : base(engine, view)
        {
        }


        public override void Start()
        {
            View.Start(this);
            Task engineStartTask = Engine.Start(this);
            
            View.Loading();

            engineStartTask.Wait();
            if (engineStartTask.IsCompleted)
            {
                Login();
                View.MainMenu(LoggedInCustomer); 
            }
        }

        public override void Login()
        {

            var loginStatus = LoginStatus.Initial;
            while (loginStatus != LoginStatus.Success)
            {
                var (loginId, password) = View.Login(loginStatus);

                if (loginStatus == LoginStatus.MaxAttempts)
                {
                    Exit();
                    return; // TODO Remove this when I figure out how to properly exit
                }

                try
                {
                    var loginAttemptTask = Engine.LoginAttempt(loginId, password);
                    
                    View.Loading();

                    loginAttemptTask.Wait();
                    LoggedInCustomer = loginAttemptTask.Result;
                    loginStatus = LoginStatus.Success;
                }
                catch (LoginFailedException)
                {
                    loginStatus = LoginStatus.IncorrectPassword;
                }
                catch (LoginAttemptsExceededException)
                {
                    loginStatus = LoginStatus.MaxAttempts;
                }
            }

            View.MainMenu(LoggedInCustomer);


        }

        public override void TransactionHistory()
        {
            var getAccountsTask = Engine.GetAccounts(LoggedInCustomer);
            
            View.Loading();
            getAccountsTask.Wait();
            
            View.ShowTransactions(getAccountsTask.Result);
            View.MainMenu(LoggedInCustomer);
        }

        public override void Transfer()
        {
            var getAccountsTask = Engine.GetAccounts(LoggedInCustomer);
            
            View.Loading();
            getAccountsTask.Wait();
            
            var (sourceAccount, destinationAccount, amount) = View.Transfer(getAccountsTask.Result);

            if (sourceAccount != null)
            {
                var transferTask = Engine.MakeTransfer(sourceAccount, destinationAccount, amount);
                View.Loading();

                transferTask.Wait();
                
                if (transferTask.Result)
                {
                    View.TransferResponse(transferTask.Result, sourceAccount, destinationAccount, amount);
                }

                Transfer();
            }


            View.MainMenu(LoggedInCustomer);
        }

        public override List<Transaction> GetTransactions(Account account)
        {
            var getTransactionsTask = Engine.GetTransactions(account);
            
            View.Loading();
            getTransactionsTask.Wait();

            return getTransactionsTask.Result;
        }

        // TODO Implement
        public override void ModifyProfile()
        {
            View.WorkInProgress();
            View.MainMenu(LoggedInCustomer);
        }

        // TODO Implement
        public override void ApplyForLoan()
        {
            View.WorkInProgress();
            View.MainMenu(LoggedInCustomer);
        }

        public override void Logout()
        {
            LoggedInCustomer = null;
            View.Clear();
            Login();
        }

        public override void AtmTransaction()
        {
            while (true)
            {
                var getAccountsTask = Engine.GetAccounts(LoggedInCustomer);
            
                View.Loading();
                getAccountsTask.Wait();
                
                var (account, transactionType, amount) = View.AtmTransaction(getAccountsTask.Result);
                if (account == null)
                {
                    View.MainMenu(LoggedInCustomer);
                }
                else
                {
                    var transactionTask = Engine.MakeTransaction(account, transactionType, amount);
                    View.Loading();
                    transactionTask.Wait();
                    var (wasSuccess, endingBalance) = transactionTask.Result;
                    
                    View.TransactionResponse(wasSuccess, transactionType, amount, endingBalance);
                    continue;
                }

                break;
            }
        }

        public override void Exit()
        {
            // TODO Put in the real method for exiting
        }
    }
}
