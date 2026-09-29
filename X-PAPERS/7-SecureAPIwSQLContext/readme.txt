We need to discuss SQL permissions like this... built sample table HR.....


Thank you for your help me my audit tool. I need to two things today. APIs make calls to databases, and the record access may be restricted based on the user login. For SQL Azure tables can be restricted to users. If so we may need to have a dynamic connection string based on the user hitting an api
Copilot said:
Yes, but I would strongly recommend not solving this with dynamic connection strings unless you have a specific multi-database requirement.

Option 1 (Recommended): Single Connection String + Row Level Security
supports Row-Level Security (RLS), which allows the database engine itself to automatically filter records based on the user identity or application context.
microsoft
+1

Example:

User A calls /api/customers
API connects with its normal service account
API tells SQL who the authenticated user is
SQL automatically returns only User A's rows
User B gets different results from the exact same query
This is usually the cleanest architecture for SaaS and audit-driven systems because the security lives in the database and cannot be bypassed by a forgotten WHERE clause.
microsoft
+1

For FusionIdentity, I would likely do something like:




C#
EXEC sp_set_session_context
    @key = N'UserId',
    @value = @CurrentUserId;
Then create an RLS policy that checks:




SQL
SESSION_CONTEXT(N'UserId')
against the owning user on each row. Microsoft specifically recommends SESSION_CONTEXT for multi-tenant scenarios.
microsoft
+1

Option 2: Dynamic Connection String Per User
This is possible.

You can create individual database users and build a connection string dynamically:




C#
Server=myserver.database.windows.net;
Database=FusionIdentity;
User ID=john.smith;
Password=*****
Azure SQL supports user-level authentication and contained database users.
microsoft
+1

However, there are drawbacks:

Large user management overhead
Massive connection pool fragmentation
Password rotation complexity
Harder troubleshooting
Thousands of users can become expensive to administer
For an application like FusionIdentity, I would avoid this unless every user truly owns separate database credentials.

Option 3: Role-Based Restrictions
You already have:




Plain Text
Role   = User / SuperUser / Admin
Role2  = Auditor / Accountant
Role3  = SwaggerAdmin
I would extend that concept into the database:




SQL
Customer
---------
CustomerId
OwnerUserId
 
AuditRecord
---------
AuditId
OwnerUserId
Then:




SQL
User      -> only their records
Auditor   -> all customer records
Accountant-> accounting records
Admin     -> everything
implemented through RLS policies.
microsoft
+1

What I Would Do For FusionIdentity
My recommendation would be:

Keep one Azure SQL connection string in configuration.

Authenticate through FusionIdentity as you do today.

After authentication, place:

UserId
Role
Role2
Role3
into SQL SESSION_CONTEXT.

Add Azure SQL Row Level Security policies.

Let SQL decide which rows the caller can see.

This keeps security centralized and auditable, which fits your audit tooling extremely well.
microsoft
+1

Also, thinking ahead: if your APIs eventually authenticate with JWT tokens, the middleware can extract the FusionIdentity user and automatically set the SQL session context before every query, making the row security completely transparent to the API code.





Favicon typeFavicon type
Sources
You said:
so when I pass the user token to the api, it should be able to determine that. Ok so I need to build a test template which uses this high security mode. I would like to do a column permission scheme. So lets build a simple hr table with a salary, and stock options list, so username social security number, salary, total stock options
Copilot said:
Yes. This is actually a great security test case because it demonstrates both:

Row-level security (which employee records can you see?)
Column-level security (which fields can you see within a record?)
For an HR system, the sensitive fields are exactly what you mentioned.

Test Table



SQL
CREATE TABLE EmployeeHR
(
    EmployeeId          INT IDENTITY(1,1) PRIMARY KEY,
    Username            NVARCHAR(100) NOT NULL,
    SocialSecurityNo    NVARCHAR(11) NOT NULL,
    Salary              DECIMAL(18,2) NOT NULL,
    TotalStockOptions   INT NOT NULL
);
Example data:




