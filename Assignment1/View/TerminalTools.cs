using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1.View
{
    public static class TerminalTools
    {
        public static string GetSecureInput(string inputMessage)
        {
            Console.Write(inputMessage);

            var stringBuilder = new StringBuilder();
            var inputKey = Console.ReadKey(true);
            while (inputKey.Key != ConsoleKey.Enter)
            {
                stringBuilder.Append(inputKey.KeyChar);
                inputKey = Console.ReadKey();
            }

            return stringBuilder.ToString();
        }
        
        
        public static T ChooseFromList<T>(IReadOnlyList<T> objects, string initialMessage)
        {

            Console.WriteLine(initialMessage);

            for (var i = 0; i < objects.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {objects[i].ToString()}");
            }
            Console.WriteLine($"{objects.Count + 1}: Cancel");

            var accountSelectInput = GetAcceptableInput(objects.Count + 1);

            if (accountSelectInput == objects.Count + 1)
            {
                throw new InputCancelException();
            }

            return objects[accountSelectInput - 1];
        }

        public static (bool escaped, decimal result) GetCurrencyInput(string requestMessage, string failMessage, Func<decimal, bool> validator)
        {
            while (true)
            {
                Console.Write(requestMessage);
                var input = Console.ReadLine();

                if (input == "")
                {
                    return (true, 0);
                }

                if (decimal.TryParse(input, out var parsedInput) && validator(parsedInput))
                {
                    return (false, parsedInput);
                }
                else
                {
                    Console.Write(failMessage);
                }
            }
        }
        
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
        
        private static (bool isAcceptable, int parsedValue) IsAcceptableMenuInput(string input, int maxInput)
        {
            if (input.Length != 1 || !int.TryParse(input, out var numericalInput)) return (false, 0);
            for (var i = 1; i <= maxInput; i++)
            {
                if (i == numericalInput)
                {
                    return (true, numericalInput);
                }
            }

            return (false, 0);

        }
    }
    
    
}