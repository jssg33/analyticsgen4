using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Models;

public partial class EnterpriseContext : DbContext
{
    public EnterpriseContext()
    {
    }

    public EnterpriseContext(DbContextOptions<EnterpriseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adminlog> Adminlogs { get; set; }
    //API AUDIT ADDED 09/21/2026 - 4 NEW TABLES
    public virtual DbSet<Apiaudit> Apiaudits { get; set; }
    public virtual DbSet<Apihost> Apihosts { get; set; }
    public virtual DbSet<AuditResult> AuditResults { get; set; }
    public virtual DbSet<AuditException> AuditExceptions { get; set; }
    public virtual DbSet<ApiHostException> ApiHostExceptions { get; set; }
    
    public virtual DbSet<Allstock> Allstocks { get; set; }

    public virtual DbSet<Apilog> Apilogs { get; set; }

    public virtual DbSet<CipherSupport> CipherSupports { get; set; }

    public virtual DbSet<CockyCipherBlock> CockyCipherBlocks { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Keyassignment> Keyassignments { get; set; }

    public virtual DbSet<Learndetail> Learndetails { get; set; }

    public virtual DbSet<Learnlog> Learnlogs { get; set; }

    public virtual DbSet<Portfolio> Portfolios { get; set; }

    public virtual DbSet<PortfolioStock> PortfolioStocks { get; set; }

    public virtual DbSet<Sectorssummary> Sectorssummaries { get; set; }

    public virtual DbSet<Sessionlog> Sessionlogs { get; set; }

    public virtual DbSet<Superuserlog> Superuserlogs { get; set; }

    public virtual DbSet<SystemStock> SystemStocks { get; set; }

    public virtual DbSet<TradeOrder> TradeOrders { get; set; }

    public virtual DbSet<Trader> Traders { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLocation> UserLocations { get; set; }

    public virtual DbSet<Useraction> Useractions { get; set; }

    public virtual DbSet<Usergroup> Usergroups { get; set; }

    public virtual DbSet<Userhelp> Userhelps { get; set; }

    public virtual DbSet<Userlog> Userlogs { get; set; }

    public virtual DbSet<Usernotice> Usernotices { get; set; }

    public virtual DbSet<Userprofile> Userprofiles { get; set; }

    public virtual DbSet<Usersession> Usersessions { get; set; }

    public virtual DbSet<UserProfileLog> UserProfileLogs { get; set; }

    public virtual DbSet<Syslog> Syslogs { get; set; }

    public virtual DbSet<Lunalog> LunaLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:cockysql.database.windows.net,1433;Initial Catalog=CockyAnalytics;Persist Security Info=False;User ID=cockysa;Password=!test123456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apilog>().ToTable("Apilogs");
        modelBuilder.Entity<Learnlog>().ToTable("Learnlogs");
        modelBuilder.Entity<UserLocation>().ToTable("UserLocations");
        modelBuilder.Entity<Usersession>().ToTable("Usersessions");
        modelBuilder.Entity<Adminlog>().ToTable("Adminlog");
        modelBuilder.Entity<Sessionlog>().ToTable("Sessionlog");
        modelBuilder.Entity<Superuserlog>().ToTable("Superuserlog");
        modelBuilder.Entity<Useraction>().ToTable("Useraction");
        modelBuilder.Entity<Userlog>().ToTable("Userlog");
        modelBuilder.Entity<Usernotice>().ToTable("Usernotice");
        modelBuilder.Entity<UserProfileLog>().ToTable("UserProfileLog");
        modelBuilder.Entity<Syslog>().ToTable("SysLog");
        modelBuilder.Entity<Lunalog>().ToTable("LunaLog");

        modelBuilder.Entity<Allstock>(entity =>
        {
            entity.ToTable("allstocks");

            entity.Property(e => e.Avgdividend).HasColumnName("avgdividend");
            entity.Property(e => e.Change).HasColumnName("change");
            entity.Property(e => e.Div2016).HasColumnName("Div_2016");
            entity.Property(e => e.Div2017).HasColumnName("Div_2017");
            entity.Property(e => e.Div2018).HasColumnName("Div_2018");
            entity.Property(e => e.Div2019).HasColumnName("Div_2019");
            entity.Property(e => e.Div2020).HasColumnName("Div_2020");
            entity.Property(e => e.Div2021).HasColumnName("Div_2021");
            entity.Property(e => e.Div2022).HasColumnName("Div_2022");
            entity.Property(e => e.Div2023).HasColumnName("Div_2023");
            entity.Property(e => e.Div2024).HasColumnName("Div_2024");
            entity.Property(e => e.Div2025).HasColumnName("Div_2025");
            entity.Property(e => e.Div2026).HasColumnName("Div_2026");
            entity.Property(e => e.Fiveyeardivproj).HasColumnName("fiveyeardivproj");
            entity.Property(e => e.Fiveyearequityproj).HasColumnName("fiveyearequityproj");
            entity.Property(e => e.Price2016).HasColumnName("Price_2016");
            entity.Property(e => e.Price2017).HasColumnName("Price_2017");
            entity.Property(e => e.Price2018).HasColumnName("Price_2018");
            entity.Property(e => e.Price2019).HasColumnName("Price_2019");
            entity.Property(e => e.Price2020).HasColumnName("Price_2020");
            entity.Property(e => e.Price2021).HasColumnName("Price_2021");
            entity.Property(e => e.Price2022).HasColumnName("Price_2022");
            entity.Property(e => e.Price2023).HasColumnName("Price_2023");
            entity.Property(e => e.Price2024).HasColumnName("Price_2024");
            entity.Property(e => e.Price2025).HasColumnName("Price_2025");
            entity.Property(e => e.Price2026).HasColumnName("Price_2026");
            entity.Property(e => e.PriceEnd).HasColumnName("price_end");
            entity.Property(e => e.PriceStart).HasColumnName("price_start");
            entity.Property(e => e.Sector).HasColumnName("sector");
            entity.Property(e => e.Shares500).HasColumnName("shares500");
            entity.Property(e => e.Totaldividends).HasColumnName("totaldividends");
            entity.Property(e => e.Totalfiveyearview).HasColumnName("totalfiveyearview");
            entity.Property(e => e.Totalreturn).HasColumnName("totalreturn");
            entity.Property(e => e.Totalreturnover10).HasColumnName("totalreturnover10");
            entity.Property(e => e.Totalspend).HasColumnName("totalspend");
        });

        modelBuilder.Entity<CipherSupport>(entity =>
        {
            entity.ToTable("CipherSupport");
        });

        modelBuilder.Entity<CockyCipherBlock>(entity =>
        {
            entity.Property(e => e.type).HasColumnName("type");
        });

        modelBuilder.Entity<Keyassignment>(entity =>
        {
            entity.HasIndex(e => e.SessionId, "IX_Keyassignments_SessionId");

            entity.HasOne(d => d.Session).WithMany(p => p.Keyassignments).HasForeignKey(d => d.SessionId);
        });

        modelBuilder.Entity<Portfolio>(entity =>
        {
            entity.HasIndex(e => e.CustomerId, "IX_Portfolios_CustomerId");

            entity.Property(e => e.PortfolioTargetInvestment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UserId).HasMaxLength(200);

            entity.HasOne(d => d.Customer).WithMany(p => p.Portfolios).HasForeignKey(d => d.CustomerId);
        });

        modelBuilder.Entity<PortfolioStock>(entity =>
        {
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.PreviousDayPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Shares).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.StockTargetInvestment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Ticker).HasMaxLength(50);
        });

        modelBuilder.Entity<Sectorssummary>(entity =>
        {
            entity.ToTable("sectorssummaries");

            entity.Property(e => e.Avgdividend).HasColumnName("avgdividend");
            entity.Property(e => e.Change).HasColumnName("change");
            entity.Property(e => e.Fiveyeardivproj).HasColumnName("fiveyeardivproj");
            entity.Property(e => e.Fiveyearequityproj).HasColumnName("fiveyearequityproj");
            entity.Property(e => e.PriceEnd).HasColumnName("price_end");
            entity.Property(e => e.PriceStart).HasColumnName("price_start");
            entity.Property(e => e.Sector).HasColumnName("sector");
            entity.Property(e => e.Selected).HasColumnName("selected");
            entity.Property(e => e.Shares500).HasColumnName("shares500");
            entity.Property(e => e.Totaldividends).HasColumnName("totaldividends");
            entity.Property(e => e.Totalfiveyearview).HasColumnName("totalfiveyearview");
            entity.Property(e => e.Totalreturn).HasColumnName("totalreturn");
            entity.Property(e => e.Totalreturnover10).HasColumnName("totalreturnover10");
            entity.Property(e => e.Totalspend).HasColumnName("totalspend");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Useridasstring).HasColumnName("useridasstring");
        });

