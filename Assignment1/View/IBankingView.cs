using System.Collections.Generic;
using Assignment1.Controller;
using Assignment1.Enum;
using Assignment1.POCO;

namespace Assignment1.View
{
    public interface IBankingView
    {
        public void Start(BankingController controller);
        public int Login();

        public void MainMenu(in Customer loggedInCustomer);

        public void ShowTransactions(in List<Account> accounts);
        public void Clear();
        public void WorkInProgress();
        public (Account account, TransactionType transactionType, decimal amount) AtmTransaction(in List<Account> accounts);

        public void TransactionResponse(bool wasSuccess, TransactionType transactionType, decimal amount, decimal newBalance);
        public void Transfer(in List<Account> accounts);
        public void Loading();
    }
}
