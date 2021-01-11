using System;
using System.Collections.Generic;
using Assignment1.Controller;
using Assignment1.Engine;
using Assignment1.Enum;
using Assignment1.POCO;

namespace Assignment1.View
{
    public class TerminalBankingView : IBankingView
    {
        private const int TransactionsPerPage = 4;

        private BankingController Controller { get; set; }

        public void Start(BankingController controller)
        {
            Console.WriteLine("Welcome to MCBA Banking Application");
            this.Controller = controller;
        }

        public void Login()
        {
            var warningMessage = "";
            while (true)
            {
                Clear();
                Console.WriteLine("-- Login -- \n");

                if (warningMessage != "")
                {
                    Console.WriteLine("\n" + warningMessage);
                }

                Console.WriteLine("Please provide your details below:");

                Console.Write("Login ID: ");
                var loginId = Console.ReadLine();
                if (loginId != null && loginId.Length == 8)
                {
                    var password = TerminalTools.GetSecureInput("Password: ");

                    if (password.Length == 0)
                    {
                        warningMessage = "Please input a valid password";
                        continue;
                    }

                    try
                    {
                        if (!Controller.ValidateLogin(loginId, password))
                        {
                            Login();
                        }
                        return;
                    }
                    catch (LoginFailedException)
                    {
                        warningMessage = "Provided login ID and password do not match";
                    }
                    catch (LoginAttemptsExceededException)
                    {
                        warningMessage = "Exceeded the number of login attempts. Press any key to exit";
                    }
                }
                else
                {
                    warningMessage = "Provided login ID was incorrect. A login ID must be 8 digits\n";
                }
            }
        }

        public void MainMenu(in Customer loggedInCustomer)
        {
            Clear();
            Console.WriteLine("-- Main Menu --\n");

            Console.WriteLine($"Welcome {loggedInCustomer.Name}\n");

            Console.WriteLine("Please provide input one of the options below (single character):\n" +
                              "1: ATM Transaction\n" +
                              "2: Transfer\n" +
                              "3: My Statements\n" +
                              "4: Modify profile\n" +
                              "5: Apply for loan\n" +
                              "6: Logout\n" +
                              "7: Exit\n");


            switch (TerminalTools.GetAcceptableInput(7))
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
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void ShowTransactions(in List<Account> accounts)
        {
            while (true)
            {
                Account account;
                try
                {
                    account = TerminalTools.ChooseFromList(accounts, "Please select an account:\n");
                }
                catch (InputCancelException)
                {
                    break;
                }

                var transactions = Controller.GetTransactions(account);

                if (transactions.Count <= TransactionsPerPage)
                {
                    ShowTransactionPage(transactions, 0, TransactionsPerPage - 1);
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                    continue;
                }

                var index = 0;
                while (true)
                {
                    ShowTransactionPage(transactions, index, index + (TransactionsPerPage - 1));

                    var options = new List<(string text, int indexChange)>();
                    if (index > 3) options.Add(("Previous Page", -TransactionsPerPage));
                    if (index + 3 < transactions.Count) options.Add(("Next Page", TransactionsPerPage));

                    for (var i = 0; i < options.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}: {options[i].text}");
                    }

                    Console.WriteLine($"{options.Count + 1}: Exit");
                    var input = TerminalTools.GetAcceptableInput(options.Count + 1);

                    if (input == options.Count + 1) break;

                    index += options[input - 1].indexChange;
                }
            }
        }

        private static void ShowTransactionPage(in IReadOnlyList<Transaction> transactions, int startIndex,
            int endIndex)
        {
            for (var i = startIndex; i <= endIndex && i < transactions.Count; i++)
            {
                var transaction = transactions[i];

                Console.WriteLine(
                    $"\n********************* {i + 1} of {transactions.Count} ********************************\n" +
                    $"Transaction ID:\t\t{transaction.TransactionId}\n" +
                    $"Transaction type:\t{transaction.TransactionType}\n" +
                    $"Source account #:\t{transaction.SourceAccount}\n" +
                    $"Destination account #:\t{transaction.DestinationAccountNumber}\n" +
                    $"Amount:\t\t\t{transaction.Amount}\n" +
                    $"Comment:\t\t{transaction.Comment}\n" +
                    $"Time:\t\t\t{transaction.TransactionTimeUtc}\n");
            }

            Console.WriteLine("*************************************************************");
        }

