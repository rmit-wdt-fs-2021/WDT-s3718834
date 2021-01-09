using System;

namespace Assignment1.POCO
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public char TransactionType { get; set; }
        public int SourceAccount { get; set; }
        public int DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public DateTime TransactionTimeUtc { get; set; }

        public Transaction(char transactionType, int sourceAccount, int destinationAccountNumber, decimal amount, string comment, DateTime transactionTimeUtc)
        {
            TransactionType = transactionType;
            SourceAccount = sourceAccount;
            DestinationAccountNumber = destinationAccountNumber;
            Amount = amount;
            Comment = comment;
            TransactionTimeUtc = transactionTimeUtc;
        }

        public Transaction(int transactionId, char transactionType, int sourceAccount, int destinationAccountNumber, decimal amount, string comment, DateTime transactionTimeUtc)
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
