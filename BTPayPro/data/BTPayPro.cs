using BTPayPro.Autmpay.Models;
using BTPayPro.CPMPay.Models;
using BTPayPro.Domaine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTPayPro.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Add DbSets for your entities
        public DbSet<Accounting> Accountings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<DetailRecord> DetailRecords { get; set; }
        public DbSet<HeaderRecord> headerRecords { get; set; }
        public DbSet<TrailerRecord> trailerRecords { get; set; }
        public DbSet<FileDetailRecord> FileDetailRecords { get; set; }
        public DbSet<FileHeaderRecord> FileHeaderRecords { get; set; }
        public DbSet<FileTrailerRecord> FileTrailerRecords { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Accounting>().ToTable("Accountings");
            modelBuilder.Entity<Complaint>().ToTable("Complaints");
            modelBuilder.Entity<Transaction>().ToTable("Transactions");
            modelBuilder.Entity<Wallet>().ToTable("Wallets");
            modelBuilder.Entity<DetailRecord>().ToTable("DetailRecords").HasNoKey();
            modelBuilder.Entity<HeaderRecord>().ToTable("HeaderRecords").HasNoKey();
            modelBuilder.Entity<TrailerRecord>().ToTable("TrailerRecord").HasNoKey();
            modelBuilder.Entity<FileDetailRecord>().ToTable("FileDetailRecords").HasNoKey();
            modelBuilder.Entity<FileHeaderRecord>().ToTable("FileHeaderRecords").HasNoKey();
            modelBuilder.Entity<FileTrailerRecord>().ToTable("FileTrailerRecord").HasNoKey();
            // User and Wallet (One-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId);

            // User and Complaint (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Complaints)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);

            // Wallet and Transaction (One-to-Many)
            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.Transactions)
                .WithOne(t => t.Wallet)
                .HasForeignKey(t => t.WalletId);

            // Wallet and Accounting (One-to-One)
            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.Accounting)
                .WithOne(a => a.Wallet)
                .HasForeignKey<Wallet>(w => w.AccountId);
        }
    }
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Use the same connection string as in appsettings.json
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=BTPayPro;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
