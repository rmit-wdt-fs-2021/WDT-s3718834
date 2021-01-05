using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    class TerminalBankingView : BankingView
    {

        private BankingController Controller { get; set; }


        // TODO Is there a better way to do this?
        delegate bool FieldValidator(String loginId);
        private FieldValidator loginIdValidator = login => login.Length == 8 && int.TryParse(login, out var discard);
        private FieldValidator passwordValidator = login => true;


        public void Start(BankingController controller)
        {
            Console.WriteLine("Welcome to MCBA Banking Applicaiton");
            this.Controller = controller;
        }

        public (string login, string password) Login()
        {
            string loginId = GetValue("Please provide your loginID\n", "LoginID must all digits and 8 characters in length\n", loginIdValidator);
            string password = GetValue("Please provide your password\n", "Invalid password provided", passwordValidator);

            return (loginId, password);

        }

        public void LoginFailed()
        {
            Console.WriteLine("Login attempt failed");
        }

        public void MainMenu()
        {

            char[] acceptableCharacters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G'};

            string input = GetValue("Please provide input one of the options below (single character):\n" +
                "A: Check balance\n" +
                "B: Transaction History\n" +
                "C: Make transaction\n" + "" +
                "D: Make transfer\n" +
                "E: Modify profile\n" +
                "F: Apply for loan\n" + 
                "G: Logout\n",
                "Please provide on a single character. Accepted characters are listed on the left\n",

                generateAcceptableInputsLambda(new List<char>(acceptableCharacters)));

            switch (input.ToUpper())
            {
                case "A":
                    Controller.CheckBalance();
                    break;
                case "B":
                    Controller.TransactionHistory();
                    break;
                case "C":
                    Controller.Transaction();
                    break;
                case "D":
                    Controller.Transfer();
                    break;
                case "E":
                    Controller.ModifyProfile();
                    break;
                case "F":
                    Controller.ApplyForLoan();
                    break;
                case "G":
                    Controller.Logout();
                    break;
                default:
                    Console.WriteLine("Fatal Error: Something happened in the main menu and an invalid input got through");
                    break;
            }

        }

        public void ShowAccountBalances(List<Account> accounts)
        {
            foreach (Account account in accounts)
            {
                String accountType;
                if (account.AccountType == 'S')
                {
                    accountType = "Savings";
                }
                else
                {
                    accountType = "Checking";
                }
                Console.WriteLine($"Account number: {account.AccountNumber} ({accountType})");
                Console.WriteLine($"Balance: {account.Balance}\n");
            }
        }

        public Account SelectAccount(List<Account> accounts)
        {

            List<Char> acceptableInputs = generateAlphabetArray(accounts.Count);
            StringBuilder stringBuilder = new StringBuilder("\nPlease select an account from the list below\n");

            for (int i = 0; i < accounts.Count; i++)
            {
                Account account = accounts[i];

                String accountType;
                if (account.AccountType == 'S')
                {
                    accountType = "Savings";
                }
                else
                {
                    accountType = "Checking";
                }

                stringBuilder.Append($"{acceptableInputs[i]}: {account.AccountNumber} ({accountType}), ${account.Balance}\n");
            }

            string input = GetValue(stringBuilder.ToString(),
                "Please provide on a single character. Accepted characters are listed on the left\n",
                generateAcceptableInputsLambda(acceptableInputs)).ToUpper();

            // Convert response but to numerical and subtract the value of A. Bring the value to array indexes.
            return accounts[((int)char.Parse(input)) - 65]; 

        }


        public void showTransactions(List<Transaction> transactions)
        {
            foreach (Transaction transaction in transactions)
            {
                Console.WriteLine("*************************************************************\n" + 
                    $"Transaction ID:\t\t{transaction.TransactionID}\n" +
                    $"Transaction type:\t{transaction.TransactionType}\n" +
                    $"Source account #:\t{transaction.SourceAccount}\n" +
                    $"Destination account #:\t{transaction.DestinationAccountNumber}\n" +
                    $"Amount:\t\t\t{transaction.Amount}\n" +
                    $"Comment:\t\t{transaction.Comment}\n" +
                    $"Time:\t\t\t{transaction.TransactionTimeUtc}\n");
            }
            Console.WriteLine("*************************************************************");
        }

        void LoginAttemptedExceded()
        {
            Console.WriteLine("Exceded max password attempts. Press Enter to close window");
            Console.ReadLine(); // Stops the terminal from closing immediately, allowing the user to read the output
        }

        private List<Char> generateAlphabetArray(int length)
        {
            List<Char> alphabet = new List<char>();

            for (int i = 0; i < length; i++)
            {
                alphabet.Add(Convert.ToChar(65 + i));
            }

            return alphabet;
        }


        private FieldValidator generateAcceptableInputsLambda(List<Char> acceptableCharacters)
        {
            return login => login.Length == 1 && acceptableCharacters.Contains(login.ToUpper().ToCharArray()[0]);
        }

        private FieldValidator generateAcceptableInputsLambda(List<String> acceptableCharacters)
        {
            return login => acceptableCharacters.Contains(login.ToUpper());
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

        void BankingView.LoginAttemptedExceded()
        {
            throw new NotImplementedException();
        }


    }
}
