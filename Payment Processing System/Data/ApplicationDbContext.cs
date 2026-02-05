using Microsoft.EntityFrameworkCore;
using PPS.Common;

namespace Payment_Processing_System.Data
{


    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50);
            });

            // Account Configuration
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccountId);
                entity.Property(e => e.AccountId).HasMaxLength(50);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.HasIndex(e => e.UserId);
            });

            // Payment Configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.PaymentId).HasMaxLength(50);
                entity.Property(e => e.FromAccountId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ToAccountId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.HasIndex(e => e.FromAccountId);
                entity.HasIndex(e => e.ToAccountId);
                entity.HasIndex(e => e.Status);
            });

            // Seed Data (опционально)
            //modelBuilder.Entity<Account>().HasData(
            //    new Account
            //    {
            //        AccountId = "acc_123",
            //        UserId = "1",
            //        Balance = 1000.00m,
            //        Currency = "USD",
            //        Status = "ACTIVE",
            //        CreatedAt = DateTime.UtcNow
            //    },
            //    new Account
            //    {
            //        AccountId = "acc_456",
            //        UserId = "2",
            //        Balance = 500.00m,
            //        Currency = "USD",
            //        Status = "ACTIVE",
            //        CreatedAt = DateTime.UtcNow
            //    }
            //);
        }
    }
}
