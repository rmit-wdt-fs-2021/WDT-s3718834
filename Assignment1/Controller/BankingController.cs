﻿using System.Collections.Generic;
using Assignment1.Engine;
using Assignment1.Enum;
using Assignment1.POCO;
using Assignment1.View;

namespace Assignment1.Controller
{
    public abstract class BankingController
    {
        protected IBankingEngine Engine { get; }
        protected IBankingView View { get; }

        protected BankingController(IBankingEngine engine, IBankingView view)
        {
            this.Engine = engine;
            this.View = view;
        }

        public abstract void Start();
        public abstract void Login();
        public abstract bool ValidateLogin(string loginId, string password);
        public abstract void AtmTransaction();
        public abstract void Transfer();

        public abstract (bool success, Account updatedSourceAccount, Account updatedDestinationAccount) MakeTransfer(
            Account sourceAccount, Account destinationAccount, decimal amount);

        public abstract (bool wasSuccess, decimal newBalance) MakeAtmTransaction(Account account, TransactionType transactionType, decimal amount);

        public abstract void TransactionHistory();
        public abstract void ModifyProfile();
        public abstract void ApplyForLoan();
        public abstract void Logout();
        public abstract void Exit();
        public abstract List<Transaction> GetTransactions(Account account);
        public abstract Account GetAccount(int accountNumber);
    }
}