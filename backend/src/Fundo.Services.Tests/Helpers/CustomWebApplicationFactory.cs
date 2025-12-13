using Fundo.DAL;
using Fundo.DAL.Entities;
using Fundo.DAL.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fundo.Services.Tests.Helpers
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<FundoDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add an in-memory DbContext with a database
                services.AddDbContext<FundoDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                var sp = services.BuildServiceProvider();

                // Create a scope to obtain the DbContext and perform seeding
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<FundoDbContext>();
                    db.Database.EnsureCreated();
                    SeedTestData(db);
                }
            });
        }

        private void SeedTestData(FundoDbContext context)
        {
            // Remove data
            context.LoanPayments.RemoveRange(context.LoanPayments);
            context.Loans.RemoveRange(context.Loans);
            context.SaveChanges();

            // Add data
            var loans = new List<Loan>
            {
                new Loan
                {
                    LoanId = 1,
                    ApplicantName = "Test User 1",
                    Amount = 50000.00m,
                    CurrentBalance = 30000.00m,
                    Status = LoanStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new Loan
                {
                    LoanId = 2,
                    ApplicantName = "Test User 2",
                    Amount = 75000.00m,
                    CurrentBalance = 0.00m,
                    Status = LoanStatus.Paid,
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                },
                new Loan
                {
                    LoanId = 3,
                    ApplicantName = "Test User 3",
                    Amount = 100000.00m,
                    CurrentBalance = 80000.00m,
                    Status = LoanStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                }
            };

            context.Loans.AddRange(loans);
            context.SaveChanges();

            // Add payments
            var payments = new List<LoanPayment>
            {
                new LoanPayment
                {
                    LoanPaymentId = 1,
                    LoanId = 1,
                    Amount = 20000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-25)
                },
                new LoanPayment
                {
                    LoanPaymentId = 2,
                    LoanId = 2,
                    Amount = 75000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-50)
                },
                new LoanPayment
                {
                    LoanPaymentId = 3,
                    LoanId = 3,
                    Amount = 20000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                }
            };

            context.LoanPayments.AddRange(payments);
            context.SaveChanges();
        }
    }
}
