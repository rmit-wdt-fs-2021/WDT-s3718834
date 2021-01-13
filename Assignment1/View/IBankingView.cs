using System.Collections.Generic;
using Assignment1.Controller;
using Assignment1.Data;

namespace Assignment1.View
{
    /// <summary>
    /// Interface for connecting the controller to the view.
    /// Allows the view implementation to change without change to the controller.
    /// </summary>
    public interface IBankingView
    {
        /// <summary>
        /// Allows the view to perform any startup processes.
        /// Must be called by the controller before other view methods otherwise unexpected behaviour may occur
        /// </summary>
        /// <param name="controller">The controller the view will contact</param>
        public void Start(BankingController controller);
        
        /// <summary>
        /// Open the interface for user login
        /// </summary>
        public void Login();

        /// <summary>
        /// Open the main menu 
        /// </summary>
        /// <param name="loggedInCustomer">customer data the view can use to display a welcome message</param>
        public void MainMenu(in Customer loggedInCustomer);

        /// <summary>
        /// Opens the interface that lists an account's transactions
        /// </summary>
        /// <param name="accounts">The accounts which can to have their transactions displayed</param>
        public void ShowTransactions(in List<Account> accounts);
        
        /// <summary>
        /// Opens the interface that allows the user to conduct an ATM transaction.
        /// </summary>
        /// <param name="accounts">The accounts the user can make an ATM transaction from</param>
        public void AtmTransaction(in List<Account> accounts);
        
        /// <summary>
        /// Opens the interface that allows the user to conduct a transfer
        /// </summary>
        /// <param name="accounts">The accounts the user can make a transfer from</param>
        public void Transfer(in List<Account> accounts);
        
        /// <summary>
        /// Displays a loading screen
        /// </summary>
        public void Loading();
    }
}