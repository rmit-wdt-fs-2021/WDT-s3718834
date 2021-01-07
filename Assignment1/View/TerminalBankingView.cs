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
            Clear();
            Console.WriteLine("-- Login -- \n");

            if (loginStatus == LoginStatus.MaxAttempts)
            {
                Console.WriteLine("Exceded the number of login attempts. Press any key to exit");
                Console.ReadKey();
                return (null, null);
            }
            else
            {
                if (loginStatus == LoginStatus.IncorrectID)
                {
                    Console.WriteLine("Provided login ID was incorrect. A login ID must be 8 digits\n");
                }
                else if (loginStatus == LoginStatus.IncorrectPassword)
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

                }
                else
                {
                    return Login(LoginStatus.IncorrectID);
                }
            }



        }

        public void MainMenu(User loggedInUser)
        {
            Clear();
            Console.WriteLine("-- Main Menu --\n");

            Console.WriteLine($"Welcome {loggedInUser.Name}\n");

            Console.WriteLine("Please provide input one of the options below (single character):\n" +
                "1: ATM Transaction\n" +
                "2: Transfer\n" +
                "3: My Statements\n" +
                "4: Modify profile\n" +
                "5: Apply for loan\n" +
                "6: Logout\n" +
                "7: Exit\n");


            switch (GetAcceptableInput(7))
            {
                case 1:
                    Controller.AtmTransaction();
                    break;
                case 2:
                    Controller.Transfer();
                    break;
                case 3:
                    Controller.TransactionHistory();
                    break;
                case 4:
                    Controller.ModifyProfile();
                    break;
                case 5:
                    Controller.ApplyForLoan();
                    break;
                case 6:
                    Controller.Logout();
                    break;
                case 7:
                    Controller.Exit();
                    break;
                default:
                    Console.WriteLine("Fatal Error: Something happened in the main menu and an invalid input got through");
                    break;
            }



        }

        private int GetAcceptableInput(int choices)
        {
            (bool isAcceptable, int parsedValue) inputStatus;
            do
            {
                Console.Write("Your choice: ");

                string input = Console.ReadLine();

                inputStatus = IsAcceptableMenuInput(input, choices);

                if (!inputStatus.isAcceptable)
                {
                    Console.WriteLine("\nPlease provide a correct input\n");
                }
            } while (!inputStatus.isAcceptable);

            return inputStatus.parsedValue;
        }

        private (bool isAcceptable, int parsedValue) IsAcceptableMenuInput(string input, int maxInput)
        {
            int numericalInput;
            if (input.Length == 1 && int.TryParse(input, out numericalInput))
            {
                for (int i = 1; i <= maxInput; i++)
                {
                    if (i == numericalInput)
                    {
                        return (true, numericalInput);
                    }
                }
            }

            return (false, 0);

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
            if (accounts.Count == 1)
            {
                destinationAccount = accounts[0];
                Console.WriteLine($"Transfering to account: {destinationAccount.AccountNumber} ({getFullAccountType(destinationAccount.AccountType)})");
            }
            else
            {
                destinationAccount = SelectAccount(accounts);
            }

            string transferAmount = GetValue("Please input transfer amount\n", "Please input a valid transfer amount\n", GenerateTransferAmountLambda(sourceAccount.Balance));

            return (sourceAccount, destinationAccount, double.Parse(transferAmount));
        }

        private String getFullAccountType(char accountType)
        {
            return accountType == 'S' ? "Savings" : "Checking";
        }

        public void Clear()
        {
            Console.Clear();
        }


        public void WorkInProgress()
        {
            Console.Clear();
            Console.WriteLine("Feature currently not implemented. Press any key to continue");
            Console.ReadKey();
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
                if (double.TryParse(input, out transferAmount))
                {
                    return (transferAmount < availableBalance && transferAmount > 0);
                }
                else
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

        public (Account account, TransactionType transactionType, double amount) AtmTransaction(List<Account> accounts)
        {
            Clear();
            Console.WriteLine("-- ATM Transaction -- \n");
            Console.WriteLine("Please select an account\n");

            for (int i = 0; i < accounts.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {accounts[i].AccountNumber} ({getFullAccountType(accounts[i].AccountType)}), {accounts[i].Balance}");
            }
            Console.WriteLine($"{accounts.Count + 1}: Cancel");

            int accountSelectInput = GetAcceptableInput(accounts.Count + 1);

            if (accountSelectInput == accounts.Count + 1)
            {
                return (null, TransactionType.Deposit, 0);
            }

            Account selectedAccount = accounts[accountSelectInput - 1];

            Clear();
            Console.WriteLine("-- ATM Transaction -- \n");
            Console.WriteLine($"Using account: {selectedAccount.AccountNumber} ({getFullAccountType(selectedAccount.AccountType)}), ${selectedAccount.Balance}\n");
            Console.WriteLine("Please select a transaction type\n");
            Console.WriteLine("1: Deposit\n2: Withdraw\n3: Different account\n4: Main menu");
            int transactionTypeInput = GetAcceptableInput(4);

            switch(transactionTypeInput)
            {
                case 1:
                    Clear();
                    Console.WriteLine("-- ATM Transaction -- \n");
                    Console.WriteLine($"Using account: {selectedAccount.AccountNumber} ({getFullAccountType(selectedAccount.AccountType)}), ${selectedAccount.Balance}\n");

                    while(true)
                    {
                        Console.WriteLine("Please enter how much you would like to deposit (Input nothing to return): ");
                        string depositAmountInput = Console.ReadLine();

                        if (depositAmountInput == "")
                        {
                            return AtmTransaction(accounts); // TODO Look into a way to return to transaction selection rather then acount selection
                        }

                        double depositAmount;
                        if (!double.TryParse(depositAmountInput, out depositAmount) || depositAmount < 0)
                        {
                            Console.WriteLine("\nPlease input a correct deposit amount\n");
                        } else
                        {
                            return (selectedAccount, TransactionType.Deposit, depositAmount);
                        }
                    }
                case 2:
                    Clear();
                    Console.WriteLine("-- ATM Transaction -- \n");
                    Console.WriteLine($"Using account: {selectedAccount.AccountNumber} ({getFullAccountType(selectedAccount.AccountType)}), ${selectedAccount.Balance}\n");

                    while (true)
                    {
                        Console.WriteLine("Please enter how much you would like to withdraw (Input nothing to return): ");
                        string withdrawAmountInput = Console.ReadLine();

                        if (withdrawAmountInput == "")
                        {
                            return AtmTransaction(accounts); // TODO Look into a way to return to transaction selection rather then acount selection
                        }

                        double withdrawAmount;
                        if (!double.TryParse(withdrawAmountInput, out withdrawAmount) || withdrawAmount < 0 || withdrawAmount >= selectedAccount.Balance)
                        {
                            Console.WriteLine("\nPlease input a correct deposit amount\n");
                        }
                        else
                        {
                            return (selectedAccount, TransactionType.Withdraw, withdrawAmount);
                        }
                    }
                case 3:
                    return AtmTransaction(accounts);
                case 4:
                    return (null, TransactionType.Deposit, 0);
                default:
                    Console.WriteLine("Fatal Error: Incorrect input go through in ATM Transaction menu");
                    return (null, TransactionType.Deposit, 0);
            }
        }

        public void TransactionResponse(bool wasSuccess, TransactionType transactionType, double amount, double newBalance)
        {
            if(wasSuccess)
            {
                Console.WriteLine($"{transactionType} of ${amount} was success\n");
                Console.WriteLine($"Balance of account is now ${newBalance}");
            } else
            {
                Console.WriteLine($"{transactionType} of ${amount} failed. Contact customer service for assistance\n");
            }

            Console.WriteLine("Press any key to return to account selection");
            Console.ReadKey();
        }

    }


}
