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
        }

        public User LoginAttempt(string loginID, string password)
        {
            // Temporary password checking for terminal testing
            if (password.Equals("a"))
            {
                throw new LoginFailedException();
            } else if(password.Equals("b"))
            {
                throw new LoginAttemptsExcededException();
            }

            return null;
        }

        public List<Account> GetAccounts(User user)
        {
           if(user.Accounts == null)
            {
                user.Accounts = new List<Account>();
                user.Accounts.Add(new Account(12346789, 'S', user.CustomerID, 100000.01));
                user.Accounts.Add(new Account(987654321, 'C', user.CustomerID, 1.43));
            }

            return user.Accounts;
        }


    }
}
