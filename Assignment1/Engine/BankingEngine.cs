using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public interface BankingEngine
    {
        public void Start(BankingController controller);

        public Customer LoginAttempt(string loginID, string password);

        public List<Account> GetAccounts(Customer customer);
        public List<Transaction> GetTransactions(Account account);

        public bool MakeTransfer(Account sourceAccount, Account destinationAccount, double amount);

        public (bool wasSuccess, double endingBalance) MakeTransaction(Account account, TransactionType transactionType, double amount);

    }

    public class LoginAttemptsExcededException : Exception
    {
        public LoginAttemptsExcededException() : base("User reached the max number of login attempts allowed")
        {

        }
    }

    public class LoginFailedException : Exception
    {
        public LoginFailedException() : base("Login attempt failed")
        {
        }
    }


}
