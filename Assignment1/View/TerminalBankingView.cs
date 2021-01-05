using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    class TerminalBankingView : BankingView
    {

        // TODO Is there a better way to do this?
        delegate bool FieldValidator(String loginId);
        private FieldValidator loginIdValidator = login => login.Length == 8 && int.TryParse(login, out var discard); 
        private FieldValidator passwordValidator = login => true;


        void BankingView.Start()
        {
            Console.WriteLine("Welcome to MCBA Banking Applicaiton");
        }

        (string login, string password) BankingView.Login()
        {
            string loginId = GetValue("Please provide your loginID\n", "LoginID must all digits and 8 characters in length\n", loginIdValidator);
            string password = GetValue("Please provide your password\n", "Invalid password provided", passwordValidator);

            return (loginId, password);

        }

        void BankingView.LoginFailed()
        {
            Console.WriteLine("Login attempt failed");
        }

        void BankingView.MainMenu()
        {
            Console.WriteLine("Main menu here. Press Enter to close window");
            Console.ReadLine();  // Stops the terminal from closing immediately, allowing the user to read the output
        }

        void BankingView.LoginAttemptedExceded()
        {
            Console.WriteLine("Exceded max password attempts. Press Enter to close window");
            Console.ReadLine(); // Stops the terminal from closing immediately, allowing the user to read the output
        }



        /*
        * Endlessly attempts to get a valid value from the user.
        * Does this by using the GetValidatedConsoleInput() method, see it for more details.
        */
        private string GetValue(string requestMessage, string failMessage, FieldValidator validator)
        {
            (bool wasSuccess, string result) result;
            do
            {
                result = GetValidatedConsoleInput(requestMessage, failMessage, validator);
            } while (!result.wasSuccess);

            return result.result;
        }


        /*
         * Repeats an attempt to get a valid value from the user. Repeating the number of times provided by the user. 
         * Does this by using the GetValidatedConsoleInput() method, see it for more details.
        */
        private (bool wasSuccess, string result) AttemptToGetValue(string requestMessage, string failMessage, int attempts, FieldValidator validator)
        {
            for (int i = 0; i < attempts; i++)
            {
                (bool wasSuccess, string result) result = GetValidatedConsoleInput(requestMessage, failMessage, validator);
                if (result.wasSuccess)
                {
                    return result;
                }
            }

            return (false, "");
        }


        /*
         * Gets a value from the console validating with the provided validator. Uses the provided messages to direct users
        */
        private (bool wasSuccess, string result) GetValidatedConsoleInput(string requestMessage, string failMessage, FieldValidator validator)
        {
            Console.Write(requestMessage);
            string input = Console.ReadLine();
            if (validator(input))
            {
                return (true, input);
            }
            else
            {
                Console.Write(failMessage);
                return (false, input);
            }
        }

    }
}
