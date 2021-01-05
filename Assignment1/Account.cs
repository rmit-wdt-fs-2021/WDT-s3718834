using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public class Account
    {
        public int AccountNumber { get; set; }
        public char AccountType { get; set; }
        public int CustomerID { get; set; }
        public double Balance { get; set; }

        public Account(int accountNumber, char accountType, int customerID, double balance)
        {
            AccountNumber = accountNumber;
            AccountType = accountType;
            CustomerID = customerID;
            Balance = balance;
        }
    }
}
