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
        private Customer _loggedInCustomer;

        public BankingControllerImpl(IBankingEngine engine, IBankingView view) : base(engine, view)
        {
        }


        public override void Start()
        {
            View.Start(this);
            PerformVoidWithLoading(Engine.Start(this));

            Login();
            View.MainMenu(_loggedInCustomer);
        }

        public override void Login()
        {
            View.MainMenu(_loggedInCustomer);
        }

        public override int ValidateLogin(int loginId, string password)
        {
            _loggedInCustomer = PerformWithLoading(Engine.LoginAttempt(loginId, password));

            return _loggedInCustomer.CustomerId;
        }

        public override (bool wasSuccess, decimal newBalance) MakeAtmTransaction(Account account,
            TransactionType transactionType,
            decimal amount)
        {
            var (wasSuccess, endingBalance) =
                PerformWithLoading(Engine.MakeTransaction(account, transactionType, amount));

            return (wasSuccess, endingBalance);
        }

        public override void TransactionHistory()
        {
            View.ShowTransactions(PerformWithLoading(Engine.GetAccounts(_loggedInCustomer)));
            View.MainMenu(_loggedInCustomer);
        }

        public override void Transfer()
        {
            View.Transfer(PerformWithLoading(Engine.GetAccounts(_loggedInCustomer)));

            View.MainMenu(_loggedInCustomer);
        }

        public override (bool success, Account updatedSourceAccount, Account updatedDestinationAccount) MakeTransfer(
            Account sourceAccount,
            Account destinationAccount, decimal amount)
        {
            var (success, updatedSourceAccount, updatedDestinationAccount) =
                PerformWithLoading(Engine.MakeTransfer(sourceAccount, destinationAccount, amount));

            return (success, updatedSourceAccount,
                updatedDestinationAccount);
        }

        public override List<Transaction> GetTransactions(Account account)
        {
            return PerformWithLoading(Engine.GetTransactions(account));
        }

        public override Account GetAccount(int accountNumber)
        {
            return PerformWithLoading(Engine.GetAccount(accountNumber));
        }

        private T PerformWithLoading<T>(Task<T> task)
        {
            View.Loading();
            task.Wait();
            return task.Result;
        }

        private void PerformVoidWithLoading(Task task)
        {
            View.Loading();
            task.Wait();
        }

        // TODO Implement
        public override void ModifyProfile()
        {
            View.WorkInProgress();
            View.MainMenu(_loggedInCustomer);
        }

        // TODO Implement
        public override void ApplyForLoan()
        {
            View.WorkInProgress();
            View.MainMenu(_loggedInCustomer);
        }

        public override void Logout()
        {
            _loggedInCustomer = null;
            View.Clear();
            Login();
        }

        public override void AtmTransaction()
        {
            var getAccountsTask = Engine.GetAccounts(_loggedInCustomer);

            View.Loading();
            getAccountsTask.Wait();

            View.AtmTransaction(getAccountsTask.Result);

            View.MainMenu(_loggedInCustomer);
        }

        public override void Exit()
        {
            // TODO Put in the real method for exiting
        }
    }
}