using System;

namespace Assignment1.POCO
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public char TransactionType { get; set; }
        public int SourceAccount { get; set; }
        public int DestinationAccountNumber { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }
        public DateTime TransactionTimeUtc { get; set; }

        public Transaction(int transactionId, char transactionType, int sourceAccount, int destinationAccountNumber, double amount, string comment, DateTime transactionTimeUtc)
        {
            TransactionId = transactionId;
            TransactionType = transactionType;
            SourceAccount = sourceAccount;
            DestinationAccountNumber = destinationAccountNumber;
            Amount = amount;
            Comment = comment;
            TransactionTimeUtc = transactionTimeUtc;
        }
    }
}
