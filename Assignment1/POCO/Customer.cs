using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public String Postcode { get; set; }

        public Customer(int customerID, string name, string address, string city, string postcode)
        {
            CustomerID = customerID;
            Name = name;
            Address = address;
            this.City = city;
            this.Postcode = postcode;
        }
    }
}
