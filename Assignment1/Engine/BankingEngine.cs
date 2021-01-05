using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public interface BankingEngine
    {
        public void start();

        public User loginAttempt(string loginID, string password);
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
