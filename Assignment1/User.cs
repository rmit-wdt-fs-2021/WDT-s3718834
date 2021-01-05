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
    }
}
