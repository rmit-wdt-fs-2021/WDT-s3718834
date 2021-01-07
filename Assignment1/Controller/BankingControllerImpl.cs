﻿using System.Collections.Generic;

namespace Assignment1
{
    public class BankingControllerImpl : BankingController
    {

        public User LoggedInUser { get; private set; }

        public BankingControllerImpl(BankingEngine engine, BankingView view) : base(engine, view)
        {
        }


        public override void Start()
        {
            Engine.Start(this);
            View.Start(this);

           Login();
/*            LoggedInUser = new User();
            View.MainMenu(); // Skipping login for testing*/
        }

        public override void Login()
        {

            LoginStatus loginStatus = LoginStatus.Initial;
            while(loginStatus != LoginStatus.Success)
            {
                (string loginID, string password) loginDetails = View.Login(loginStatus);

                if(loginStatus == LoginStatus.MaxAttempts)
                {
                    Exit();
                    return; // TODO Remove this when I figure out how to properly exit
                }

                try
                {
                    LoggedInUser = Engine.LoginAttempt(loginDetails.loginID, loginDetails.password);
                    loginStatus = LoginStatus.Success;
                }
                catch (LoginFailedException e)
                {
                    loginStatus = LoginStatus.IncorrectPassword;
                }
                catch (LoginAttemptsExcededException e)
                {
                    loginStatus = LoginStatus.MaxAttempts;
                }
            }

            View.MainMenu(LoggedInUser);

            
        }

        public override void TransactionHistory()
        {
            View.ShowTransactions(Engine.GetAccounts(LoggedInUser));
            View.MainMenu(LoggedInUser);
        }

        public override void Transfer()
        {
            /*            (Account sourceAccount, Account destinationAccount, double amount) transferDetails = View.GetTransferDetails(Engine.GetAccounts(LoggedInUser));
                        bool transferSuccessful = Engine.MakeTransfer(transferDetails.sourceAccount, transferDetails.destinationAccount, transferDetails.amount);
                        if(transferSuccessful)
                        {
                            View.TransferSuccessful();
                            View.MainMenu(LoggedInUser);
                        } else
                        {
                            View.TransferFailed();
                            View.GetTransferDetails(Engine.GetAccounts(LoggedInUser));
                        }*/

            View.WorkInProgress();
            View.MainMenu(LoggedInUser);
        }

        public override List<Transaction> GetTransactions(Account account)
        {
            return Engine.GetTransactions(account);
        }


        // TODO Implement
        public override void ModifyProfile()
        {
            View.WorkInProgress();
            View.MainMenu(LoggedInUser);
        }

        // TODO Implement
        public override void ApplyForLoan()
        {
            View.WorkInProgress();
            View.MainMenu(LoggedInUser);
        }

        public override void Logout()
        {
            LoggedInUser = null;
            View.Clear();
            Login();
        }

        public override void AtmTransaction()
        {
            View.WorkInProgress();
            View.MainMenu(LoggedInUser);
        }

        public override void Exit()
        {
            // TODO Put in the real method for exitting
        }
    }
}
