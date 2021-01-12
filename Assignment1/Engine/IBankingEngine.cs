using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment1.Controller;
using Assignment1.Data;
using Assignment1.Enum;

namespace Assignment1.Engine
{
    public interface IBankingEngine
    {
        public Task Start(BankingController controller);

        public Task<Customer> LoginAttempt(int loginId, string password);

        public Task<List<Account>> GetAccounts(Customer customer);
        public Task<List<Transaction>> GetTransactions(Account account);

        public Task<(bool success, Account updatedSourceAccount, Account updatedDestinationAccount)> MakeTransfer(
            Account sourceAccount, Account destinationAccount, decimal amount);

        public Task<(bool wasSuccess, decimal endingBalance)> MakeTransaction(Account account,
            TransactionType transactionType, decimal amount);

        public Task<Account> GetAccount(int accountNumber);
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