using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public interface BankingView
    {
        public void start();
        public (string login, string password) login();

        public void loginFailed();
        public void mainMenu();

        public void loginAttemptedExceded();

    }
}
