using Fundo.DAL.Entities;
using Fundo.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fundo.DAL
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(FundoDbContext context)
        {
            // Ensure the database has been created
            await context.Database.MigrateAsync();

            // Check if data already exists
            if (await context.Loans.AnyAsync())
            {
                return;
            }

            // Seed data loans
            var loans = new List<Loan>
            {
                new Loan
                {
                    ApplicantName = "Juan Pérez",
                    Amount = 50000.00m,
                    CurrentBalance = 35000.00m,
                    Status = LoanStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                },
                new Loan
                {
                    ApplicantName = "María García",
                    Amount = 100000.00m,
                    CurrentBalance = 0.00m,
                    Status = LoanStatus.Paid,
                    CreatedAt = DateTime.UtcNow.AddDays(-180)
                },
                new Loan
                {
                    ApplicantName = "Carlos Rodríguez",
                    Amount = 75000.00m,
                    CurrentBalance = 60000.00m,
                    Status = LoanStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                },
                new Loan
                {
                    ApplicantName = "Ana Martínez",
                    Amount = 30000.00m,
                    CurrentBalance = 15000.00m,
                    Status = LoanStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-120)
                },
                new Loan
                {
                    ApplicantName = "Luis González",
                    Amount = 80000.00m,
                    CurrentBalance = 0.00m,
                    Status = LoanStatus.Paid,
                    CreatedAt = DateTime.UtcNow.AddDays(-200)
                }
            };

            await context.Loans.AddRangeAsync(loans);
            await context.SaveChangesAsync();

            // Seed data loan payments
            var payments = new List<LoanPayment>
            {
                // Loan 1
                new LoanPayment
                {
                    LoanId = loans[0].LoanId,
                    Amount = 10000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-85)
                },
                new LoanPayment
                {
                    LoanId = loans[0].LoanId,
                    Amount = 5000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                },

                // Loan 2
                new LoanPayment
                {
                    LoanId = loans[1].LoanId,
                    Amount = 50000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-150)
                },
                new LoanPayment
                {
                    LoanId = loans[1].LoanId,
                    Amount = 30000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-120)
                },
                new LoanPayment
                {
                    LoanId = loans[1].LoanId,
                    Amount = 20000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                },

                // Loan 3
                new LoanPayment
                {
                    LoanId = loans[2].LoanId,
                    Amount = 15000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-45)
                },

                // Loan 4
                new LoanPayment
                {
                    LoanId = loans[3].LoanId,
                    Amount = 7500.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-100)
                },
                new LoanPayment
                {
                    LoanId = loans[3].LoanId,
                    Amount = 7500.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-70)
                },

                // Loan 5
                new LoanPayment
                {
                    LoanId = loans[4].LoanId,
                    Amount = 40000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-170)
                },
                new LoanPayment
                {
                    LoanId = loans[4].LoanId,
                    Amount = 40000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-140)
                }
            };

            await context.LoanPayments.AddRangeAsync(payments);
            await context.SaveChangesAsync();
        }
    }
}
