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
            View.start();

            login();
        }

        public override void login()
        {
            (string loginID, string password) loginDetails = View.login();

            try
            {
                LoggedInUser = Engine.loginAttempt(loginDetails.loginID, loginDetails.password);
                View.mainMenu();
            } catch (LoginFailedException e)
            {
                View.loginFailed();
            } catch (LoginAttemptsExcededException e)
            {
                View.loginAttemptedExceded();
            }
            
        }
    }
}
