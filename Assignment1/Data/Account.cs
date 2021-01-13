namespace Assignment1.Data
{
    /// <summary>
    /// Basic DTO for account data
    /// </summary>
    public class Account
    {
        public int AccountNumber { get; set; }
        public char AccountType { get; set; }
        public int CustomerId { get; set; }
        public decimal Balance { get; set; }

        public Account(int accountNumber, char accountType, int customerId, decimal balance)
        {
            AccountNumber = accountNumber;
            AccountType = accountType;
            CustomerId = customerId;
            Balance = balance;
        }

        public override string ToString()
        {
            return $"{AccountNumber} ({GetFullAccountType()}), ${Balance}";
        }
        
        /// <summary>
        /// Formats the account's account type as a full string
        /// </summary>
        /// <returns>The account's account type in a full string</returns>
        public string GetFullAccountType()
        {
            return AccountType == 'S' ? "Savings" : "Checking";
        }
    }
}
