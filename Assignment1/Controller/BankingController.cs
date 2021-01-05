using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public abstract class BankingController
    {
        public BankingEngine Engine { get; private set; }
        public BankingView View { get; private set; }

        public BankingController(BankingEngine engine, BankingView view)
        {
            this.Engine = engine;
            this.View = view;
        }

        public abstract void start();

    }
}
