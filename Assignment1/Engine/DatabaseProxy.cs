using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Assignment1.POCO;
using ConfigurationLibrary;
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

        public async Task<bool> CustomersExist()
        {
            var dataTable = await GetDataTable("SELECT COUNT(*) FROM Customer");
            return (int) dataTable.Select()[0][0] > 0;
        }

        public async Task AddCustomer(Customer customer)
        {
            var command =
                CreateCommand(
                    "INSERT INTO Customer (CustomerID, Name, Address, City, PostCode) VALUES (@customerID, @name, @address, @city, @postcode)");
            command.Parameters.AddWithValue("@customerID", customer.CustomerId);
            command.Parameters.AddWithValue("@name", (object) customer.Name ?? DBNull.Value);
            command.Parameters.AddWithValue("@address", (object) customer.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@city", (object) customer.City ?? DBNull.Value);
            command.Parameters.AddWithValue("@postcode", (object) customer.Postcode ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        
        public async Task AddCustomerBulk(IEnumerable<Customer> customers)
        {
            foreach (var customer in customers)
            {
                await AddCustomer(customer);
            }
        }

        public async Task<Customer> GetCustomer(int customerId)
        {
            var command = CreateCommand("SELECT * FROM Customer WHERE CustomerID = @customerID");
            command.Parameters.AddWithValue("@customerID", customerId);

            var data = await GetDataTable(command);

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

        public async Task AddLogin(Login login) 
        {
            var command =
                CreateCommand(
                    "INSERT INTO Login (LoginID, CustomerID, PasswordHash) VALUES (@loginID, @customerID, @passwordHash)");
            command.Parameters.AddWithValue("@loginID", login.LoginId);
            command.Parameters.AddWithValue("@customerID", login.CustomerId);
            command.Parameters.AddWithValue("@passwordHash", login.PasswordHash);

            await command.ExecuteNonQueryAsync();
        }

        
        public async void AddLoginBulk(IEnumerable<Login> logins)
        {
            foreach (var login in logins)
            {
                await AddLogin(login);
            }
        }

        public async Task<(int customerId, string passwordHash)> GetPasswordHashAndCustomerId(int loginId)
        {
            var command = CreateCommand("SELECT C.CustomerID, PasswordHash FROM Login JOIN Customer C on C.CustomerID = Login.CustomerID WHERE Login.LoginID = @loginID");
            command.Parameters.AddWithValue("@loginID", loginId);

            var data = await GetDataTable(command);

            var dataRows = data.Select();
            if (dataRows.Length <= 0) throw new LoginFailedException();
           
            var dataRow = dataRows[0];
            return ((int) dataRow["CustomerId"], dataRow["PasswordHash"].ToString());

        }

        public async Task AddAccount(Account account) 
        {
            var command =
                CreateCommand(
                    "INSERT INTO Account (AccountNumber, AccountType, CustomerID, Balance) VALUES (@accountNumber, @accountType, @customerId, @balance)");
            command.Parameters.AddWithValue("@accountNumber", account.AccountNumber);
            command.Parameters.AddWithValue("@accountType", account.AccountType);
            command.Parameters.AddWithValue("@customerId",account.CustomerId);
            command.Parameters.AddWithValue("@balance",account.Balance);

            await command.ExecuteNonQueryAsync();
        }

        
        public async Task AddAccountBulk(IEnumerable<Account> accounts)
        {
            foreach (var account in accounts)
            {
                await AddAccount(account);
            }
        }
        
        public async Task<List<Account>> GetAccounts(int customerId)
        {
            var command = CreateCommand("SELECT * FROM Account WHERE CustomerID = @customerId");
            command.Parameters.AddWithValue("@customerId", customerId);

            var data = await GetDataTable(command);

            return data.Select().Select(dataRow =>
                new Account(
                    (int) dataRow["AccountNumber"],
                    dataRow["AccountType"].ToString().ToCharArray()[0],
                    (int) dataRow["CustomerID"],
                    (decimal) dataRow["Balance"])).ToList();
        }

        public async Task UpdateAccountBalance(decimal newBalance, int accountNumber)
        {
            var command = CreateCommand("UPDATE Account SET Balance = @balance WHERE AccountNumber = @accountNumber");
            command.Parameters.AddWithValue("@balance", newBalance);
            command.Parameters.AddWithValue("@accountNumber", accountNumber);
            
            await command.ExecuteNonQueryAsync();
        }

        public async Task<Account> GetAccount(int accountNumber)
        {
            var command = CreateCommand("SELECT * FROM Account WHERE AccountNumber = @accountNumber");
            command.Parameters.AddWithValue("@accountNumber", accountNumber);

            var data = await GetDataTable(command);

            var dataSet = data.Select();

            if (dataSet.Length > 0)
            {
                return dataSet.Select(dataRow =>
                    new Account(
                        (int) dataRow["AccountNumber"],
                        dataRow["AccountType"].ToString().ToCharArray()[0],
                        (int) dataRow["CustomerID"],
                        (decimal) dataRow["Balance"])).First();
            }

            return null;

        }

        public async Task AddTransaction(Transaction transaction) 
        {
            var command =
                CreateCommand(
                    "INSERT INTO [Transaction] (TransactionType, AccountNumber, DestinationAccountNumber, Amount, Comment, TransactionTimeUtc) VALUES (@transactionType, @accountNumber, @destinationAccountNumber, @amount, @comment, @transactionTimeUtc)");
            command.Parameters.AddWithValue("@transactionType", transaction.TransactionType);
            command.Parameters.AddWithValue("@accountNumber", transaction.SourceAccount);
            command.Parameters.AddWithValue("@destinationAccountNumber", transaction.DestinationAccountNumber);
            command.Parameters.AddWithValue("@amount", transaction.Amount);
            command.Parameters.AddWithValue("@comment", (object) transaction.Comment ?? DBNull.Value);
            command.Parameters.AddWithValue("@transactionTimeUtc", transaction.TransactionTimeUtc);

            await command.ExecuteNonQueryAsync();
        }

        
        public async Task AddTransactionBulk(IEnumerable<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                await AddTransaction(transaction);
            }
        }

        
        public async Task<List<Transaction>> GetTransactions(int accountNumber)
        {
            var command = CreateCommand("SELECT * FROM [Transaction] WHERE AccountNumber = @accountNumber ORDER BY TransactionTimeUtc DESC ");
            command.Parameters.AddWithValue("@accountNumber", accountNumber);

            var data = await GetDataTable(command);

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

        public async Task<int> GetServiceFeeTransactionCounts(int accountNumber)
        {
            var command = CreateCommand("SELECT COUNT(*) FROM [Transaction] WHERE (TransactionType = 'W' OR TransactionType = 'T') AND AccountNumber = @accountNumber");
            command.Parameters.AddWithValue("accountNumber", accountNumber);
            
            var data = await GetDataTable(command);

            return (int) data.Select()[0][0]; // Get the count
        }


        private SqlCommand CreateCommand(string commandString)
        {
            var command = _connection.CreateCommand();
            command.CommandText = commandString;

            return command;
        }

        private async Task<DataTable> GetDataTable(string commandString)
        {
            return await GetDataTable(CreateCommand(commandString));
        }

        private static async Task<DataTable> GetDataTable(SqlCommand sqlCommand)
        {
            return await Task.Run(() =>
            {
                var table = new DataTable();


                new SqlDataAdapter(sqlCommand).Fill(table);

                return table;
            });


        }
    }
}