
<h1> Web Development Technologies Assignment 1</h1>
<h2>Written by Brodey Yendall (s3718834)</h2>
This repository contains all code for my assignment 1 submission along with this readme that
will go through some of my design decisions. 
<br>
To run my code simply compile all the classes and run the Main() method in the Program class.

<h2> Design patterns in my code</h2>
Three of the design patterns used in my code are listed below:
<h3>MVC</h3>
MVC dictates the overall structure of my code, it is why my view, controller and model are seperated.
Note that in my code I have called the model an engine. I used this pattern for a variety of reasons
first being the improved encapsulation it provides, code that manages user input is far seperated 
from the backend code that connects to the database, etc. <br>The second and bigger advantage that
MVC provides is how easy it makes it to change implementations of not only the user interface but 
also the backend data management and flow controlling. Due to assignment 2 being based on assignment 1
code I immediately saw how valuable MVC would be, it allows me to replace the terminal interface with 
a web interface and lets me to replace the SQL engine with a entity frame work engine. Both of these 
tasks can be done without changing another MVC level, allowing me to make changes to one knowing another
works. 
<h3>Proxy</h3>
To prevent the engine class from getting too big I use the proxy pattern. As the names suggests,
DatabaseProxy and DataSeedApiProxy are the proxy classes. 
<br>The DatabaseProxy handles all the databse 
operations and gives a simple interface for the engine to access the database. This allows me to easily
change SQL or database interaction libraries without making a change to the database engine class
while reducing the complexity of the engine class.
<br> The DataSeedApiProxy is similar where it provides 2 simple methods for retrieving data from the API.
This means the engine doesn't have to worry about datatype conversions if the API changes and doesn't have
to deal with the various API libraries if they were change. In the case that the API changes in any way
I can change my code that interacts with it without editing the engine directly. Once again this 
reduces the complexity of the engine class. 
<h3>DTO</h3>
DTO simply allows me to move data easily through the various MVC levels. With DTO all the levels can use 
the same data type which makes passing data significantly easier and performant. It also allows me to
quickly make changes if the data structure were to change.

All DTO objects can be found in the data directory of the Assignment 1 application. They are called Account, Customer, Login and Transaction

<h2> My use of class library </h2>
The class library I use in the my project is called ConfigurationProvider. This class, as the name suggests,
creates a simple interface for access configuration from file. An example of this configuration is a 
database connection string. This is mostly used in the engine currently however this could be used in
any level of the MVC model. The class is static so that the provider doesn't need to be passed around.
<br> The ConfigurationProvider code could be used not only for this project but many others and because of 
this I made it a class library; It can easily be extracted and used else where. Its possible usage throughout
the entire project further endorses this choice.

<h2> Async Use </h2>
My use of asynchronous functionality is centered around the engine where the most amount of computation
and IO occurs. In my project it is mostly IO that is used asynchronously, this is because they can 
often take significant time to occur. From API to database requests, my asynchronous functionality allows the user
to see a loading screen when requests are processing and continue to interact with the terminal. 
<br> This functionality isn't too important when using a terminal application but when you move to a 
graphic interface (such as a web) one this is very important, without async the interface will completely 
lock up. This async also allows the engine to perform an API request and database request at the same time,
reducing data seeding time. The async could also allow the application to send multiple database requests 
at a time which could be increase the speed of transfers and transactions however the database we use 
for this assignment doesn't support this. I still implemented it incase we 
need to do it in the future.


<br> You can find async in all engine classes and it ends in the controller classes.

<h2> Customer Exceptions </h2>
I have three customer Exceptions:
<h3> LoginFailedException </h3> 
Located in the IBankingEngine class, this exception is throw in engine classes when login data (login id and password) do not match or the login doesn't exist at all. 
<h3> RecordMissingException </h3>
Located in the DatabaseProxy class, this exception is thrown and dealt with in Engine classes for when a database record cannot be found. 
<h3> InputCancelException </h3>
Located in the TerminalBankingView class, this exception is thrown and dealt with in View classes. It occurs when the user chooses a cancel option. 
