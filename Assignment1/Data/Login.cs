namespace Assignment1.Data
{
    /// <summary>
    /// Basic DTO for login data
    /// </summary>
    public class Login
    {
        public int LoginId { get; set; }
        public int CustomerId { get; set; }
        public string PasswordHash { get; set; }

        public Login(int loginId, int customerId, string passwordHash)
        {
            LoginId = loginId;
            CustomerId = customerId;
            PasswordHash = passwordHash;
        }
    }
}
