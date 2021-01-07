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

        public User(int customerID, string name, string address, string city, string postcode, List<Account> accounts)
        {
            CustomerID = customerID;
            Name = name;
            Address = address;
            this.city = city;
            this.postcode = postcode;
            Accounts = accounts;
        }
    }
}
