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
        private FieldValidator accountNumberValidator = login => login.Length == 4 && int.TryParse(login, out var discard);
        private FieldValidator passwordValidator = login => true;


        public void Start(BankingController controller)
        {
            Console.WriteLine("Welcome to MCBA Banking Applicaiton");
            this.Controller = controller;
        }

        public (string login, string password) Login(LoginStatus loginStatus)
        {
            Console.Clear();
            Console.WriteLine("-- Login -- \n");

            if (loginStatus == LoginStatus.MaxAttempts)
            {
                Console.WriteLine("Exceded the number of login attempts. Press any key to exit");
                Console.ReadKey();
                return (null, null);
            }
            else
            {
                if(loginStatus == LoginStatus.IncorrectID)
                {
                    Console.WriteLine("Provided login ID was incorrect. A login ID must be 8 digits\n");
                } else if(loginStatus == LoginStatus.IncorrectPassword)
                {
                    Console.WriteLine("Provided login ID and password do not match\n");
                }

                Console.WriteLine("Please provide your details below:");

                Console.Write("Login ID: ");
                string loginID = Console.ReadLine();

                if (loginIdValidator(loginID))
                {
                    Console.Write("Password: ");

                    StringBuilder passwordBuilder = new StringBuilder();
                    ConsoleKeyInfo inputKey = System.Console.ReadKey();
                    while (inputKey.Key != ConsoleKey.Enter)
                    {
                        passwordBuilder.Append(inputKey.KeyChar);
                        inputKey = System.Console.ReadKey();
                    }

                    return (loginID, passwordBuilder.ToString());

                } else
                {
                    return Login(LoginStatus.IncorrectID);
                }
            }
            


        }

        public void MainMenu()
        {

            char[] acceptableCharacters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G'};

            string input = GetValue("Please provide input one of the options below (single character):\n" +
                "A: Check balance\n" +
                "B: Transaction History\n" +
                "C: Make transfer\n" +
                "D: Modify profile\n" +
                "E: Apply for loan\n" + 
                "F: Logout\n" +
                "G: Exit\n",
                "Please provide on a single character. Accepted characters are listed on the left\n",

                GenerateAcceptableInputsLambda(new List<char>(acceptableCharacters)));

            switch (input.ToUpper())
            {
                case "A":
                    Controller.CheckBalance();
                    break;
                case "B":
                    Controller.TransactionHistory();
                    break;
                case "C":
                    Controller.Transfer();
                    break;
                case "D":
                    Controller.ModifyProfile();
                    break;
                case "E":
                    Controller.ApplyForLoan();
                    break;
                case "F":
                    Controller.Logout();
                    break;
                case "G":
                    Controller.Exit();
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
                Console.WriteLine($"Account number: {account.AccountNumber} ({getFullAccountType(account.AccountType)})");
                Console.WriteLine($"Balance: {account.Balance}\n");
            }
        }

        private Account SelectAccount(List<Account> accounts)
        {

            List<Char> acceptableInputs = generateAlphabetArray(accounts.Count);
            StringBuilder stringBuilder = new StringBuilder("\nPlease select an account from the list below\n");

            for (int i = 0; i < accounts.Count; i++)
            {
                Account account = accounts[i];
                stringBuilder.Append($"{acceptableInputs[i]}: {account.AccountNumber} ({getFullAccountType(account.AccountType)}), ${account.Balance}\n");
            }

            string input = GetValue(stringBuilder.ToString(),
                "Please provide on a single character. Accepted characters are listed on the left\n",
                GenerateAcceptableInputsLambda(acceptableInputs)).ToUpper();

            // Convert response but to numerical and subtract the value of A. Bring the value to array indexes.
            return accounts[((int)char.Parse(input)) - 65]; 

        }


        public void ShowTransactions(List<Account> accounts)
        {
            Account account = SelectAccount(accounts);

            foreach (Transaction transaction in Controller.GetTransactions(account))
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

        public (Account sourceAccount, Account destinationAccount, double amount) GetTransferDetails(List<Account> originalAccounts)
        {
            List<Account> accounts = new List<Account>(originalAccounts);

            Account sourceAccount = SelectAccount(accounts);

            accounts.Remove(sourceAccount);

            Account destinationAccount;
            if(accounts.Count == 1)
            {
                destinationAccount = accounts[0];
                Console.WriteLine($"Transfering to account: {destinationAccount.AccountNumber} ({getFullAccountType(destinationAccount.AccountType)})");
            } else
            {
                destinationAccount = SelectAccount(accounts);
            }

            string transferAmount = GetValue("Please input transfer amount\n", "Please input a valid transfer amount\n", GenerateTransferAmountLambda(sourceAccount.Balance));

            return (sourceAccount, destinationAccount, double.Parse(transferAmount));
        }

        public void TransferSuccessful()
        {
            Console.WriteLine("Transfer successful\n");
        }

        public void TransferFailed()
        {
            Console.WriteLine("Transfer failed\n");
        }

        private String getFullAccountType(char accountType)
        {
            return accountType == 'S' ? "Savings" : "Checking";
        }

        public void Clear()
        {
            Console.Clear();
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


        private FieldValidator GenerateAcceptableInputsLambda(List<Char> acceptableCharacters)
        {
            return login => login.Length == 1 && acceptableCharacters.Contains(login.ToUpper().ToCharArray()[0]);
        }

        private FieldValidator GenerateAcceptableInputsLambda(List<String> acceptableCharacters)
        {
            return login => acceptableCharacters.Contains(login.ToUpper());
        }

        private FieldValidator GenerateTransferAmountLambda(double availableBalance)
        {
            return input =>
            {
                double transferAmount;
                if(double.TryParse(input, out transferAmount))
                {
                    return (transferAmount < availableBalance && transferAmount > 0);
                } else
                {
                    return false;
                }
               
            };
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
