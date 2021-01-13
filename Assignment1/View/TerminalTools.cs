using System;
using System.Collections.Generic;
using System.Text;
using Assignment1.Controller;
using Assignment1.Data;

namespace Assignment1.View
{
    /// <summary>
    /// Provides methods for the terminal view implementations to reduce repeated code and convenience.
    /// Isnt moved to a class library due to its use of the InputCancelException 
    /// </summary>
    public static class TerminalTools
    {
        /// <summary>
        ///  Gets input from the user via the terminal but obscures the input such that user cannot see it. 
        /// </summary>
        /// <param name="assistantMessage">This is the message the user will see next to where they input their text.</param>
        /// <returns>The text that the user input. Can return an empty string</returns>
        public static string GetSecureInput(string assistantMessage)
        {
            Console.Write(assistantMessage);

            var stringBuilder = new StringBuilder();
            var inputKey = Console.ReadKey(true); // Get the input key but don't show it on the console
            while (inputKey.Key != ConsoleKey.Enter)
            {
                stringBuilder.Append(inputKey.KeyChar);
                inputKey = Console.ReadKey(true);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Creates a menu of options, each linked to one of the objects in the provided list. The final option in the menu is always "cancel"
        /// </summary>
        /// <param name="objects">The objects the user will choose from. Method uses the method's toString() method to represent it's value</param>
        /// <param name="initialMessage">The message the user will see above the menu to direct them how to input</param>
        /// <typeparam name="T">The type of the objects being listed</typeparam>
        /// <returns>The user's selected object</returns>
        /// <exception cref="InputCancelException">thrown when the user selects the cancel option in the menu</exception>
        public static T ChooseFromList<T>(IReadOnlyList<T> objects, string initialMessage)
        {
            Console.WriteLine(initialMessage);

            // Printing the menu options
            for (var i = 0; i < objects.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {objects[i].ToString()}");
            }

            Console.WriteLine($"{objects.Count + 1}: Cancel");

            var accountSelectInput = GetAcceptableInput(objects.Count + 1); // Plus 1 to include the cancel option

            if (accountSelectInput == objects.Count + 1) // Cancel was selected
            {
                throw new InputCancelException();
            }

            return objects[accountSelectInput - 1]; // minus 1 because menu starts at 1 while array starts at 0
        }

        /// <summary>
        /// Gets a currency input from the user in the form a decimal. Will parse value and validate with provided function.
        /// Will attempt to get user input until either they cancel or the input is correct
        /// </summary>
        /// <param name="requestMessage">The message the user will see next to where they input their text</param>
        /// <param name="failMessage">The message the user sees then they input an incorrect (not decimal or failed validator) input</param>
        /// <param name="validator">Optional field, function that validates the value the user input was correct</param>
        /// <returns>The parsed and validated user input</returns>
        /// <exception cref="InputCancelException">The user cancelled their input</exception>
        public static decimal GetCurrencyInput(string requestMessage, string failMessage,
            Func<decimal, bool> validator = null)
        {
            while (true)
            {
                Console.Write(requestMessage);
                var input = Console.ReadLine();

                if (input == "")
                {
                    throw new InputCancelException();
                }

                // Checks if the validator is null because it is an optional field
                if (decimal.TryParse(input, out var parsedInput) && (validator == null || validator(parsedInput)))
                {
                    return parsedInput;
                }

                Console.Write(failMessage);
            }
        }

        /// <summary>
        /// Gets the users input and validates that it is within the options you suggested. Note that this doesn't create a menu
        /// it simply represents it's input.
        /// </summary>
        /// <param name="choices">The max number a choice can be (The number of the last option in the menu)</param>
        /// <returns></returns>
        public static int GetAcceptableInput(int choices)
        {
            (bool isAcceptable, int parsedValue) inputStatus;
            do
            {
                Console.Write("Your choice: ");

                var input = Console.ReadLine();

                inputStatus = IsAcceptableMenuInput(input, choices);

                if (!inputStatus.isAcceptable)
                {
                    Console.WriteLine("\nPlease provide a correct input\n");
                }
            } while (!inputStatus.isAcceptable);

            return inputStatus.parsedValue;
        }

        /// <summary>
        /// Validates the provided input is numerical, > 0 and less then the max value provided. Validating that the user's input
        /// is within range for the suggested menu.
        /// </summary>
        /// <param name="input">The input to validate</param>
        /// <param name="maxInput">The max value the input can be</param>
        /// <returns>If the validation was success and the parsed input</returns>
        private static (bool isAcceptable, int parsedValue) IsAcceptableMenuInput(string input, int maxInput)
        {
            return !int.TryParse(input, out var numericalInput)
                ? (false, 0)
                : ((numericalInput > 0 && numericalInput <= maxInput), numericalInput);
        }
        
        
        /// <returns>The account that the user selected</returns>
        /// <exception cref="InputCancelException">Thrown when the user selects the cancel input option</exception>
        
        /// <summary>
        /// Allows the user to input an account number, validating the account exists and returning it. 
        /// </summary>
        /// <param name="controller">The controller this method can use to verify the account input</param>
        /// <returns>The account that user input</returns>
        /// <exception cref="InputCancelException">Thrown when the user chooses to cancel account input</exception>
        public static Account InputAccount(BankingController controller)
        {
            while (true)
            {
                Console.Write("Account number (Enter nothing to cancel): ");
                var input = Console.ReadLine();

                if (input == "")
                {
                    throw new InputCancelException();
                }

                // Ensure that the account number is correct before spending resources accessing the database
                if (input != null && input.Length == 4 && int.TryParse(input, out var inputAccountNumber))
                {
                    var account = controller.GetAccount(inputAccountNumber); // Attempt to access an account with the input account number
                    if (account != null) // Check that account exists 
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
    }
}