        public void Transfer(
            in List<Account> originalAccounts)
        {
            while (true)
            {
                /*
                 * We copy the list here because later we remove an item from the list which is done to prevent the user from selecting
                 * the same destination account as the source account
                 */
                var accounts = new List<Account>(originalAccounts);

                Clear();

                /*
                 * Getting the source account for the transfer
                 */
                Console.WriteLine("-- Account Transfer -- \n");
                Console.WriteLine("Please provide the account to transfer from (source)\n");

                // Will throw exception if the user cancelled their input
                var sourceAccount = TerminalTools.ChooseFromList(accounts, "Please select an account: \n");

                accounts.Remove(
                    sourceAccount); // Remove the source account so it cannot be selected as the destination account


                /*
                 * Getting the destination account for the transfer
                 */

                Clear();
                Console.WriteLine("-- Account Transfer -- \n");
                Console.WriteLine($"Sourcing transfer from the account: {sourceAccount.AccountNumber} " +
                                  $"({GetFullAccountType(sourceAccount.AccountType)}),  ${sourceAccount.Balance}\n");


                Account destinationAccount = null;
                Console.WriteLine("Please input account to transfer to");

                /*
                 * If the user has other accounts to choose then they get an option to choose them along with the raw account number input.
                 * Due to the user to this we cannot use the SelectAccount() method.
                 */
                if (accounts.Count > 0)
                {
                    while (destinationAccount == null)
                    {
                        Console.WriteLine("Please enter one of the options below or type in an account number");
                        for (var i = 0; i < accounts.Count; i++)
                        {
                            Console.WriteLine(
                                $"{i + 1}: {accounts[i].AccountNumber} ({GetFullAccountType(accounts[i].AccountType)}) ${accounts[i].Balance}");
                        }

                        Console.WriteLine($"{accounts.Count + 1}: Other Account");
                        Console.Write($"{accounts.Count + 2}: Cancel\n");

                        var accountMenuChoice = TerminalTools.GetAcceptableInput(accounts.Count + 2);
                        if (accountMenuChoice <= accounts.Count)
                        {
                            destinationAccount = accounts[accountMenuChoice - 1];
                        }
                        else if (accountMenuChoice == accounts.Count + 1)
                        {
                            try
                            {
                                destinationAccount = InputAccount();
                            }
                            catch (InputCancelException)
                            {
                                Console.WriteLine("\n");
                            }
                        }
                        else if (accountMenuChoice == accounts.Count + 2)
                        {
                            Transfer(originalAccounts);
                            return;
                        }
                    }
                }
                else
                {
                    try
                    {
                        destinationAccount = InputAccount();
                    }
                    catch (InputCancelException)
                    {
                        Transfer(accounts);
                        return;
                    }
                }

                /*
                 * Getting the amount to transfer
                 */
                try
                {
                    var currencyInput = TerminalTools.GetCurrencyInput("\nPlease input transfer amount\n",
                        "Please input a valid transfer amount\n", input => input > 0 && input <= sourceAccount.Balance);

                    var (success, updatedSourceAccount, updatedDestinationAccount) =
                        Controller.MakeTransfer(sourceAccount, destinationAccount, currencyInput);
                    TransferResponse(success, updatedSourceAccount, updatedDestinationAccount, currencyInput);
                }
                catch (InputCancelException)
                {
                    Transfer(accounts);
                    return;
                }
            }
        }

