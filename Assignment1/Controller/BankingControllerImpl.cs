using System.Collections.Generic;

namespace Assignment1
{
    public class BankingControllerImpl : BankingController
    {

        public User LoggedInUser { get; private set; }

        public BankingControllerImpl(BankingEngine engine, BankingView view) : base(engine, view)
        {
        }


        public override void Start()
        {
            Engine.Start(this);
            View.Start(this);

            //login();
            LoggedInUser = new User();
            View.MainMenu(); // Skipping login for testing
        }

        public override void Login()
        {
            (string loginID, string password) loginDetails = View.Login();

            try
            {
                LoggedInUser = Engine.LoginAttempt(loginDetails.loginID, loginDetails.password);
                View.MainMenu();
            } catch (LoginFailedException e)
            {
                View.LoginFailed();
                Login();
            } catch (LoginAttemptsExcededException e)
            {
                View.LoginAttemptedExceded();
            }
            
        }

        public override void CheckBalance()
        {
            View.ShowAccountBalances(Engine.GetAccounts(LoggedInUser));
            View.MainMenu();
        }

        public override void TransactionHistory()
        {
            Account selectedAccount = View.SelectAccount(Engine.GetAccounts(LoggedInUser));
            List<Transaction> transactions = Engine.GetTransactions(selectedAccount);
            View.showTransactions(transactions);
            View.MainMenu();
        }

        public override void Transaction()
        {
            throw new System.NotImplementedException();
        }

        public override void Transfer()
        {
            throw new System.NotImplementedException();
        }

        public override void ModifyProfile()
        {
            throw new System.NotImplementedException();
        }

        public override void ApplyForLoan()
        {
            throw new System.NotImplementedException();
        }

        public override void Logout()
        {
            LoggedInUser = null;
            View.Clear();
            View.Login();
        }
    }
}
