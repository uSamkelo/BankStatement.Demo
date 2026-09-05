using BankStatement.Demo.Entities;
using BankStatement.Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace BankStatement.Demo.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<BankStatementEntity> BankStatements { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }
        public DbSet<PdfLinkToken> PdfLinkTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankStatementEntity>(entity =>
            {
                entity.ToTable("bank_statements");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.StatementId).IsUnique();

                entity.Property(e => e.StartingBalance).HasPrecision(18, 2);
                entity.Property(e => e.EndingBalance).HasPrecision(18, 2);
                entity.Property(e => e.TotalDeposits).HasPrecision(18, 2);
                entity.Property(e => e.TotalWithdrawals).HasPrecision(18, 2);

                // Store Dictionary as jsonb column in PostgreSQL
                entity.Property(e => e.CategoryBreakdown)
                      .HasColumnType("jsonb");
            });

            modelBuilder.Entity<TransactionEntity>(entity =>
            {
                entity.ToTable("transactions");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Amount).HasPrecision(18, 2);

                entity.HasOne(e => e.BankStatement)
                      .WithMany(s => s.Transactions)
                      .HasForeignKey(e => e.BankStatementId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Option 1: Map Merchant directly as JSONB in PostgreSQL
                entity.OwnsOne(e => e.Merchant, builder =>
                {
                    builder.ToJson();
                });
            });

            modelBuilder.Entity<PdfLinkToken>(entity =>
            {
                entity.ToTable("pdf_link_tokens");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Signature).IsUnique();
            });
        }
    }
}
