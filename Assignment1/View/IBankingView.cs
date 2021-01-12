using System.Collections.Generic;
using Assignment1.Controller;
using Assignment1.Enum;
using Assignment1.POCO;

namespace Assignment1.View
{
    public interface IBankingView
    {
        public void Start(BankingController controller);
        public void Login();

        public void MainMenu(in Customer loggedInCustomer);

        public void ShowTransactions(in List<Account> accounts);
        public void Clear();
        public void AtmTransaction(in List<Account> accounts);
        public void Transfer(in List<Account> accounts);
        public void Loading();
    }
}
