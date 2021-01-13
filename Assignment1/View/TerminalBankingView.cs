using System;
using System.Collections.Generic;
using System.Data;
using Assignment1.Controller;
using Assignment1.Data;
using Assignment1.Engine;
using Assignment1.Enum;

namespace Assignment1.View
{
    public class TerminalBankingView : IBankingView
    {
        // Used when showing transaction history. Is the number of transactions on each page
        private const int TransactionsPerPage = 4;

        private BankingController _controller;

        public void Start(BankingController controller)
        {
            Console.WriteLine("Welcome to MCBA Banking Application");
            this._controller = controller;
        }

        /// <summary>
        /// Opens a login screen in the terminal. Gets the login and then the password with warning messages when an input is incorrect.
        /// </summary>
        public void Login()
        {
            var warningMessage = "";
            while (true)
            {
                Console.Clear();
                Console.WriteLine("-- Login -- \n");

                if (warningMessage != "")
                {
                    Console.WriteLine(warningMessage + "\n");
                }

                Console.WriteLine("Please provide your details below:");

                Console.Write("Login ID: ");
                var loginId = Console.ReadLine();
                if (loginId != null && loginId.Length == 8 && int.TryParse(loginId, out var loginIdNumerical))
                {
                    var password = TerminalTools.GetSecureInput("Password: ");

                    if (password.Length == 0)
                    {
                        warningMessage = "Please input a valid password";
                        continue;
                    }

                    try
                    {
                        _controller.ValidateLogin(loginIdNumerical, password);
                        return;
                    }
                    catch (LoginFailedException)
                    {
                        warningMessage = "Provided login ID and password do not match";
                    }
                }
                else
                {
                    warningMessage = "Provided login ID was incorrect. A login ID must be 8 digits";
                }
            }
        }

        /// <summary>
        /// Open the main menu in the terminal so that user can access application functionality. 
        /// </summary>
        /// <param name="loggedInCustomer">The customer who is currently logged in. Used for displaying the customer's name</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if validation code unexpectedly fails</exception>
        public void MainMenu(in Customer loggedInCustomer)
        {
            Console.Clear();
            Console.WriteLine("-- Main Menu --\n");

            Console.WriteLine($"Welcome {loggedInCustomer.Name}\n");

            Console.WriteLine("Please provide input one of the options below (single character):\n" +
                              "1: ATM Transaction\n" +
                              "2: Transfer\n" +
                              "3: My Statements\n" +
                              "4: Logout\n" +
                              "5: Exit\n");


            switch (TerminalTools.GetAcceptableInput(5))
            {
                case 1:
                    _controller.AtmTransaction();
                    break;
                case 2:
                    _controller.Transfer();
                    break;
                case 3:
                    _controller.TransactionHistory();
                    break;
                case 4:
                    _controller.Logout();
                    break;
                case 5:
                    _controller.Exit();
                    break;
                default:
                    throw
                        new ArgumentOutOfRangeException(); // Will only be reached if the GetAcceptInput() fails to correctly validate
            }
        }


        /// <summary>
        /// Displays a menu on the terminal that allows the user to see the history of transactions for the accounts provided
        /// </summary>
        /// <param name="accounts">The accounts to display a history of transactions for</param>
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

                var transactions = _controller.GetTransactions(account);

                if (transactions.Count <= TransactionsPerPage) // If only one page is needed
                {
                    ShowTransactionPage(transactions, 0, TransactionsPerPage - 1);
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                    continue;
                }

                var index = 0;
                while (true)
                {
                    // List the transactions for the current page 
                    ShowTransactionPage(transactions, index, index + (TransactionsPerPage - 1));

                    // Stores the menu options. Since there are alteast 2 pages of transactions there will always be atleast one option
                    var options = new List<(string text, int indexChange)>();

                    // If there are atleast 3 transactions before the current page then the user can go back a page
                    if (index > 3) options.Add(("Previous Page", -TransactionsPerPage));

                    // If there are atleast 3 transactions after the current page then the user can go forward a page 
                    if (index + 3 < transactions.Count) options.Add(("Next Page", TransactionsPerPage));

                    for (var i = 0; i < options.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}: {options[i].text}");
                    }

                    Console.WriteLine($"{options.Count + 1}: Exit");
                    var input = TerminalTools.GetAcceptableInput(options.Count + 1);

                    if (input == options.Count + 1) break; // Use selected the exit option

