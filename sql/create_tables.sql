-- Azure SQL security tables

IF OBJECT_ID(N'dbo.Usernotice', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usernotice (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usernotice PRIMARY KEY,
        Description NVARCHAR(MAX) NULL,
        NoticeDatetime DATETIME2 NULL,
        Noticetype NVARCHAR(255) NULL,
        Emailgwtype NVARCHAR(255) NULL,
        Userid INT NULL,
        Useridstring NVARCHAR(255) NULL,
        Emailaddress NVARCHAR(320) NULL
    );
END;

IF OBJECT_ID(N'dbo.Userlog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Userlog (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Userlog PRIMARY KEY,
        uid INT NULL,
        role NVARCHAR(255) NULL,
        Username NVARCHAR(255) NULL,
        Hashid INT NULL,
        Hashedpassword NVARCHAR(MAX) NULL,
        Loginstatus NVARCHAR(255) NULL,
        Description NVARCHAR(MAX) NULL,
        Uiorigin NVARCHAR(255) NULL
    );
END;

IF OBJECT_ID(N'dbo.Useraction', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Useraction (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Useraction PRIMARY KEY,
        Userid INT NULL,
        Description NVARCHAR(MAX) NULL,
        Acknowledged INT NULL,
        Actionpriority INT NULL,
        Actiondate DATETIME2 NULL
    );
END;

IF OBJECT_ID(N'dbo.Superuserlog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Superuserlog (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Superuserlog PRIMARY KEY,
        Userid NVARCHAR(255) NOT NULL,
        [Date] DATETIME2 NULL,
        Description NVARCHAR(MAX) NULL,
        Acknowledged NVARCHAR(255) NULL,
        Techid INT NULL,
        Managerescid INT NULL,
        Threatlevel NVARCHAR(255) NULL
    );
END;

IF OBJECT_ID(N'dbo.Sessionlog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Sessionlog (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Sessionlog PRIMARY KEY,
        Username NVARCHAR(255) NULL,
        Hashid INT NULL,
        Sessionstart DATETIME2 NULL,
        Sessionend DATETIME2 NULL,
        Moduleid NVARCHAR(255) NULL,
        Description NVARCHAR(MAX) NULL
    );
END;

IF OBJECT_ID(N'dbo.Adminlog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Adminlog (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Adminlog PRIMARY KEY,
        Userid NVARCHAR(255) NOT NULL,
        [Date] DATETIME2 NULL,
        Description NVARCHAR(MAX) NULL,
        Acknowledged NVARCHAR(255) NULL,
        Techid INT NULL,
        Managerescid INT NULL,
        Threatlevel NVARCHAR(255) NULL
    );
END;

IF OBJECT_ID(N'dbo.Apilogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Apilogs (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Apilogs PRIMARY KEY,
        Apiname NVARCHAR(255) NULL,
        Apinumber NVARCHAR(255) NULL,
        Eptype NVARCHAR(255) NULL,
        Hashid INT NULL,
        Parameterlist NVARCHAR(MAX) NULL,
        Apiresult NVARCHAR(MAX) NULL,
        Description NVARCHAR(MAX) NULL
    );
END;

IF OBJECT_ID(N'dbo.Usersessions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usersessions (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usersessions PRIMARY KEY,
        Userid INT NULL,
        Token NVARCHAR(MAX) NULL,
        GoogleToken NVARCHAR(MAX) NULL,
        FacebookToken NVARCHAR(MAX) NULL,
        MicrosoftToken NVARCHAR(MAX) NULL,
        Targetcipher NVARCHAR(MAX) NULL,
        Acknowledged INT NULL,
        Actionpriority INT NULL,
        Sessionstart DATETIME2 NULL,
        Sessionend DATETIME2 NULL,
        Sessionrecorded INT NULL,
        Sessionrecordurl NVARCHAR(MAX) NULL,
        Sessiondescription NVARCHAR(MAX) NULL,
        Sessionusername NVARCHAR(255) NULL,
        Sessionemail NVARCHAR(320) NULL,
        Sessionfirstname NVARCHAR(255) NULL,
        Sessionlastname NVARCHAR(255) NULL,
        Sessionfullname NVARCHAR(255) NULL,
        Sessioncomplete INT NULL,
        Twofactorkey NVARCHAR(MAX) NULL,
        Twofactorkeysmsdestination NVARCHAR(255) NULL,
        Twofactorkeyemaildestination NVARCHAR(320) NULL,
        Twofactorprovider NVARCHAR(255) NULL,
        Twofactorprovidertoken NVARCHAR(MAX) NULL,
        Twofactorproviderauthstring NVARCHAR(MAX) NULL,
        Useridasstring NVARCHAR(255) NULL
    );
END;

IF OBJECT_ID(N'dbo.UserProfileLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserProfileLog (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserProfileLog PRIMARY KEY,
        Description NVARCHAR(MAX) NULL,
        Uid INT NOT NULL,
        UserId NVARCHAR(255) NOT NULL,
        DateCreated DATETIME2 NOT NULL,
        NotificationType NVARCHAR(255) NULL,
        NotificationTimestamp DATETIME2 NULL,
        NotificationDestination NVARCHAR(320) NULL
    );
END;

IF OBJECT_ID(N'dbo.UserLocations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserLocations (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserLocations PRIMARY KEY,
        UserId INT NULL,
        Label NVARCHAR(255) NULL,
        AddressLine1 NVARCHAR(255) NULL,
        AddressLine2 NVARCHAR(255) NULL,
        City NVARCHAR(255) NULL,
        State NVARCHAR(255) NULL,
        PostalCode NVARCHAR(50) NULL,
        Country NVARCHAR(255) NULL,
        Latitude FLOAT NULL,
        Longitude FLOAT NULL,
        CreatedAt NVARCHAR(100) NULL,
        IsPrimary INT NULL
    );
END;

IF OBJECT_ID(N'dbo.SysLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SysLog (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SysLog PRIMARY KEY,
        LogDate DATETIME2 NOT NULL,
        LogLevel NVARCHAR(100) NULL,
        Source NVARCHAR(255) NULL,
        Message NVARCHAR(MAX) NULL,
        [Exception] NVARCHAR(MAX) NULL,
        StackTrace NVARCHAR(MAX) NULL,
        Username NVARCHAR(255) NULL,
        MachineName NVARCHAR(255) NULL,
        IpAddress NVARCHAR(100) NULL,
        AdditionalData NVARCHAR(MAX) NULL
    );
END;

IF OBJECT_ID(N'dbo.LunaLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LunaLog (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LunaLog PRIMARY KEY,
        Apiid INT NULL,
        AccessTime DATETIME2 NULL,
        QueryText NVARCHAR(MAX) NULL,
        SourceIpAddress NVARCHAR(100) NULL,
        DestinationIpAddress NVARCHAR(100) NULL,
        Uid NVARCHAR(255) NULL,
        User1 NVARCHAR(255) NULL,
        User2 NVARCHAR(255) NULL,
        User3 NVARCHAR(255) NULL,
        User4 NVARCHAR(255) NULL,
        User5 NVARCHAR(255) NULL
    );
END;

IF OBJECT_ID(N'dbo.Learnlogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Learnlogs (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Learnlogs PRIMARY KEY,
        [Date] DATETIME2 NULL,
        Description NVARCHAR(MAX) NULL,
        uid INT NULL
    );
END;