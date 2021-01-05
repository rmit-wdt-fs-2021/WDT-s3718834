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

        public abstract void start();
        public abstract void login();

    }
}
