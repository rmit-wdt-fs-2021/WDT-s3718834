using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public abstract class BankingController
    {
        public BankingEngine Engine { get; protected set; }
        public BankingView View { get; protected set; }

        public BankingController(BankingEngine engine, BankingView view)
        {
            this.Engine = engine;
            this.View = view;
        }

        public abstract void Start();
        public abstract void Login();
        public abstract void CheckBalance();
        public abstract void TransactionHistory();
        public abstract void Transaction();
        public abstract void Transfer();
        public abstract void ModifyProfile();
        public abstract void ApplyForLoan();
        public abstract void Logout();
    }
}
