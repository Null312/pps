using PaymentService.DTOs;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace PaymentService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<PaymentTransaction> Transactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("accounts");
                entity.HasIndex(e => e.UserId).HasDatabaseName("idx_accounts_user_id");
                entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(a => a.User)
                    .WithMany(u => u.Accounts)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.ToTable("transactions");

                entity.HasIndex(e => e.FromAccountId).HasDatabaseName("idx_transactions_from_account");
                entity.HasIndex(e => e.ToAccountId).HasDatabaseName("idx_transactions_to_account");
                entity.HasIndex(e => e.Status).HasDatabaseName("idx_transactions_status");
                entity.HasIndex(e => e.CreatedAt).IsDescending().HasDatabaseName("idx_transactions_created_at");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.FraudReasons).HasColumnType("text[]");

                entity.HasOne(t => t.FromAccount)
                    .WithMany(a => a.TransactionsFrom)
                    .HasForeignKey(t => t.FromAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ToAccount)
                    .WithMany(a => a.TransactionsTo)
                    .HasForeignKey(t => t.ToAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
