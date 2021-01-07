using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public interface BankingView
    {
        public void Start(BankingController controller);
        public (string login, string password) Login(LoginStatus loginStatus);

        public void MainMenu(User loggedInUser);

        public void ShowTransactions(List<Account> accounts);
        public void Clear();
        public void WorkInProgress();

        public (Account sourceAccount, Account destinationAccount, double amount) GetTransferDetails(List<Account> accounts);

    }
}
