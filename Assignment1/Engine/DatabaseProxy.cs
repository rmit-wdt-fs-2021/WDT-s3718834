using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        ~DatabaseProxy()
        {
            _connection.Close();
        }

        public bool CustomersExist()
        {
            return GetDataTable("SELECT * FROM Customer").Rows.Count > 0;
        }

        public void AddCustomer(Customer customer)
        {
            var command =
                CreateCommand(
                    "INSERT INTO Customer (CustomerID, Name, Address, City, PostCode) VALUES (@customerID, @name, @address, @city, @postcode)");
            command.Parameters.AddWithValue("@customerID", customer.CustomerId);
            command.Parameters.AddWithValue("@name", customer.Name);
            command.Parameters.AddWithValue("@address", customer.Address);
            command.Parameters.AddWithValue("@city", customer.City);
            command.Parameters.AddWithValue("@postcode", customer.Postcode);

            command.ExecuteNonQuery();
        }

        
        public void AddCustomerBulk(IEnumerable<Customer> customers)
        {
            foreach (var customer in customers)
            {
                AddCustomer(customer);
            }
        }

        public Customer GetCustomer(int customerId)
        {
            var command = CreateCommand("SELECT * FROM Customer WHERE CustomerID = @customerID");
            command.Parameters.AddWithValue("@customerID", customerId);

            var data = GetDataTable(command);

            DataRow[] dataRows = data.Select();
            if (dataRows.Length > 0)
            {
                DataRow customerDataRow = dataRows[0];

                return new Customer(
                    (int) customerDataRow["CustomerID"],
                    customerDataRow["Name"].ToString(),
                    customerDataRow["Address"].ToString(),
                    customerDataRow["City"].ToString(),
                    customerDataRow["PostCode"].ToString());
            }
            else
            {
                throw new RecordMissingException("No customer with provided id exists");
            }
        }

        public void AddLogin(Login login) 
        {
            var command =
                CreateCommand(
                    "INSERT INTO Login (LoginID, CustomerID, PasswordHash) VALUES (@loginID, @customerID, @passwordHash)");
            command.Parameters.AddWithValue("@loginID", login.LoginId);
            command.Parameters.AddWithValue("@customerID", login.CustomerId);
            command.Parameters.AddWithValue("@passwordHash", login.PasswordHash);

            command.ExecuteNonQuery();
        }

        
        public void AddLoginBulk(IEnumerable<Login> logins)
        {
            foreach (var login in logins)
            {
                AddLogin(login);
            }
        }

        public string GetPasswordHash(string loginId)
        {
            var command = CreateCommand("SELECT PasswordHash FROM Login WHERE LoginID = @loginID");
            command.Parameters.AddWithValue("@loginID", loginId);

            var data = GetDataTable(command);

            var dataRows = data.Select();
            if (dataRows.Length > 0)
            {
                return dataRows[0]["PasswordHash"].ToString();
            }

            throw new RecordMissingException("No login with provided id exists");
        }

        public void AddAccount(Account account) 
        {
            var command =
                CreateCommand(
                    "INSERT INTO Account (AccountNumber, AccountType, CustomerID, Balance) VALUES (@accountNumber, @accountType, @customerId, @balance)");
            command.Parameters.AddWithValue("@accountNumber", account.AccountNumber);
            command.Parameters.AddWithValue("@accountType", account.AccountType);
            command.Parameters.AddWithValue("@customerId",account.CustomerId);
            command.Parameters.AddWithValue("@balance",account.Balance);

            command.ExecuteNonQuery();
        }

        
        public void AddAccountBulk(IEnumerable<Account> accounts)
        {
            foreach (var account in accounts)
            {
                AddAccount(account);
            }
        }
        
        public List<Account> GetAccounts(int customerId)
        {
            var command = CreateCommand("SELECT * FROM Account WHERE CustomerID = @customerId");
            command.Parameters.AddWithValue("@customerId", customerId);

            var data = GetDataTable(command);

            return data.Select().Select(dataRow =>
                new Account(
                    (int) dataRow["AccountNumber"],
                    dataRow["AccountType"].ToString().ToCharArray()[0],
                    (int) dataRow["CustomerID"],
                    (decimal) dataRow["Balance"])).ToList();
        }

        public void AddTransaction(Transaction transaction) 
        {
            var command =
                CreateCommand(
                    "INSERT INTO [Transaction] (TransactionType, AccountNumber, DestinationAccountNumber, Amount, Comment, TransactionTimeUtc) VALUES (@transactionType, @accountNumber, @destinationAccountNumber, @amount, @comment, @transactionTimeUtc)");
            command.Parameters.AddWithValue("@transactionType", transaction.TransactionType);
            command.Parameters.AddWithValue("@accountNumber", transaction.SourceAccount);
            command.Parameters.AddWithValue("@destinationAccountNumber", transaction.DestinationAccountNumber);
            command.Parameters.AddWithValue("@amount", transaction.Amount);
            command.Parameters.AddWithValue("@comment", transaction.Comment);
            command.Parameters.AddWithValue("@transactionTimeUtc", transaction.TransactionTimeUtc);

            command.ExecuteNonQuery();
        }

        
        public void AddTransactionBulk(IEnumerable<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                AddTransaction(transaction);
            }
        }

        
        public List<Transaction> GetTransactions(int accountNumber)
        {
            var command = CreateCommand("SELECT * FROM [Transaction] WHERE AccountNumber = @accountNumber ORDER BY TransactionTimeUtc DESC ");
            command.Parameters.AddWithValue("@accountNumber", accountNumber);

            var data = GetDataTable(command);

            return data.Select().Select(dataRow =>
                new Transaction(
                    (int) dataRow["TransactionID"],
                    dataRow["TransactionType"].ToString().ToCharArray()[0],
                    (int) dataRow["AccountNumber"],
                    (int) dataRow["DestinationAccountNumber"],
                    (decimal) dataRow["Amount"],
                    dataRow["Comment"].ToString(),
                    (DateTime) dataRow["TransactionTimeUTC"])).ToList();
        }


        private SqlCommand CreateCommand(string commandString)
        {
            var command = _connection.CreateCommand();
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