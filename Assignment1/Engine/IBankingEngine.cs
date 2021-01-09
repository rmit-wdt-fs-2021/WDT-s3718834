using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment1.Controller;
using Assignment1.Enum;
using Assignment1.POCO;

namespace Assignment1.Engine
{
    public interface IBankingEngine
    {
        public Task Start(BankingController controller);

        public Task<Customer> LoginAttempt(string loginId, string password);

        public Task<List<Account>> GetAccounts(Customer customer);
        public List<Transaction> GetTransactions(Account account);

        public bool MakeTransfer(Account sourceAccount, Account destinationAccount, decimal amount);

        public (bool wasSuccess, decimal endingBalance) MakeTransaction(Account account, TransactionType transactionType, decimal amount);

    }

    public class LoginAttemptsExceededException : Exception
    {
        public LoginAttemptsExceededException() : base("User reached the max number of login attempts allowed")
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
