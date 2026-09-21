using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace statsback798.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adminlogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Userid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acknowledged = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Techid = table.Column<int>(type: "int", nullable: true),
                    Managerescid = table.Column<int>(type: "int", nullable: true),
                    Threatlevel = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adminlogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "allstocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price_2016 = table.Column<double>(type: "float", nullable: true),
                    Price_2017 = table.Column<double>(type: "float", nullable: true),
                    Price_2018 = table.Column<double>(type: "float", nullable: true),
                    Price_2019 = table.Column<double>(type: "float", nullable: true),
                    Price_2020 = table.Column<double>(type: "float", nullable: true),
                    Price_2021 = table.Column<double>(type: "float", nullable: true),
                    Price_2022 = table.Column<double>(type: "float", nullable: true),
                    Price_2023 = table.Column<double>(type: "float", nullable: true),
                    Price_2024 = table.Column<double>(type: "float", nullable: true),
                    Price_2025 = table.Column<double>(type: "float", nullable: true),
                    Price_2026 = table.Column<double>(type: "float", nullable: true),
                    Div_2016 = table.Column<double>(type: "float", nullable: true),
                    Div_2017 = table.Column<double>(type: "float", nullable: true),
                    Div_2018 = table.Column<double>(type: "float", nullable: true),
                    Div_2019 = table.Column<double>(type: "float", nullable: true),
                    Div_2020 = table.Column<double>(type: "float", nullable: true),
                    Div_2021 = table.Column<double>(type: "float", nullable: true),
                    Div_2022 = table.Column<double>(type: "float", nullable: true),
                    Div_2023 = table.Column<double>(type: "float", nullable: true),
                    Div_2024 = table.Column<double>(type: "float", nullable: true),
                    Div_2025 = table.Column<double>(type: "float", nullable: true),
                    Div_2026 = table.Column<double>(type: "float", nullable: true),
                    sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    avgdividend = table.Column<double>(type: "float", nullable: true),
                    totaldividends = table.Column<double>(type: "float", nullable: true),
                    price_start = table.Column<double>(type: "float", nullable: true),
                    price_end = table.Column<double>(type: "float", nullable: true),
                    change = table.Column<double>(type: "float", nullable: true),
                    totalreturn = table.Column<double>(type: "float", nullable: true),
                    totalreturnover10 = table.Column<double>(type: "float", nullable: true),
                    shares500 = table.Column<double>(type: "float", nullable: true),
                    totalspend = table.Column<double>(type: "float", nullable: true),
                    fiveyearequityproj = table.Column<double>(type: "float", nullable: true),
                    fiveyeardivproj = table.Column<double>(type: "float", nullable: true),
                    totalfiveyearview = table.Column<double>(type: "float", nullable: true),
                    Selected = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allstocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Apilogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Apiname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Apinumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Eptype = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hashid = table.Column<int>(type: "int", nullable: true),
                    Parameterlist = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Apiresult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apilogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CipherSupport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CipherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CipherKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CipherSupport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CockyCipherBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<int>(type: "int", nullable: false),
                    SessionToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CockyCipherBlocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Learndetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Startdate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enddate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Certauthority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Userid = table.Column<int>(type: "int", nullable: true),
                    Employeeidasint = table.Column<int>(type: "int", nullable: true),
                    Employeeid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Employee = table.Column<int>(type: "int", nullable: true),
                    Learningmodulesid = table.Column<int>(type: "int", nullable: true),
                    Cataloguesku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learndetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Learnlogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Startdate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enddate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Certauthority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Userid = table.Column<int>(type: "int", nullable: true),
                    Employeeidasint = table.Column<int>(type: "int", nullable: true),
                    Employeeid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Employee = table.Column<int>(type: "int", nullable: true),
                    Learningmodulesid = table.Column<int>(type: "int", nullable: true),
                    Cataloguesku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Emplid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learnlogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PortfolioId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Shares = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PreviousDayPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PreviousDayDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockTargetInvestment = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioStocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sectorssummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    avgdividend = table.Column<double>(type: "float", nullable: true),
                    totaldividends = table.Column<double>(type: "float", nullable: true),
                    price_start = table.Column<double>(type: "float", nullable: true),
                    price_end = table.Column<double>(type: "float", nullable: true),
                    change = table.Column<double>(type: "float", nullable: true),
                    totalreturn = table.Column<double>(type: "float", nullable: true),
                    totalreturnover10 = table.Column<double>(type: "float", nullable: true),
                    shares500 = table.Column<double>(type: "float", nullable: true),
                    totalspend = table.Column<double>(type: "float", nullable: true),
                    fiveyearequityproj = table.Column<double>(type: "float", nullable: true),
                    fiveyeardivproj = table.Column<double>(type: "float", nullable: true),
                    totalfiveyearview = table.Column<double>(type: "float", nullable: true),
                    selected = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    useridasstring = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sectorssummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessionlogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hashid = table.Column<int>(type: "int", nullable: true),
                    Sessionstart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sessionend = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Moduleid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessionlogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Superuserlogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Userid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acknowledged = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Techid = table.Column<int>(type: "int", nullable: true),
                    Managerescid = table.Column<int>(type: "int", nullable: true),
                    Threatlevel = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Superuserlogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "US"),
                    PrimaryExchange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryExchange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price2016 = table.Column<double>(type: "float", nullable: true),
                    Price2017 = table.Column<double>(type: "float", nullable: true),
                    Price2018 = table.Column<double>(type: "float", nullable: true),
                    Price2019 = table.Column<double>(type: "float", nullable: true),
                    Price2020 = table.Column<double>(type: "float", nullable: true),
                    Price2021 = table.Column<double>(type: "float", nullable: true),
                    Price2022 = table.Column<double>(type: "float", nullable: true),
                    Price2023 = table.Column<double>(type: "float", nullable: true),
                    Price2024 = table.Column<double>(type: "float", nullable: true),
                    Price2025 = table.Column<double>(type: "float", nullable: true),
                    Price2026 = table.Column<double>(type: "float", nullable: true),
                    Div2016 = table.Column<double>(type: "float", nullable: true),
                    Div2017 = table.Column<double>(type: "float", nullable: true),
                    Div2018 = table.Column<double>(type: "float", nullable: true),
                    Div2019 = table.Column<double>(type: "float", nullable: true),
                    Div2020 = table.Column<double>(type: "float", nullable: true),
                    Div2021 = table.Column<double>(type: "float", nullable: true),
                    Div2022 = table.Column<double>(type: "float", nullable: true),
                    Div2023 = table.Column<double>(type: "float", nullable: true),
                    Div2024 = table.Column<double>(type: "float", nullable: true),
                    Div2025 = table.Column<double>(type: "float", nullable: true),
                    Div2026 = table.Column<double>(type: "float", nullable: true),
                    Sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avgdividend = table.Column<double>(type: "float", nullable: true),
                    Totaldividends = table.Column<double>(type: "float", nullable: true),
                    PriceStart = table.Column<double>(type: "float", nullable: true),
                    PriceEnd = table.Column<double>(type: "float", nullable: true),
                    Change = table.Column<double>(type: "float", nullable: true),
                    Totalreturn = table.Column<double>(type: "float", nullable: true),
                    Totalreturnover10 = table.Column<double>(type: "float", nullable: true),
                    Shares500 = table.Column<double>(type: "float", nullable: true),
                    Totalspend = table.Column<double>(type: "float", nullable: true),
                    Fiveyearequityproj = table.Column<double>(type: "float", nullable: true),
                    Fiveyeardivproj = table.Column<double>(type: "float", nullable: true),
                    Totalfiveyearview = table.Column<double>(type: "float", nullable: true),
                    Selected = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemStocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Traders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Useractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Userid = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acknowledged = table.Column<int>(type: "int", nullable: true),
                    Actionpriority = table.Column<int>(type: "int", nullable: true),
                    Actiondate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Useractions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usergroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Groupid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Groupdescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Groupownerid = table.Column<int>(type: "int", nullable: true),
                    Groupcompanyid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usergroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Userhelps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticketid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Emplid = table.Column<int>(type: "int", nullable: true),
                    Descr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: true),
                    Userid = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bestcontactnumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Replied = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Repliedmanagerid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Repliedmanagerphone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Repliedmanageremail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ticketdate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Responsedate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Userhelps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Userlog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    uid = table.Column<int>(type: "int", nullable: true),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hashid = table.Column<int>(type: "int", nullable: true),
                    Hashedpassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Loginstatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uiorigin = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Userlog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usernotice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoticeDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Noticetype = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Emailgwtype = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Userid = table.Column<int>(type: "int", nullable: true),
                    Useridstring = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Emailaddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usernotice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Userprofiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stateregion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sms = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Maritalstatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    University1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    University2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Linkedinurl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Instagramurl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vimeourl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Facebookurl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Googleurl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    University = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pronoun = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activepictureurl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Userid = table.Column<int>(type: "int", nullable: true),
                    Employeeid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Postalzip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Companyid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Buid = table.Column<int>(type: "int", nullable: true),
                    Managerid = table.Column<int>(type: "int", nullable: true),
                    Regionid = table.Column<int>(type: "int", nullable: true),
                    Branchid = table.Column<int>(type: "int", nullable: true),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Defaultinstanceid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Defaultshardid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CipherSupportId = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Targetcipher = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Userprofiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Employee = table.Column<int>(type: "int", nullable: true),
                    Employeeid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Microsoftid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ncrid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Oracleid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Azureid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Plainpassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hashedpassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Passwordtype = table.Column<int>(type: "int", nullable: true),
                    Jid = table.Column<int>(type: "int", nullable: true),
                    Profileurl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Companyid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resettoken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resettokenexpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Userid = table.Column<int>(type: "int", nullable: true),
                    Btn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iscertified = table.Column<int>(type: "int", nullable: true),
                    Groupid1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Groupid2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Groupid3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Groupid4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Groupid5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accountstatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accountactiondate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accountactiondescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Displayname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Useridstring = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usersessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Userid = table.Column<int>(type: "int", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacebookToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MicrosoftToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Targetcipher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acknowledged = table.Column<int>(type: "int", nullable: true),
                    Actionpriority = table.Column<int>(type: "int", nullable: true),
                    Sessionstart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sessionend = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sessionrecorded = table.Column<int>(type: "int", nullable: true),
                    Sessionrecordurl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sessiondescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sessionusername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sessionemail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sessionfirstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sessionlastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sessionfullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sessioncomplete = table.Column<int>(type: "int", nullable: true),
                    Twofactorkey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Twofactorkeysmsdestination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Twofactorkeyemaildestination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Twofactorprovider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Twofactorprovidertoken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Twofactorproviderauthstring = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Useridasstring = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usersessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PortfolioTargetInvestment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Portfolios_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Keyassignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    KeyId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrivateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveredToUser = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keyassignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Keyassignments_Usersessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Usersessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioId = table.Column<int>(type: "int", nullable: false),
                    PortfolioStockId = table.Column<int>(type: "int", nullable: false),
                    TraderId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutionPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeOrders_PortfolioStocks_PortfolioStockId",
                        column: x => x.PortfolioStockId,
                        principalTable: "PortfolioStocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeOrders_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeOrders_Traders_TraderId",
                        column: x => x.TraderId,
                        principalTable: "Traders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Keyassignments_SessionId",
                table: "Keyassignments",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_CustomerId",
                table: "Portfolios",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeOrders_PortfolioId",
                table: "TradeOrders",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeOrders_PortfolioStockId",
                table: "TradeOrders",
                column: "PortfolioStockId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeOrders_TraderId",
                table: "TradeOrders",
                column: "TraderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adminlogs");

            migrationBuilder.DropTable(
                name: "allstocks");

            migrationBuilder.DropTable(
                name: "Apilogs");

            migrationBuilder.DropTable(
                name: "CipherSupport");

            migrationBuilder.DropTable(
                name: "CockyCipherBlocks");

            migrationBuilder.DropTable(
                name: "Keyassignments");

            migrationBuilder.DropTable(
                name: "Learndetails");

            migrationBuilder.DropTable(
                name: "Learnlogs");

            migrationBuilder.DropTable(
                name: "sectorssummaries");

            migrationBuilder.DropTable(
                name: "Sessionlogs");

            migrationBuilder.DropTable(
                name: "Superuserlogs");

            migrationBuilder.DropTable(
                name: "SystemStocks");

            migrationBuilder.DropTable(
                name: "TradeOrders");

            migrationBuilder.DropTable(
                name: "Useractions");

            migrationBuilder.DropTable(
                name: "Usergroups");

            migrationBuilder.DropTable(
                name: "Userhelps");

            migrationBuilder.DropTable(
                name: "UserLocations");

            migrationBuilder.DropTable(
                name: "Userlog");

            migrationBuilder.DropTable(
                name: "Usernotice");

            migrationBuilder.DropTable(
                name: "Userprofiles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Usersessions");

            migrationBuilder.DropTable(
                name: "PortfolioStocks");

            migrationBuilder.DropTable(
                name: "Portfolios");

            migrationBuilder.DropTable(
                name: "Traders");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
