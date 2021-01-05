using System;

namespace Assignment1
{
    class Program
    {
        static void Main(string[] args)
        {
            new BankingControllerImpl(new BankingEngineImpl(), new TerminalBankingView()).Start();
        }
    }
}
