using Assignment1.Controller;
using Assignment1.Engine;
using Assignment1.View;

namespace Assignment1
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            new BankingControllerImpl(new BankingEngineImpl(), new TerminalBankingView()).Start();
        }
    }
}