        private static void TransferResponse(bool wasSuccess, in Account sourceAccount, in Account destinationAccount,
            decimal amount)
        {
            if (wasSuccess)
            {
                Console.WriteLine(
                    $"Transfer from {sourceAccount.AccountNumber} ({GetFullAccountType(sourceAccount.AccountType)}) " +
                    $"to {destinationAccount.AccountNumber} ({GetFullAccountType(destinationAccount.AccountType)}) of {amount} was successful");
            }
            else
            {
                Console.WriteLine(
                    $"Transfer from {sourceAccount.AccountNumber} ({GetFullAccountType(sourceAccount.AccountType)}) " +
                    $"to {destinationAccount.AccountNumber} ({GetFullAccountType(destinationAccount.AccountType)}) of {amount} was unsuccessful");
                Console.WriteLine("Please contact customer service for assistance");
            }

            Console.WriteLine("\nEnding balances:");
            Console.WriteLine(
                $"{sourceAccount.AccountNumber} ({GetFullAccountType(sourceAccount.AccountType)}): ${sourceAccount.Balance}");

            // If the user of the source account isn't the user of the destination then hide the output.
            // This prevents from viewing user's account balances
            if (sourceAccount.CustomerId == destinationAccount.CustomerId)
            {
                Console.WriteLine(
                    $"{destinationAccount.AccountNumber} ({GetFullAccountType(destinationAccount.AccountType)}): ${destinationAccount.Balance}");
            }


            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private Account InputAccount()
        {
            while (true)
            {
                Console.Write("Account number (Enter nothing to cancel): ");
                var input = Console.ReadLine();

                if (input == "")
                {
                    throw new InputCancelException();
                }

                if (input != null && input.Length == 4 && int.TryParse(input, out var inputAccountNumber))
                {
                    var account = Controller.GetAccount(inputAccountNumber);
                    if (account != null)
                    {
                        return account;
                    }

                    Console.WriteLine("\nAccount provided doesn't exist\n");
                }
                else
                {
                    Console.WriteLine("\nPlease input a valid account number \n");
                }
            }
        }

        public void AtmTransaction(in List<Account> accounts)
        {
            while (true)
            {
                Clear();
                Console.WriteLine("-- ATM Transaction -- \n");
                var selectedAccount = TerminalTools.ChooseFromList(accounts, "Please select account: \n");

                if (selectedAccount == null)
                {
                    return;
                }

                Clear();
                Console.WriteLine("-- ATM Transaction -- \n");
                Console.WriteLine(
                    $"Using account: {selectedAccount.AccountNumber} ({GetFullAccountType(selectedAccount.AccountType)}), ${selectedAccount.Balance}\n");
                Console.WriteLine("Please select a transaction type\n");
                Console.WriteLine("1: Deposit\n2: Withdraw\n3: Different account\n4: Main menu");
                var transactionTypeInput = TerminalTools.GetAcceptableInput(4);

                switch (transactionTypeInput)
                {
                    case 1:
                        Clear();
                        Console.WriteLine("-- ATM Transaction -- \n");
                        Console.WriteLine(
                            $"Using account: {selectedAccount.AccountNumber} ({GetFullAccountType(selectedAccount.AccountType)}), ${selectedAccount.Balance}\n");

                        try
                        {
                            var currencyInput = TerminalTools.GetCurrencyInput(
                                "Please enter how much you would like to deposit (Input nothing to return): \n",
                                "\nPlease input a correct deposit amount\n\n", input => input > 0);
                            
                            var (wasSuccess, newBalance) = Controller.MakeAtmTransaction(selectedAccount, TransactionType.Deposit, currencyInput);
                            TransactionResponse(wasSuccess, TransactionType.Deposit, currencyInput, newBalance);
                            return;
                        }
                        catch (InputCancelException)
                        {
                            continue;
                        }

                    case 2:
                        Clear();
                        Console.WriteLine("-- ATM Transaction -- \n");
                        Console.WriteLine(
                            $"Using account: {selectedAccount.AccountNumber} ({GetFullAccountType(selectedAccount.AccountType)}), ${selectedAccount.Balance}\n");

                        try
                        {
                            var currencyInput = TerminalTools.GetCurrencyInput(
                                "Please enter how much you would like to withdraw (Input nothing to return): \n",
                                "\nPlease input a correct withdraw amount\n\n",
                                input => input > 0 && input < selectedAccount.Balance);
                            
                            var (wasSuccess, newBalance) = Controller.MakeAtmTransaction(selectedAccount, TransactionType.Withdraw, currencyInput);
                            TransactionResponse(wasSuccess, TransactionType.Withdraw, currencyInput, newBalance);
                            return;
                        }
                        catch (InputCancelException)
                        {
                            continue;
                        }
                    case 3:
                        continue;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Fatal Error: Incorrect input go through in ATM Transaction menu");
                        return;
                }
            }
        }

        private static void TransactionResponse(bool wasSuccess, TransactionType transactionType, decimal amount,
            decimal newBalance)
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

        public void Loading()
        {
            Console.WriteLine("\nLoading ... \n");
        }

        private static string GetFullAccountType(char accountType)
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
    }
}