SQL
INSERT INTO EmployeeHR
(
    Username,
    SocialSecurityNo,
    Salary,
    TotalStockOptions
)
VALUES
('john','111-11-1111',120000,5000),
('mary','222-22-2222',95000,2500),
('bill','333-33-3333',150000,10000);
FusionIdentity Access Model
We'll leverage your existing roles:




Plain Text
Role
-----
User
SuperUser
Admin
 
Role2
-----
Auditor
Accountant
 
Role3
-----
SwaggerAdmin
User
Can only see:




Plain Text
Username
Cannot see:




Plain Text
SSN
Salary
Stock Options
Accountant
Can see:




Plain Text
Username
Salary
Cannot see:




Plain Text
SSN
Stock Options
Auditor
Can see:




Plain Text
Username
Salary
Stock Options
 
Cannot see:




Plain Text
SSN
Admin
Can see:




Plain Text
Username
SSN
Salary
Stock Options
API Security Context
When the JWT arrives:




C#
var userId = claims.UserId;
var role = claims.Role;
var role2 = claims.Role2;
var role3 = claims.Role3;
Before SQL queries:




SQL
EXEC sp_set_session_context
    @key='UserId',
    @value=@UserId;
 
EXEC sp_set_session_context
    @key='Role',
    @value=@Role;
 
EXEC sp_set_session_context
    @key='Role2',
    @value=@Role2;
 
EXEC sp_set_session_context
    @key='Role3',
    @value=@Role3;

Show more lines
Now SQL knows who is making the call.

Dynamic View Approach
Instead of exposing the table directly, create a secure view:




SQL
CREATE VIEW vw_EmployeeHR
AS
SELECT
    Username,
 
    CASE
        WHEN SESSION_CONTEXT(N'Role') = 'Admin'
        THEN SocialSecurityNo
        ELSE NULL
    END AS SocialSecurityNo,
 
    CASE
        WHEN SESSION_CONTEXT(N'Role2') IN ('Accountant','Auditor')
             OR SESSION_CONTEXT(N'Role') = 'Admin'
        THEN Salary
        ELSE NULL
    END AS Salary,
 
    CASE
        WHEN SESSION_CONTEXT(N'Role2') = 'Auditor'
             OR SESSION_CONTEXT(N'Role') = 'Admin'
        THEN TotalStockOptions
        ELSE NULL
    END AS TotalStockOptions
FROM EmployeeHR;

Show more lines
Example Results
Standard User



JSON
{
  "username":"john",
  "socialSecurityNo":null,
  "salary":null,
  "totalStockOptions":null
}
Accountant



JSON
{
  "username":"john",
  "socialSecurityNo":null,
  "salary":120000,
  "totalStockOptions":null
}
Auditor



JSON
{
  "username":"john",
  "socialSecurityNo":null,
  "salary":120000,
  "totalStockOptions":5000
}
Admin



JSON
{
  "username":"john",
  "socialSecurityNo":"111-11-1111",
  "salary":120000,
  "totalStockOptions":5000
}
Even Better Test
I'd actually add a fifth column:




SQL
HomeAddress NVARCHAR(250)
And classify every field:

Column	Classification
Username	Public
HomeAddress	Confidential
Salary	Financial
StockOptions	Financial
SocialSecurityNo	Restricted
Then your FusionIdentity audit engine can record:




JSON
{
  "userId": 17,
  "resource": "EmployeeHR",
  "column": "SocialSecurityNo",
  "classification": "Restricted",
  "result": "Denied"
}
That becomes a very powerful demonstration of data-governance and audit controls because you're auditing access at the individual column level, not just the API endpoint level. This is exactly the sort of "high security mode" template I'd use to validate FusionIdentity's authorization model.







Add row-level security for each user

Create the API and JWT middleware

Write security tests for every role
