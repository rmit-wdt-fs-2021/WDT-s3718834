using System;
using System.Collections.Generic;
using System.Text;
using Assignment1.Engine;
using Assignment1.POCO;
using Assignment1.View;

namespace Assignment1
{
    public abstract class BankingController
    {
        public IBankingEngine Engine { get; protected set; }
        public IBankingView View { get; protected set; }

        public BankingController(IBankingEngine engine, IBankingView view)
        {
            this.Engine = engine;
            this.View = view;
        }

        public abstract void Start();
        public abstract void Login();
        public abstract void AtmTransaction();
        public abstract void Transfer();
        public abstract void TransactionHistory();
        public abstract void ModifyProfile();
        public abstract void ApplyForLoan();
        public abstract void Logout();
        public abstract void Exit();
        public abstract List<Transaction> GetTransactions(Account account);
    }
}
