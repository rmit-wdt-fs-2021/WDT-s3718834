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

        /// <summary>
        /// Checks if there are any records in the Customer table
        /// </summary>
        /// <returns>The task of the database operation with the result being if there are records in the Customer table</returns>
        public async Task<bool> CustomersExist()
        {
            var dataTable = await GetDataTable("SELECT COUNT(*) FROM Customer");
            return (int) dataTable.Select()[0][0] > 0;
        }

        /// <summary>
        /// Adds a single customer to the database 
        /// </summary>
        /// <param name="customer">The customer to add to the table</param>
        /// <returns>The task of the database operation</returns>
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

        /// <summary>
        /// Adds a collection of customers to the database table
        /// </summary>
        /// <param name="customers">The collection of the customers to add to the database table</param>
        /// <returns>The task of the database operations</returns>
        public async Task AddCustomerBulk(IEnumerable<Customer> customers)
        {
            foreach (var customer in customers)
            {
                await AddCustomer(customer);
            }
        }

        /// <summary>
        /// Retrieves customer from database based on the provided customer id 
        /// </summary>
        /// <param name="customerId">The customer id the retrieved customer must have</param>
        /// <returns>The task of the database operation with the retrieved customer as the result</returns>
        /// <exception cref="RecordMissingException">Thrown when a customer with the provided customer id doesn't exist</exception>
        public async Task<Customer> GetCustomer(int customerId)
        {
            var command = CreateCommand("SELECT * FROM Customer WHERE CustomerID = @customerID");
            command.Parameters.AddWithValue("@customerID", customerId);

            var data = await GetDataTable(command);
            var dataRows = data.Select();

            if (dataRows.Length <= 0) throw new RecordMissingException("No customer with provided id exists");

            // Get the first customer. There should only ever be one customer with a specific customer id due to table constraints
            var customerDataRow = dataRows[0];

            return new Customer(
                (int) customerDataRow["CustomerID"],
                customerDataRow["Name"].ToString(),
                customerDataRow["Address"].ToString(),
                customerDataRow["City"].ToString(),
                customerDataRow["PostCode"].ToString());
        }

        /// <summary>
        /// Adds login data to the Login database table
        /// </summary>
        /// <param name="login">The login data to add to the database table</param>
        /// <returns>The task of the database operation</returns>
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

        /// <summary>
        /// Adds a collection of login data to the Login database table
        /// </summary>
        /// <param name="logins">The login data to add to the database table</param>
        /// <returns>The task of the database operation</returns>
        public async Task AddLoginBulk(IEnumerable<Login> logins)
        {
            foreach (var login in logins)
            {
                await AddLogin(login);
            }
        }

        /// <summary>
        /// Gets the password hash of login represented by provided login id and the customer id of the customer linked to the login
        /// </summary>
        /// <param name="loginId">The login id that the retrieved data must be based on</param>
        /// <returns>The task of the database operation with the result being the customer id of the linked customer and the password hash for the login</returns>
        /// <exception cref="RecordMissingException">Thrown when the login id provided doesnt have an associated record</exception>
        public async Task<(int customerId, string passwordHash)> GetPasswordHashAndCustomerId(int loginId)
        {
            var command =
                CreateCommand(
                    "SELECT C.CustomerID, PasswordHash FROM Login JOIN Customer C on C.CustomerID = Login.CustomerID WHERE Login.LoginID = @loginID");
            command.Parameters.AddWithValue("@loginID", loginId);

            var data = await GetDataTable(command);

            var dataRows = data.Select();
            if (dataRows.Length <= 0) throw new RecordMissingException("No login with provided login id exists");

            var dataRow =
                dataRows[0]; // Gets the first record because there should only be result due to database constraints
            return ((int) dataRow["CustomerId"], dataRow["PasswordHash"].ToString());
        }

        /// <summary>
        /// Adds the provided account to the Account database table
        /// </summary>
        /// <param name="account">The account to add to the database table</param>
        /// <returns>The task of the database operation</returns>
        public async Task AddAccount(Account account)
        {
            var command =
                CreateCommand(
                    "INSERT INTO Account (AccountNumber, AccountType, CustomerID, Balance) VALUES (@accountNumber, @accountType, @customerId, @balance)");
            command.Parameters.AddWithValue("@accountNumber", account.AccountNumber);
            command.Parameters.AddWithValue("@accountType", account.AccountType);
            command.Parameters.AddWithValue("@customerId", account.CustomerId);
            command.Parameters.AddWithValue("@balance", account.Balance);

            await command.ExecuteNonQueryAsync();
        }


        /// <summary>
        /// Adds a collection of accounts to the Account database table
        /// </summary>
        /// <param name="accounts">The accounts to save to the database table</param>
        /// <returns>The task of the database operations</returns>
        public async Task AddAccountBulk(IEnumerable<Account> accounts)
        {
            foreach (var account in accounts)
            {
                await AddAccount(account);
            }
        }

        /// <summary>
        /// Gets the accounts linked to the customer with the provided customer id
        /// </summary>
        /// <param name="customerId">The customer id of the customer who owns the accounts to retrieve</param>
        /// <returns>The task of the database operation with the result being the accounts retrieved</returns>
        public async Task<List<Account>> GetAccounts(int customerId)
        {
            var command = CreateCommand("SELECT * FROM Account WHERE CustomerID = @customerId");
            command.Parameters.AddWithValue("@customerId", customerId);

            var data = await GetDataTable(command);

            return data.Select().Select(dataRow =>
                new Account(
                    (int) dataRow["AccountNumber"],
                    dataRow["AccountType"].ToString().ToCharArray()[0], // Convert the database value to a char
                    (int) dataRow["CustomerID"],
                    (decimal) dataRow["Balance"])).ToList();
        }

        /// <summary>
        /// Updates the balance of the account with the provided account number with the balance provided
        /// </summary>
        /// <param name="newBalance">The value to update the account's balance to</param>
        /// <param name="accountNumber">The account number of the account to update</param>
        /// <returns>The task of the database operation</returns>
        public async Task UpdateAccountBalance(decimal newBalance, int accountNumber)
        {
            var command = CreateCommand("UPDATE Account SET Balance = @balance WHERE AccountNumber = @accountNumber");
            command.Parameters.AddWithValue("@balance", newBalance);
            command.Parameters.AddWithValue("@accountNumber", accountNumber);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Gets the account with the provided account number
        /// </summary>
        /// <param name="accountNumber">The account number of the account to retrieve</param>
        /// <returns>The task of the database operation with the result being account retrieved</returns>
        /// <exception cref="RecordMissingException">Thrown when no account exists with the provided account number</exception>
        public async Task<Account> GetAccount(int accountNumber)
        {
            var command = CreateCommand("SELECT * FROM Account WHERE AccountNumber = @accountNumber");
            command.Parameters.AddWithValue("@accountNumber", accountNumber);

            var data = await GetDataTable(command);

            var dataSet = data.Select();

            if (dataSet.Length == 0) throw new RecordMissingException("No account with provided account number exists");

            return dataSet.Select(dataRow =>
                new Account(
                    (int) dataRow["AccountNumber"],
                    dataRow["AccountType"].ToString().ToCharArray()[0],
                    (int) dataRow["CustomerID"],
                    (decimal) dataRow["Balance"])).First();
        }

        /// <summary>
        /// Adds the provided transaction to the Transaction database table
        /// </summary>
        /// <param name="transaction">The transaction to add to the database table</param>
        /// <returns>The task of the database operation</returns>
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


        /// <summary>
        /// Adds a collection of transactions to the Transaction database table
        /// </summary>
        /// <param name="transactions">The transaction to add to the database table</param>
        /// <returns>The task of the database operation</returns>
        public async Task AddTransactionBulk(IEnumerable<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                await AddTransaction(transaction);
            }
        }


        /// <summary>
        /// Gets all the transactions linked to account with the provided account number
        /// </summary>
        /// <param name="accountNumber">The account number of the account to get transactions for</param>
        /// <returns>The task of the database operation with result being a list of the transactions</returns>
        public async Task<List<Transaction>> GetTransactions(int accountNumber)
        {
            var command =
                CreateCommand(
                    "SELECT * FROM [Transaction] WHERE AccountNumber = @accountNumber ORDER BY TransactionTimeUtc DESC ");
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

        /// <summary>
        /// Gets the number of withdrawal and transfer transactions linked to the account the provided account number represents.
        /// </summary>
        /// <param name="accountNumber">The account number of the account the transactions are linked to</param>
        /// <returns>The task of the database operation with the result being number of withdrawal/transfer transactions</returns>
        public async Task<int> GetServiceFeeTransactionCounts(int accountNumber)
        {
            var command =
                CreateCommand(
                    "SELECT COUNT(*) FROM [Transaction] WHERE (TransactionType = 'W' OR TransactionType = 'T') AND AccountNumber = @accountNumber");
            command.Parameters.AddWithValue("accountNumber", accountNumber);

            var data = await GetDataTable(command);

            return (int) data.Select()[0][0]; // Get the count
        }

        /// <summary>
        /// Creates a SQL command from the provided string 
        /// </summary>
        /// <param name="commandString">The string to make a command out of</param>
        /// <returns>The sql command created</returns>
        private SqlCommand CreateCommand(string commandString)
        {
            var command = _connection.CreateCommand();
            command.CommandText = commandString;

            return command;
        }

        /// <summary>
        /// Creates a command from the provided string and feels a DataTable with the command's results
        /// </summary>
        /// <param name="commandString">The string to make the command out of</param>
        /// <returns>The results of the command</returns>
        private async Task<DataTable> GetDataTable(string commandString)
        {
            return await GetDataTable(CreateCommand(commandString));
        }

        /// <summary>
        /// Fills a DataTable with the results of the provided command
        /// </summary>
        /// <param name="sqlCommand">The command to be used to fill the DataTable</param>
        /// <returns>The results of the provided command</returns>
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

    /// <summary>
    /// Thrown when the database proxy didn't find a record
    /// </summary>
    public class RecordMissingException : Exception
    {
        public RecordMissingException() : base("Failed to retrieve any records")
        {
        }

        public RecordMissingException(string message) : base(message)
        {
        }
    }
}