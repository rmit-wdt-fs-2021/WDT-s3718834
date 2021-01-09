using System.Collections.Generic;
using Assignment1.Engine;
using Assignment1.Enum;
using Assignment1.POCO;
using Assignment1.View;

namespace Assignment1
{
    public class BankingControllerImpl : BankingController
    {

        public Customer LoggedInCustomer { get; private set; }

        public BankingControllerImpl(IBankingEngine engine, IBankingView view) : base(engine, view)
        {
        }


        public override void Start()
        {
            Engine.Start(this);
            View.Start(this);

            //Login();
            LoggedInCustomer = Engine.LoginAttempt("", "");
            View.MainMenu(LoggedInCustomer); // Skipping login for testing
        }

        public override void Login()
        {

            LoginStatus loginStatus = LoginStatus.Initial;
            while (loginStatus != LoginStatus.Success)
            {
                (string loginID, string password) loginDetails = View.Login(loginStatus);

                if (loginStatus == LoginStatus.MaxAttempts)
                {
                    Exit();
                    return; // TODO Remove this when I figure out how to properly exit
                }

                try
                {
                    LoggedInCustomer = Engine.LoginAttempt(loginDetails.loginID, loginDetails.password);
                    loginStatus = LoginStatus.Success;
                }
                catch (LoginFailedException e)
                {
                    loginStatus = LoginStatus.IncorrectPassword;
                }
                catch (LoginAttemptsExceededException e)
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
            (Account sourceAccount, Account destinationAccount, double amount) transferDetails = View.Transfer(Engine.GetAccounts(LoggedInCustomer));

            if (transferDetails.sourceAccount != null)
            {
                bool transferResult = Engine.MakeTransfer(transferDetails.sourceAccount, transferDetails.destinationAccount, transferDetails.amount);
                if (transferResult)
                {
                    View.TransferResponse(transferResult, transferDetails.sourceAccount, transferDetails.destinationAccount, transferDetails.amount);
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
            (Account account, TransactionType transactionType, double amount) transactionDetails = View.AtmTransaction(Engine.GetAccounts(LoggedInCustomer));
            if (transactionDetails.account == null)
            {
                View.MainMenu(LoggedInCustomer);
            }
            else
            {
                (bool wasSuccess, double endingBalance) transactionResult = Engine.MakeTransaction(transactionDetails.account, transactionDetails.transactionType, transactionDetails.amount);
                View.TransactionResponse(transactionResult.wasSuccess, transactionDetails.transactionType, transactionDetails.amount, transactionResult.endingBalance);
                AtmTransaction();
            }
        }

        public override void Exit()
        {
            // TODO Put in the real method for exitting
        }
    }
}