        modelBuilder.Entity<SystemStock>(entity =>
        {
            entity.Property(e => e.Country).HasDefaultValue("US");
        });

        modelBuilder.Entity<TradeOrder>(entity =>
        {
            entity.HasIndex(e => e.PortfolioId, "IX_TradeOrders_PortfolioId");

            entity.HasIndex(e => e.PortfolioStockId, "IX_TradeOrders_PortfolioStockId");

            entity.HasIndex(e => e.TraderId, "IX_TradeOrders_TraderId");

            entity.Property(e => e.ExecutionPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Portfolio).WithMany(p => p.TradeOrders).HasForeignKey(d => d.PortfolioId);

            entity.HasOne(d => d.PortfolioStock).WithMany(p => p.TradeOrders).HasForeignKey(d => d.PortfolioStockId);

            entity.HasOne(d => d.Trader).WithMany(p => p.TradeOrders).HasForeignKey(d => d.TraderId);
        });

    modelBuilder.Entity<User>(entity =>
    {
        entity.ToTable("User");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Firstname)
            .HasMaxLength(255);

        entity.Property(e => e.Lastname)
            .HasMaxLength(255);

        entity.Property(e => e.Username)
            .HasMaxLength(255);

        entity.Property(e => e.Email)
            .HasMaxLength(255);

        entity.Property(e => e.Employee);

        entity.Property(e => e.Employeeid)
            .HasMaxLength(255);

        entity.Property(e => e.Microsoftid)
            .HasMaxLength(255);

        entity.Property(e => e.Ncrid)
            .HasMaxLength(255);

        entity.Property(e => e.Oracleid)
            .HasMaxLength(255);

        entity.Property(e => e.Azureid)
            .HasMaxLength(255);

        entity.Property(e => e.Plainpassword)
            .HasMaxLength(500);

        entity.Property(e => e.Hashedpassword)
            .HasMaxLength(500);

        entity.Property(e => e.Passwordtype);

        entity.Property(e => e.Jid);

        entity.Property(e => e.Profileurl)
            .HasMaxLength(1000);

        entity.Property(e => e.Role)
            .HasMaxLength(100);

        entity.Property(e => e.Fullname)
            .HasMaxLength(255);

        entity.Property(e => e.Companyid)
            .HasMaxLength(100);

        entity.Property(e => e.Resettoken)
            .HasMaxLength(500);

        entity.Property(e => e.Resettokenexpiration);

        entity.Property(e => e.Userid);

        entity.Property(e => e.Btn)
            .HasMaxLength(100);

        entity.Property(e => e.Iscertified);

        entity.Property(e => e.Groupid1)
            .HasMaxLength(100);

        entity.Property(e => e.Groupid2)
            .HasMaxLength(100);

        entity.Property(e => e.Groupid3)
            .HasMaxLength(100);

        entity.Property(e => e.Groupid4)
            .HasMaxLength(100);

        entity.Property(e => e.Groupid5)
            .HasMaxLength(100);

        entity.Property(e => e.Accountstatus)
            .HasMaxLength(100);

        entity.Property(e => e.Accountactiondate)
            .HasMaxLength(100);

        entity.Property(e => e.Accountactiondescription)
            .HasMaxLength(1000);

        entity.Property(e => e.Displayname)
            .HasMaxLength(255);

        entity.Property(e => e.Useridstring)
            .HasMaxLength(100);

        entity.Property(e => e.Tenantid)
            .HasMaxLength(100);

        // Recommended indexes
        entity.HasIndex(e => e.Email);
        entity.HasIndex(e => e.Username);
        entity.HasIndex(e => e.Microsoftid);
        entity.HasIndex(e => e.Azureid);
        entity.HasIndex(e => e.Tenantid);
        entity.HasIndex(e => e.Companyid);
    });

        modelBuilder.Entity<Userlog>(entity =>
        {
            entity.ToTable("Userlog");

            entity.Property(e => e.role).HasColumnName("role");
            entity.Property(e => e.uid).HasColumnName("uid");
        });

        modelBuilder.Entity<ApiHostException>(entity =>
        {
        entity.ToTable("ApiHostException");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.ExceptionType).HasMaxLength(100);
        entity.Property(e => e.ApprovedBy).HasMaxLength(255);
        entity.Property(e => e.Active).HasDefaultValue(true);
        entity.Property(e => e.CreatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        modelBuilder.Entity<Usernotice>(entity =>
        {
            entity.ToTable("Usernotice");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
