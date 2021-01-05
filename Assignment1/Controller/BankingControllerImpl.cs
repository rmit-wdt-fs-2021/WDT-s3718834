namespace Assignment1
{
    public class BankingControllerImpl : BankingController
    {

        public User LoggedInUser { get; private set; }

        public BankingControllerImpl(BankingEngine engine, BankingView view) : base(engine, view)
        {
        }


        public override void start()
        {
            Engine.start();
            View.Start();

            login();
        }

        public override void login()
        {
            (string loginID, string password) loginDetails = View.Login();

            try
            {
                LoggedInUser = Engine.loginAttempt(loginDetails.loginID, loginDetails.password);
                View.MainMenu();
            } catch (LoginFailedException e)
            {
                View.LoginFailed();
                login();
            } catch (LoginAttemptsExcededException e)
            {
                View.LoginAttemptedExceded();
            }
            
        }
    }
}
