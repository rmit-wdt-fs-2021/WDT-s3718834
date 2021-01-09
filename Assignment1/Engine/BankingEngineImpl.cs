using System;
using System.Collections.Generic;
using Assignment1.Enum;
using Assignment1.POCO;

namespace Assignment1.Engine
{
    public class BankingEngineImpl : IBankingEngine
    {

        private BankingController Controller { get; set; }

        public void Start(BankingController controller)
        {
            this.Controller = controller;
            var databaseProxy = new DatabaseProxy();
            var test = databaseProxy.CustomersExist();

            return;
        }

        public Customer LoginAttempt(string loginId, string password)
        {
            return password switch
            {
                // Temporary password checking for terminal testing
                "a" => throw new LoginFailedException(),
                "b" => throw new LoginAttemptsExceededException(),
                _ => new Customer(12345678, "bob", "9 bob street", "melbourne", "3000")
            };
        }

        public List<Account> GetAccounts(Customer customer)
        {
            var accounts = new List<Account>
            {
                new Account(12346789, 'S', customer.CustomerId, 100000.01),
                new Account(987654321, 'C', customer.CustomerId, 1.43),
                new Account(312312612, 'C', customer.CustomerId, 420.43)
            };
            
            return accounts;
        }

        public List<Transaction> GetTransactions(Account account)
        {
            var transactions = new List<Transaction>
            {
                new Transaction(1, 'D', 987654321, 123012302, 10.01, "deposit money", DateTime.Now),
                new Transaction(1, 'S', 987654321, 987654321, 0.1, "withdraw charge", DateTime.Now),
                new Transaction(1, 'W', 987654321, 987654321, 20.02, "withdraw money", DateTime.Now),
                new Transaction(1, 'S', 987654321, 987654321, 0.2, "transfer charge", DateTime.Now),
                new Transaction(1, 'T', 987654321, 123456789, 40.03, "transfer to savings", DateTime.Now),
                new Transaction(1, 'D', 987654321, 123012302, 10.01, "deposit money", DateTime.Now),
                new Transaction(1, 'S', 987654321, 987654321, 0.1, "withdraw charge", DateTime.Now),
                new Transaction(1, 'W', 987654321, 987654321, 20.02, "withdraw money", DateTime.Now),
                new Transaction(1, 'S', 987654321, 987654321, 0.2, "transfer charge", DateTime.Now)
            };



            return transactions;
        }

        public bool MakeTransfer(Account sourceAccount, Account destinationAccount, double amount)
        {
            return amount <= sourceAccount.Balance;
        }

        public (bool wasSuccess, double endingBalance) MakeTransaction(Account account, TransactionType transactionType, double amount)
        {
            if (Math.Abs(amount - 2.50) < 2)
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
