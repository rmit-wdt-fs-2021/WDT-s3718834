using System;

namespace Assignment1.POCO
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }

        public Customer(int customerId, string name, string address, string city, string postcode)
        {
            CustomerId = customerId;
            Name = name;
            Address = address;
            this.City = city;
            this.Postcode = postcode;
        }
    }
}
