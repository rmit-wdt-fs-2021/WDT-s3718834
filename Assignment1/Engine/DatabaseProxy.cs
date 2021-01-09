using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Assignment1
{
    class DatabaseProxy
    {

        private SqlConnection connection;
        public DatabaseProxy()
        {
            connection = new SqlConnection(ConfigurationProvider.GetDatabaseConnectionString());
            connection.Open();
        }

        public bool CustomersExist()
        {
            return GetDataTable("SELECT * FROM Customer").Rows.Count > 0;
        }

        public void AddCustomer(Customer customer) 
        {
            SqlCommand command = CreateCommand("INSERT INTO Customer (CustomerID, Name, Address, City, PostCode) VALUES (@customerID, @name, @address, @city, @postcode)");
            command.Parameters.AddWithValue("@customerID", customer.CustomerID);
            command.Parameters.AddWithValue("@name", customer.Name);
            command.Parameters.AddWithValue("@address", customer.Address);
            command.Parameters.AddWithValue("@city", customer.City);
            command.Parameters.AddWithValue("@postcode", customer.Postcode);

            command.ExecuteNonQuery();
        }

        // TODO Replace with more efficent bulk insert
        public void AddCustomerBulk(List<Customer> customers) 
        {
            foreach(Customer customer in customers)
            {
                AddCustomer(customer);
            }
        }

        public Customer GetCustomer(string customerID)
        {
            SqlCommand command = CreateCommand("SELECT * FROM Customer WHERE CustomerID = @customerID");
            command.Parameters.AddWithValue("@customerID", customerID);

            DataTable data = GetDataTable(command);

            DataRow[] dataRows = data.Select();
            if(dataRows.Length > 0)
            {
                DataRow customerDataRow = dataRows[0];

                return new Customer(
                        (int)customerDataRow["CustomerID"],
                        customerDataRow["Name"].ToString(),
                        customerDataRow["Address"].ToString(),
                        customerDataRow["City"].ToString(),
                        customerDataRow["PostCode"].ToString());
            } else
            {
                throw new RecordMissingException("No customer with provided id exists");
            }
        }

        public void AddLogin(Login login)
        {
            SqlCommand command = CreateCommand("INSERT INTO Login (LoginID, CustomerID, PasswordHash) VALUES (@loginID, @customerID, @passwordHash)");
            command.Parameters.AddWithValue("@loginID", login.LoginID);
            command.Parameters.AddWithValue("@customerID", login.CustomerID);
            command.Parameters.AddWithValue("@passwordHash", login.PasswordHash);

            command.ExecuteNonQuery();
        }

        // TODO Replace with more efficent bulk insert
        public void AddLoginBulk(List<Login> logins)
        {
            foreach (Login login in logins)
            {
                AddLogin(login);
            }
        }

        public string GetPasswordHash(string loginID)
        {
            SqlCommand command = CreateCommand("SELECT * FROM Login WHERE LoginID = @loginID");
            command.Parameters.AddWithValue("@loginID", loginID);

            DataTable data = GetDataTable(command);

            DataRow[] dataRows = data.Select();
            if (dataRows.Length > 0)
            {
                return dataRows[0]["LoginID"].ToString();
            }
            else
            {
                throw new RecordMissingException("No login with provided id exists");
            }
        }


        private SqlCommand CreateCommand(string commandString)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = commandString;

            return command;
        }

        private DataTable GetDataTable(string commandString)
        {
            return GetDataTable(CreateCommand(commandString));
        }

        private DataTable GetDataTable(SqlCommand command)
        {
            DataTable table = new DataTable();

            new SqlDataAdapter(command).Fill(table);

            return table;
        }
    }

    
}
