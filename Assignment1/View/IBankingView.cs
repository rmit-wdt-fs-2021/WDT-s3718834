using System.Collections.Generic;
using Assignment1.Controller;
using Assignment1.Enum;
using Assignment1.POCO;

namespace Assignment1.View
{
    public interface IBankingView
    {
        public void Start(BankingController controller);
        public (string login, string password) Login(LoginStatus loginStatus);

        public void MainMenu(Customer loggedInCustomer);

        public void ShowTransactions(List<Account> accounts);
        public void Clear();
        public void WorkInProgress();
        public (Account account, TransactionType transactionType, decimal amount) AtmTransaction(List<Account> accounts);

        public void TransactionResponse(bool wasSuccess, TransactionType transactionType, decimal amount, decimal newBalance);
        public (Account sourceAccount, Account destinationAccount, decimal amount) Transfer(List<Account> accounts);
        public void TransferResponse(bool wasSuccess, Account sourceAccount, Account destinationAccount, decimal amount);
        public void Loading();
    }
}
