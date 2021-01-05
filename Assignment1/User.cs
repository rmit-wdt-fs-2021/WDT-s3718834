using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public class User
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public String Address { get; set; }
        public String city { get; set; }
        public String postcode { get; set; }
        public List<Account> Accounts { get; set; }

        public User()
        {
            Accounts = new List<Account>();
            this.Accounts.Add(new Account(12346789, 'S', CustomerID, 100000.01));
            this.Accounts.Add(new Account(987654321, 'C', CustomerID, 1.43));
        }
    }
}
