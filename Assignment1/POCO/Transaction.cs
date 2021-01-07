using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public char TransactionType { get; set; }
        public int SourceAccount { get; set; }
        public int DestinationAccountNumber { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }
        public DateTime TransactionTimeUtc { get; set; }

        public Transaction(int transactionID, char transactionType, int sourceAccount, int destinationAccountNumber, double amount, string comment, DateTime transactionTimeUtc)
        {
            TransactionID = transactionID;
            TransactionType = transactionType;
            SourceAccount = sourceAccount;
            DestinationAccountNumber = destinationAccountNumber;
            Amount = amount;
            Comment = comment;
            TransactionTimeUtc = transactionTimeUtc;
        }
    }
}
