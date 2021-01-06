using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public interface BankingView
    {
        public void Start(BankingController controller);
        public (string login, string password) Login();

        public void LoginFailed();
        public void MainMenu();

        public void LoginAttemptedExceded();

        public void ShowAccountBalances(List<Account> accouts);

        public void ShowTransactions(List<Account> accounts);
        public void Clear();

        public (Account sourceAccount, Account destinationAccount, double amount) GetTransferDetails(List<Account> accounts);

        public void TransferSuccessful();

        public void TransferFailed();

    }
}
