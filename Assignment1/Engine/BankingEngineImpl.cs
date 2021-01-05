using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public class BankingEngineImpl : BankingEngine
    {

        private BankingController Controller { get; set; }

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

        public void Start(BankingController controller)
        {
            this.Controller = controller;
        }
    }
}
