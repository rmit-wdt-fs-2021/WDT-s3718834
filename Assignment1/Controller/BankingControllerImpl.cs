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
            View.Login();
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

            View.Transfer(getAccountsTask.Result);

            View.MainMenu(LoggedInCustomer);
        }

        public override List<Transaction> GetTransactions(Account account)
        {
            var getTransactionsTask = Engine.GetTransactions(account);

            View.Loading();
            getTransactionsTask.Wait();

            return getTransactionsTask.Result;
        }

        public override Account GetAccount(int accountNumber)
        {
            var accountExistsTask = Engine.GetAccount(accountNumber);
            View.Loading();
            accountExistsTask.Wait();

            return accountExistsTask.Result;
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
            var getAccountsTask = Engine.GetAccounts(LoggedInCustomer);

            View.Loading();
            getAccountsTask.Wait();

            View.AtmTransaction(getAccountsTask.Result);

            View.MainMenu(LoggedInCustomer);
        }

        public override void Exit()
        {
            // TODO Put in the real method for exiting
        }
    }
}