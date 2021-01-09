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
                LoggedInCustomer = Engine.LoginAttempt("", "");
                View.MainMenu(LoggedInCustomer); // Skipping login for testing 
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
                    LoggedInCustomer = Engine.LoginAttempt(loginId, password);
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
            View.ShowTransactions(Engine.GetAccounts(LoggedInCustomer));
            View.MainMenu(LoggedInCustomer);
        }

        public override void Transfer()
        {
            var (sourceAccount, destinationAccount, amount) = View.Transfer(Engine.GetAccounts(LoggedInCustomer));

            if (sourceAccount != null)
            {
                var transferResult = Engine.MakeTransfer(sourceAccount, destinationAccount, amount);
                if (transferResult)
                {
                    View.TransferResponse(transferResult, sourceAccount, destinationAccount, amount);
                }

                Transfer();
            }


            View.MainMenu(LoggedInCustomer);
        }

        public override List<Transaction> GetTransactions(Account account)
        {
            return Engine.GetTransactions(account);
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
                var (account, transactionType, amount) = View.AtmTransaction(Engine.GetAccounts(LoggedInCustomer));
                if (account == null)
                {
                    View.MainMenu(LoggedInCustomer);
                }
                else
                {
                    var (wasSuccess, endingBalance) = Engine.MakeTransaction(account, transactionType, amount);
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
