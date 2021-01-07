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
        delegate bool FieldValidator(double loginId);


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

                if (loginID.Length == 8 && int.TryParse(loginID, out _))
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

            Console.WriteLine("Please select an account\n");

            for (int i = 0; i < accounts.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {accounts[i].AccountNumber} ({getFullAccountType(accounts[i].AccountType)}), {accounts[i].Balance}");
            }
            Console.WriteLine($"{accounts.Count + 1}: Cancel");

            int accountSelectInput = GetAcceptableInput(accounts.Count + 1);

            if (accountSelectInput == accounts.Count + 1)
            {
                return null;
            }

            return accounts[accountSelectInput - 1];

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

            (bool escaped, double result) input = GetCurrencyInput("Please input transfer amount\n", "Please input a valid transfer amount\n", input => input > 0 && input <= sourceAccount.Balance);

            return (sourceAccount, destinationAccount, input.result);
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

        private (bool escaped, double result) GetCurrencyInput(string requestMessage, string failMessage, FieldValidator validator)
        {
            while (true)
            {
                Console.Write(requestMessage);
                string input = Console.ReadLine();

                if (input == "")
                {
                    return (true, 0);
                }

                double parsedInput;

                if (double.TryParse(input, out parsedInput) && validator(parsedInput))
                {
                    return (false, parsedInput);
                } else
                {
                    Console.Write(failMessage);
                }
            }
        }

        public (Account account, TransactionType transactionType, double amount) AtmTransaction(List<Account> accounts)
        {
            Clear();
            Console.WriteLine("-- ATM Transaction -- \n");
            Account selectedAccount = SelectAccount(accounts);

            if (selectedAccount == null)
            {
                return (null, TransactionType.Deposit, 0);
            }

            Clear();
            Console.WriteLine("-- ATM Transaction -- \n");
            Console.WriteLine($"Using account: {selectedAccount.AccountNumber} ({getFullAccountType(selectedAccount.AccountType)}), ${selectedAccount.Balance}\n");
            Console.WriteLine("Please select a transaction type\n");
            Console.WriteLine("1: Deposit\n2: Withdraw\n3: Different account\n4: Main menu");
            int transactionTypeInput = GetAcceptableInput(4);

            switch (transactionTypeInput)
            {
                case 1:
                    Clear();
                    Console.WriteLine("-- ATM Transaction -- \n");
                    Console.WriteLine($"Using account: {selectedAccount.AccountNumber} ({getFullAccountType(selectedAccount.AccountType)}), ${selectedAccount.Balance}\n");

                    (bool escaped, double result) depositInput = GetCurrencyInput("Please enter how much you would like to deposit (Input nothing to return): \n", "\nPlease input a correct deposit amount\n\n",
                        input => input > 0);

                    if (depositInput.escaped)
                    {
                        return AtmTransaction(accounts); // TODO Look into a way to return to transaction selection rather then acount selection
                    }
                    else
                    {
                        return (selectedAccount, TransactionType.Deposit, depositInput.result);
                    }
                case 2:
                    Clear();
                    Console.WriteLine("-- ATM Transaction -- \n");
                    Console.WriteLine($"Using account: {selectedAccount.AccountNumber} ({getFullAccountType(selectedAccount.AccountType)}), ${selectedAccount.Balance}\n");

                    (bool escaped, double result) withdrawInput = GetCurrencyInput("Please enter how much you would like to withdraw (Input nothing to return): \n", "\nPlease input a correct withdraw amount\n\n",
                        input => input > 0 && input < selectedAccount.Balance);

                    if (withdrawInput.escaped)
                    {
                        return AtmTransaction(accounts); // TODO Look into a way to return to transaction selection rather then acount selection
                    }
                    else
                    {
                        return (selectedAccount, TransactionType.Withdraw, withdrawInput.result);
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
            if (wasSuccess)
            {
                Console.WriteLine($"{transactionType} of ${amount} was success\n");
                Console.WriteLine($"Balance of account is now ${newBalance}");
            }
            else
            {
                Console.WriteLine($"{transactionType} of ${amount} failed. Contact customer service for assistance\n");
            }

            Console.WriteLine("Press any key to return to account selection");
            Console.ReadKey();
        }

    }


}
