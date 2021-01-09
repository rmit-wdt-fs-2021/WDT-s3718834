namespace Assignment1.POCO
{
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
    }
}
