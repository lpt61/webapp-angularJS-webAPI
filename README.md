
About the project:
- This project simulates an email client application with basic email operations: create, send, update (draft mails), delete.
- This project uses AngularJS at client side, web API controllers and HMailServer API at server side.

Setting up hMailServer:
1. Restore the MS SQL database in the DB folder.
2. Install hMailServer.
3. Setup hMailServer administrator and remember the password, you will need it later. The 
4. Setup hMailServer database:
   - At step 2 of 7: choose Select a new default database
   - At step 3: choose Microsoft SQL server
   - At step 4:  Enter database name : GreenMailDB, fill up the rest with your SQL server's information. 
   - At step 5: choose Service: SQL Server
   - Then click next until complete.
   
Running the project:
1. Run hMailServer Administrator and login with the password you register before
2. Modify HMClient.Data\Concrete\HMS APIs\HMSApi.cs:
   - Replace the value of adminPassword in the parameterless HMSApi constructor with your hMailServer Admin's password
   
All done.