                    // Change the current page index based on the option selected. (Add to the index if next page and subtract if previous page)
                    index += options[input - 1].indexChange;
                }
            }
        }

        /// <summary>
        /// Prints transactions in a readable format from the provided list between the provided indexes
        /// </summary>
        /// <param name="transactions">The list of transactions that will be printed from</param>
        /// <param name="startIndex">The start of the range to print from</param>
        /// <param name="endIndex">The end of the range to print from</param>
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
                    $"Time:\t\t\t{transaction.TransactionTimeUtc.ToLocalTime()}\n");
            }

            Console.WriteLine("*************************************************************");
        }

        /// <summary>
        /// Displays the menu for making a transfer in the terminal
        /// </summary>
        /// <param name="originalAccounts">The accounts the user can make transfers out of</param>
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

                Console.Clear();

                /*
                 * Getting the source account for the transfer
                 */
                Console.WriteLine("-- Account Transfer -- \n");
                Console.WriteLine("Please provide the account to transfer from (source)\n");

                // Will throw exception if the user cancelled their input

                Account sourceAccount;
                try
                {
                    sourceAccount = TerminalTools.ChooseFromList(accounts, "Please select an account: \n");
                }
                catch (InputCancelException)
                {
                    return;
                }

                accounts.Remove(
                    sourceAccount); // Remove the source account so it cannot be selected as the destination account


                /*
                 * Getting the destination account for the transfer
                 */

                Console.Clear();
                Console.WriteLine("-- Account Transfer -- \n");
                Console.WriteLine($"Sourcing transfer from the account: {sourceAccount.AccountNumber} " +
                                  $"({sourceAccount.GetFullAccountType()}),  ${sourceAccount.Balance}\n");


                Account destinationAccount = null;
                Console.WriteLine("Please input account to transfer to");

                /*
                 * If the user has other accounts to choose then they get an option to choose them along with the raw account number input.
                 * Due to the user being able to this we cannot use the SelectAccount() method.
                 */
                if (accounts.Count > 0)
                {
                    while (destinationAccount == null)
                    {
                        Console.WriteLine("Please enter one of the options below or type in an account number");
                        for (var i = 0; i < accounts.Count; i++)
                        {
                            Console.WriteLine(
                                $"{i + 1}: {accounts[i].AccountNumber} ({accounts[i].GetFullAccountType()}) ${accounts[i].Balance}");
                        }

                        Console.WriteLine($"{accounts.Count + 1}: Other Account");
                        Console.Write($"{accounts.Count + 2}: Cancel\n");

                        var accountMenuChoice = TerminalTools.GetAcceptableInput(accounts.Count + 2);
                        if (accountMenuChoice <= accounts.Count)
                        {
                            destinationAccount = accounts[accountMenuChoice - 1];
                        }
                        else if (accountMenuChoice == accounts.Count + 1
                        ) // User wants to input a different account number
                        {
                            try
                            {
                                destinationAccount = TerminalTools.InputAccount(_controller, sourceAccount.AccountNumber);
                            }
                            catch (InputCancelException)
                            {
                                Console.WriteLine("\n"); // Put a space before trying to get an account again
                            }
                        }
                        else if (accountMenuChoice == accounts.Count + 2)
                        {
                            Transfer(
                                originalAccounts); // Return to the start of the transfer menu. The user can return to the main menu there
                            return;
                        }
                    }
                }
                else // User has no other accounts so we just get the raw account number input
                {
                    try
                    {
                        destinationAccount = TerminalTools.InputAccount(_controller, sourceAccount.AccountNumber);
                    }
                    catch (InputCancelException)
                    {
                        Transfer(
                            originalAccounts); // Return to the start of the transfer menu. The user can return to the main menu there
                        return;
                    }
                }

                /*
                 * Getting the amount to transfer
                 */
                try
                {
                    // Gets the transfer amount
                    var currencyInput = TerminalTools.GetCurrencyInput("\nPlease input transfer amount\n",
                        "Please input a valid transfer amount\n",
                        input => input > 0);

                    var (success, updatedSourceAccount, updatedDestinationAccount) =
                        _controller.MakeTransfer(sourceAccount, destinationAccount, currencyInput);

                    // Update the user with the result of the transfer
                    TransferResponse(success, updatedSourceAccount, updatedDestinationAccount, currencyInput);
                }
                catch (InputCancelException)
                {
                    Transfer(originalAccounts); // Return to the start of the transfer menu. The user can return to the main menu there
                    return;
                }
            }
        }

        /// <summary>
        /// Provides the result of a transfer into the terminal. Done here to reduce the size of the transfer() method.
        /// </summary>
        /// <param name="wasSuccess">Whether the transfer was successful</param>
        /// <param name="sourceAccount">The account the funds came out of</param>
        /// <param name="destinationAccount">The account where the funds landed</param>
        /// <param name="amount">The amount of money that was sent between accounts</param>
        private static void TransferResponse(bool wasSuccess, in Account sourceAccount, in Account destinationAccount,
            decimal amount)
        {
            // Print where the funds came from and where they went
            Console.Write($"Transfer from {sourceAccount.AccountNumber} ({sourceAccount.GetFullAccountType()}) " +
                          $"to {destinationAccount.AccountNumber} ({destinationAccount.GetFullAccountType()})");

            if (wasSuccess)
            {
                Console.WriteLine($"of {amount} was successful");
            }
            else
            {
                Console.WriteLine($" of {amount} was unsuccessful");
                Console.WriteLine("Please contact customer service for assistance");
            }

            Console.WriteLine("\nEnding balances:");
            Console.WriteLine(
                $"{sourceAccount.AccountNumber} ({sourceAccount.GetFullAccountType()}): ${sourceAccount.Balance}");

            // If the user of the source account isn't the user of the destination then hide the output.
            // This prevents from viewing other user's account balances
            if (sourceAccount.CustomerId == destinationAccount.CustomerId)
            {
                Console.WriteLine(
                    $"{destinationAccount.AccountNumber} ({destinationAccount.GetFullAccountType()}): ${destinationAccount.Balance}");
            }


            // Lets the user see the transfer result before taking them away from the menu
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays the interface for making an atm transaction to the terminal 
        /// </summary>
        /// <param name="accounts">The accounts the user can make an atm transaction from</param>
        public void AtmTransaction(in List<Account> accounts)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("-- ATM Transaction -- \n");

                Account selectedAccount;
                try
                {
                    selectedAccount = TerminalTools.ChooseFromList(accounts, "Please select account: \n");
                }
                catch (InputCancelException)
                {
                    return;
                }


                Console.Clear();
                Console.WriteLine("-- ATM Transaction -- \n");
                Console.WriteLine(
                    $"Using account: {selectedAccount.AccountNumber} ({selectedAccount.GetFullAccountType()}), ${selectedAccount.Balance}\n");
                Console.WriteLine("Please select a transaction type\n");

                Console.WriteLine("1: Deposit\n" +
                                  "2: Withdraw\n" +
                                  "3: Different account\n" +
                                  "4: Main menu");
                var transactionTypeInput = TerminalTools.GetAcceptableInput(4);

                if (transactionTypeInput <= 2) // User input 1 or 2 (Deposit or withdraw)
                {
                    var transactionType =
                        (transactionTypeInput == 1) ? TransactionType.Deposit : TransactionType.Withdraw;

                    Console.Clear();
                    Console.WriteLine("-- ATM Transaction -- \n");
                    Console.WriteLine(
                        $"Using account: {selectedAccount.AccountNumber} ({selectedAccount.GetFullAccountType()}), ${selectedAccount.Balance}\n");

                    try
                    {
                        // Get the amount the user wished to deposit / withdraw
                        var currencyInput = TerminalTools.GetCurrencyInput(
                            $"Please enter how much you would like to {transactionType.ToString().ToLower()} (Input nothing to return): \n",
                            $"\nPlease input a correct {transactionType.ToString().ToLower()} amount\n\n",
                            input => input > 0);

                        // Attempts to perform the transaction
                        var (wasSuccess, newBalance) = _controller.MakeAtmTransaction(selectedAccount,
                            transactionType, currencyInput);

                        // Updates the user with the result of the transaction
                        TransactionResponse(wasSuccess, transactionType, currencyInput, newBalance);
                        return;
                    }
                    catch (InputCancelException)
                    {
                        continue;
                    }
                }

                switch (transactionTypeInput)
                {
                    case 3: // User selected "Different account" so we loop back to the start
                        continue;
                    case 4: // User selected "Exit" so we leave the menu
                        return;
                }
            }
        }

        /// <summary>
        /// Provides the result of a atm transaction into the terminal. Done here to reduce the size of the AtmTransaction() method.
        /// </summary>
        /// <param name="wasSuccess">Whether the transaction was successful</param>
        /// <param name="transactionType">What type of atm transaction it was</param>
        /// <param name="amount">The account of the atm transaction</param>
        /// <param name="newBalance">The balance of the account after the atm transaction</param>
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


        /// <summary>
        /// A simple loading message when the application is waiting on a backend process to finish
        /// </summary>
        public void Loading()
        {
            Console.WriteLine("\nLoading ... \n");
        }

        /// <summary>
        /// Displays a message to the user if a fatal unexpected error occurs.
        /// This will only occur if either the database or the api were unreachable
        /// </summary>
        /// <param name="cause"></param>
        public void ApplicationFailed(string cause)
        {
            Console.Clear();
            Console.WriteLine("An external service has resulting the application failing, details below:");
            Console.WriteLine(cause);
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Thrown when the user cancels their input into the terminal
    /// </summary>
    public class InputCancelException : Exception
    {
        public InputCancelException() : base("The user cancelled their input and the cancellation wasn't caught")
        {
        }
    }
}