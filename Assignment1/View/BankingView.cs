﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public interface BankingView
    {
        public void Start(BankingController controller);
        public (string login, string password) Login();

        public void LoginFailed();
        public void MainMenu();

        public void LoginAttemptedExceded();

        public void ShowAccountBalances(List<Account> accouts);

        public Account SelectAccount(List<Account> accounts);

        public void showTransactions(List<Transaction> transactions);
        public void Clear();

    }
}
