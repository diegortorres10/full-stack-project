using Fundo.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundo.DAL
{
    public class FundoDbContext : DbContext
    {
        public FundoDbContext(DbContextOptions<FundoDbContext> options) : base(options)
        {
        }

        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanPayment> LoanPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Loan
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("Loans");

                entity.HasKey(e => e.LoanId);

                entity.Property(e => e.ApplicantName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Amount)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.CurrentBalance)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(10);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ApplicantName);
                entity.HasIndex(e => e.CreatedAt);
            });

            // LoanPayment
            modelBuilder.Entity<LoanPayment>(entity =>
            {
                entity.ToTable("LoanPayments");

                entity.HasKey(e => e.LoanPaymentId);

                entity.Property(e => e.Amount)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Loan)
                    .WithMany(e => e.LoanPayments)
                    .HasForeignKey(e => e.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.LoanId);
            });
        }
    }
}
