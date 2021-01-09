using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public class BankingEngineImpl : BankingEngine
    {

        private BankingController Controller { get; set; }

        public void Start(BankingController controller)
        {
            this.Controller = controller;
            DatabaseProxy databaseProxy = new DatabaseProxy();
            bool test = databaseProxy.CustomersExist();

            return;
        }

        public Customer LoginAttempt(string loginID, string password)
        {
            // Temporary password checking for terminal testing
            if (password.Equals("a"))
            {
                throw new LoginFailedException();
            }
            else if (password.Equals("b"))
            {
                throw new LoginAttemptsExcededException();
            }

            return new Customer(12345678, "bob", "9 bob street", "melbourne", "3000");
        }

        public List<Account> GetAccounts(Customer customer)
        {

            List<Account> accounts = new List<Account>();

            accounts.Add(new Account(12346789, 'S', customer.CustomerID, 100000.01));
            accounts.Add(new Account(987654321, 'C', customer.CustomerID, 1.43));
            accounts.Add(new Account(312312612, 'C', customer.CustomerID, 420.43));

            return accounts;
        }

        public List<Transaction> GetTransactions(Account account)
        {
            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction(1, 'D', 987654321, 123012302, 10.01, "deposit money", DateTime.Now));
            transactions.Add(new Transaction(1, 'S', 987654321, 987654321, 0.1, "withdraw charge", DateTime.Now));
            transactions.Add(new Transaction(1, 'W', 987654321, 987654321, 20.02, "withdraw money", DateTime.Now));
            transactions.Add(new Transaction(1, 'S', 987654321, 987654321, 0.2, "transfer charge", DateTime.Now));
            transactions.Add(new Transaction(1, 'T', 987654321, 123456789, 40.03, "transfer to savings", DateTime.Now));
            transactions.Add(new Transaction(1, 'D', 987654321, 123012302, 10.01, "deposit money", DateTime.Now));
            transactions.Add(new Transaction(1, 'S', 987654321, 987654321, 0.1, "withdraw charge", DateTime.Now));
            transactions.Add(new Transaction(1, 'W', 987654321, 987654321, 20.02, "withdraw money", DateTime.Now));
            transactions.Add(new Transaction(1, 'S', 987654321, 987654321, 0.2, "transfer charge", DateTime.Now));



            return transactions;
        }

        public bool MakeTransfer(Account sourceAccount, Account destinationAccount, double amount)
        {
            return amount <= sourceAccount.Balance;
        }

        public (bool wasSuccess, double endingBalance) MakeTransaction(Account account, TransactionType transactionType, double amount)
        {
            if (amount == 2.50)
            {
                return (false, 10000);
            }
            else
            {
                return (true, 10000);
            }

        }
    }
}
