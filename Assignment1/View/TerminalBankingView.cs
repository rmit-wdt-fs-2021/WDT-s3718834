﻿using System;
using System.Collections.Generic;
using Assignment1.Controller;
using Assignment1.Engine;
using Assignment1.Enum;
using Assignment1.POCO;

namespace Assignment1.View
{
    public class TerminalBankingView : IBankingView
    {
        private BankingController Controller { get; set; }

        public void Start(BankingController controller)
        {
            Console.WriteLine("Welcome to MCBA Banking Application");
            this.Controller = controller;
        }

        public int Login()
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
                        return Controller.ValidateLogin(loginIdNumerical, password);
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

        public void MainMenu(Customer loggedInCustomer)
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


        public void ShowTransactions(List<Account> accounts)
        {
            var account = TerminalTools.ChooseFromList(accounts, "Please select an account:\n");
            var transactions = Controller.GetTransactions(account);


            var escaped = false;
            var index = 1;
            while (!escaped)
            {
                ShowTransactionPage(transactions, index - 1, index + 2);

                var backPage = (index > 3);
                var nextPage = (index + 3 < transactions.Count);

                switch (backPage)
                {
                    case false when !nextPage:
                        escaped = true;
                        break;
                    case true when nextPage:
                        Console.WriteLine("\nPlease provide your input\n" +
                                          "1: Next Page\n" +
                                          "2: Previous Page\n" +
                                          "3: Cancel");
                        switch (TerminalTools.GetAcceptableInput(3))
                        {
                            case 1:
                                index += 4;
                                break;
                            case 2:
                                index -= 4;
                                break;
                            case 3:
                                escaped = true;
                                break;
                            default:
                                Console.WriteLine(
                                    "Fatal Error: Invalid input got through in transaction history options");
                                escaped = true;
                                break;
                        }

                        break;
                    case true:
                        Console.WriteLine("\nPlease provide your input\n" +
                                          "1: Previous Page\n" +
                                          "2: Cancel");
                        switch (TerminalTools.GetAcceptableInput(3))
                        {
                            case 1:
                                index -= 4;
                                break;
                            case 2:
                                escaped = true;
                                break;
                            default:
                                Console.WriteLine(
                                    "Fatal Error: Invalid input got through in transaction history options");
                                escaped = true;
                                break;
                        }

                        break;
                    default:
                    {
                        Console.WriteLine("\nPlease provide your input\n" +
                                          "1: Next Page\n" +
                                          "2: Cancel");
                        switch (TerminalTools.GetAcceptableInput(3))
                        {
                            case 1:
                                index += 4;
                                break;
                            case 2:
                                escaped = true;
                                break;
                            default:
                                Console.WriteLine(
                                    "Fatal Error: Invalid input got through in transaction history options");
                                escaped = true;
                                break;
                        }

                        break;
                    }
                }
            }

            if (transactions.Count > 4) return;
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        private static void ShowTransactionPage(IReadOnlyList<Transaction> transactions, int startIndex, int endIndex)
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

        public (Account sourceAccount, Account destinationAccount, decimal amount) Transfer(
            List<Account> originalAccounts)
        {
            while (true)
            {
                var accounts = new List<Account>(originalAccounts);

                Clear();
                Console.WriteLine("-- Account Transfer -- \n");
                Console.WriteLine("Please provide the account to transfer from (source)\n");
                var sourceAccount = TerminalTools.ChooseFromList(accounts, "Please select an account: \n");

                if (sourceAccount == null)
                {
                    return (null, null, 0);
                }

                accounts.Remove(sourceAccount);

                Clear();
                Console.WriteLine("-- Account Transfer -- \n");
                Console.WriteLine(
                    $"Sourcing transfer from the account: {sourceAccount.AccountNumber} ({GetFullAccountType(sourceAccount.AccountType)}),  ${sourceAccount.Balance}\n");


                Account destinationAccount;
                Console.WriteLine("Please input account to transfer to");
                if (accounts.Count > 0)
                {
                    Console.WriteLine("Please enter one of the options below or type in an account number");
                    for (var i = 0; i < accounts.Count; i++)
                    {
                        Console.WriteLine(
                            $"{i + 1}: {accounts[i].AccountNumber} ({GetFullAccountType(accounts[i].AccountType)}) ${accounts[i].Balance}");
                    }

                    Console.Write($"{accounts.Count + 1}: Cancel\n");

                    while (true)
                    {
                        Console.Write("Your input: ");

                        var input = Console.ReadLine();

                        if (!int.TryParse(input, out var numericalInput)) continue;

                        if (numericalInput > 0 && numericalInput <= accounts.Count)
                        {
                            destinationAccount = accounts[numericalInput - 1];
                            break;
                        }

                        if (numericalInput == accounts.Count + 1)
                        {
                            return (null, null, 0);
                        }

                        if (numericalInput >= 1000 && numericalInput < 10000)
                        {
                            destinationAccount = Controller.GetAccount(numericalInput);
                            if (destinationAccount != null) break;
                        }

                        Console.WriteLine("\nPlease provide a correct input");
                    }
                }
                else
                {
                    while (true)
                    {
                        Console.Write("Account number: ");
                        var input = Console.ReadLine();

                        if (input != null && input.Length == 4 && int.TryParse(input, out var inputAccountNumber))
                        {
                            destinationAccount = Controller.GetAccount(inputAccountNumber);
                            if (destinationAccount != null) break;
                        }

                        Console.WriteLine("\nPlease input a valid account number");
                    }
                }

                var (escaped, result) = TerminalTools.GetCurrencyInput("\nPlease input transfer amount\n",
                    "Please input a valid transfer amount\n", input => input > 0 && input <= sourceAccount.Balance);
                if (escaped) continue;
                return (sourceAccount, destinationAccount, result);
            }
        }

        public void TransferResponse(bool wasSuccess, Account sourceAccount, Account destinationAccount, decimal amount)
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


        public (Account account, TransactionType transactionType, decimal amount) AtmTransaction(List<Account> accounts)
        {
            while (true)
            {
                Clear();
                Console.WriteLine("-- ATM Transaction -- \n");
                var selectedAccount = TerminalTools.ChooseFromList(accounts, "Please select account: \n");

                if (selectedAccount == null)
                {
                    return (null, TransactionType.Deposit, 0);
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

                        var (escaped, result) = TerminalTools.GetCurrencyInput(
                            "Please enter how much you would like to deposit (Input nothing to return): \n",
                            "\nPlease input a correct deposit amount\n\n", input => input > 0);

                        if (escaped) continue;
                        return (selectedAccount, TransactionType.Deposit, result);
                    case 2:
                        Clear();
                        Console.WriteLine("-- ATM Transaction -- \n");
                        Console.WriteLine(
                            $"Using account: {selectedAccount.AccountNumber} ({GetFullAccountType(selectedAccount.AccountType)}), ${selectedAccount.Balance}\n");

                        var (success, inputAmount) = TerminalTools.GetCurrencyInput(
                            "Please enter how much you would like to withdraw (Input nothing to return): \n",
                            "\nPlease input a correct withdraw amount\n\n",
                            input => input > 0 && input < selectedAccount.Balance);

                        if (success)
                        {
                            continue;
                        }
                        else
                        {
                            return (selectedAccount, TransactionType.Withdraw, inputAmount);
                        }
                    case 3:
                        continue;
                    case 4:
                        return (null, TransactionType.Deposit, 0);
                    default:
                        Console.WriteLine("Fatal Error: Incorrect input go through in ATM Transaction menu");
                        return (null, TransactionType.Deposit, 0);
                }
            }
        }

        public void TransactionResponse(bool wasSuccess, TransactionType transactionType, decimal amount,
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
    }
}