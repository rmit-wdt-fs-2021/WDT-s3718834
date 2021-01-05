using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public class BankingEngineImpl : BankingEngine
    {
        public User LoginAttempt(string loginID, string password)
        {
            if(password.Equals("a"))
            {
                throw new LoginFailedException();
            } else if(password.Equals("b"))
            {
                throw new LoginAttemptsExcededException();
            }

            return null;
        }
    }
}
