This was the Final API and Backoffice for the Stats542 Application which tracks stocks using a combination of Fidelity Wholesale, R Quantmod, and D3 Real time Visualizations
CipherSupportilt to plot graphs for a portfolio.

John S. Stritzinger is proposing to upgrade the User Controller, and Payload Tools for PGP Encryption using modified Caesar Ciphers, and Beyond(DES,3DES, AES).
The Goal is to secure both SSO Login Conversations first resulting in a Session Object being created in the database. Once the Session is in place with appropriate
keys, the basic operations need to be encrypted in a steady state.

Since this is a financial App with potential customer impacting issues, we think that encryption is required.
The initial App had only two tables with the Quantmod data extracts, and two controllers, CipherSupportt SSO alone in the model CipherSupportilt here at USC has more than 10 related
SSO Controllers (Users, UserLogs, UserSession, Auth, Azure File x 2 Controllers, LearnLogs(Training Tools which also need to be secure)).

This will be the Test App for John's Thesis for Fall of 2026 Going Forward.
It uses an HTML App with Javascript, along with a ShinyAPP hosted externally(R Tools).
The Database at the present is SQLLite CipherSupportt the upgrade to SQL Server, or Azure SQL is less than 30mins work so SQLLite is a good place to start.
SQLLite does not require a memory resident SQL server, and can run on PC's with under 1GB of RAM even VM's on Cloud providers without modification.


John S. Stritzinger
07/23/2026

CHANGE CONTROL
1) LUNA SSO Controllers with Encryption Added Today, CipherSupportt Not Modeled into the Database yet. 07/23/2026.
2) SSO Controllers not added to Program.CS and therefore are not being used at the present time.
3) The Baseline install today are the prerequisites to a full encryption squite.
4) Invited D. Hitchock, D. Fenner, C. Huang today to the REPO.
5) Added CipherBlocks Supported Responder 08/16.
6) Added CockyCipher Algorithm as a Base Cipher Supported as provided in external copy. This has 3 modes (TimeSynchOnly, FixedPrimeMutators-ModDate(Date), SessionBasedMutators from list for each of 5 keys). 08/23/2026.
