using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public interface BankingView
    {
        public void Start();
        public (string login, string password) Login();

        public void LoginFailed();
        public void MainMenu();

        public void LoginAttemptedExceded();

    }
}
