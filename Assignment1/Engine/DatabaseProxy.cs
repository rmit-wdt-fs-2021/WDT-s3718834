using System.Collections.Generic;
using System.Data;
using Assignment1.POCO;
using Microsoft.Data.SqlClient;

namespace Assignment1.Engine
{
    public class DatabaseProxy
    {

        private readonly SqlConnection _connection;
        public DatabaseProxy()
        {
            _connection = new SqlConnection(ConfigurationProvider.GetDatabaseConnectionString());
            _connection.Open();
        }

        public bool CustomersExist()
        {
            return GetDataTable("SELECT * FROM Customer").Rows.Count > 0;
        }

        public void AddCustomer(Customer customer) 
        {
            var command = CreateCommand("INSERT INTO Customer (CustomerID, Name, Address, City, PostCode) VALUES (@customerID, @name, @address, @city, @postcode)");
            command.Parameters.AddWithValue("@customerID", customer.CustomerId);
            command.Parameters.AddWithValue("@name", customer.Name);
            command.Parameters.AddWithValue("@address", customer.Address);
            command.Parameters.AddWithValue("@city", customer.City);
            command.Parameters.AddWithValue("@postcode", customer.Postcode);

            command.ExecuteNonQuery();
        }

        // TODO Replace with more efficient bulk insert
        public void AddCustomerBulk(List<Customer> customers) 
        {
            foreach(var customer in customers)
            {
                AddCustomer(customer);
            }
        }

        public Customer GetCustomer(string customerId)
        {
            var command = CreateCommand("SELECT * FROM Customer WHERE CustomerID = @customerID");
            command.Parameters.AddWithValue("@customerID", customerId);

            var data = GetDataTable(command);

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
            var command = CreateCommand("INSERT INTO Login (LoginID, CustomerID, PasswordHash) VALUES (@loginID, @customerID, @passwordHash)");
            command.Parameters.AddWithValue("@loginID", login.LoginId);
            command.Parameters.AddWithValue("@customerID", login.CustomerId);
            command.Parameters.AddWithValue("@passwordHash", login.PasswordHash);

            command.ExecuteNonQuery();
        }

        // TODO Replace with more efficient bulk insert
        public void AddLoginBulk(List<Login> logins)
        {
            foreach (var login in logins)
            {
                AddLogin(login);
            }
        }

        public string GetPasswordHash(string loginId)
        {
            SqlCommand command = CreateCommand("SELECT * FROM Login WHERE LoginID = @loginID");
            command.Parameters.AddWithValue("@loginID", loginId);

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
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = commandString;

            return command;
        }

        private DataTable GetDataTable(string commandString)
        {
            return GetDataTable(CreateCommand(commandString));
        }

        private static DataTable GetDataTable(SqlCommand command)
        {
            var table = new DataTable();

            new SqlDataAdapter(command).Fill(table);

            return table;
        }
    }

    
}
