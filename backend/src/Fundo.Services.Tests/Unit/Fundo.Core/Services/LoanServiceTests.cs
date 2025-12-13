using FluentAssertions;
using Fundo.Core.Models.Loan;
using Fundo.Core.Services;
using Fundo.DAL;
using Fundo.DAL.Entities;
using Fundo.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Unit.Fundo.Core.Services
{
    public class LoanServiceTests
    {
        private FundoDbContext GetInMemoryDbContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<FundoDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            return new FundoDbContext(options);
        }

        [Fact]
        public async Task GetAllLoansAsync_WithNoData_ShouldReturnSuccessWithEmptyResult()
        {
            // Arrange
            using var context = GetInMemoryDbContext("GetAllLoans_Empty");
            var service = new LoanService(context);
            var filter = new GetLoansFilter();

            // Act
            var result = await service.GetAllLoansAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Message.Should().Be("There are no records of loans");
            result.Loans.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllLoansAsync_WithData_ShouldReturnSuccessWithData()
        {
            // Arrange
            using var context = GetInMemoryDbContext("GetAllLoans_WithData");

            var loan = new Loan
            {
                LoanId = 1,
                ApplicantName = "John Doe",
                Amount = 50000.00m,
                CurrentBalance = 30000.00m,
                Status = LoanStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            context.Loans.Add(loan);
            await context.SaveChangesAsync();

            var service = new LoanService(context);
            var filter = new GetLoansFilter();

            // Act
            var result = await service.GetAllLoansAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Loans retrieved successfully");
            result.Loans.Should().HaveCount(1);;
        }

        [Fact]
        public async Task GetAllLoansAsync_WhenContextIsValid_ShouldNotThrowException()
        {
            // Arrange
            using var context = GetInMemoryDbContext("GetAllLoans_NoException");
            var service = new LoanService(context);
            var filter = new GetLoansFilter();

            // Act
            Func<Task> act = async () => await service.GetAllLoansAsync(filter);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllLoansAsync_WithMultipleLoans_ShouldReturnSuccess()
        {
            // Arrange
            using var context = GetInMemoryDbContext("GetAllLoans_MultipleLoans");

            var loans = new List<Loan>
            {
                new Loan
                {
                    LoanId = 1,
                    ApplicantName = "John Doe",
                    Amount = 50000.00m,
                    CurrentBalance = 30000.00m,
                    Status = LoanStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new Loan
                {
                    LoanId = 2,
                    ApplicantName = "Jane Smith",
                    Amount = 75000.00m,
                    CurrentBalance = 0.00m,
                    Status = LoanStatus.Paid,
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                },
                new Loan
                {
                    LoanId = 3,
                    ApplicantName = "Bob Johnson",
                    Amount = 100000.00m,
                    CurrentBalance = 80000.00m,
                    Status = LoanStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                }
            };

            context.Loans.AddRange(loans);
            await context.SaveChangesAsync();

            var service = new LoanService(context);
            var filter = new GetLoansFilter();

            // Act
            var result = await service.GetAllLoansAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Loans.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAllLoansAsync_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            using var context = GetInMemoryDbContext("GetAllLoans_Pagination");

            var loans = Enumerable.Range(1, 15).Select(i => new Loan
            {
                LoanId = i,
                ApplicantName = $"User {i}",
                Amount = 50000.00m + i,
                CurrentBalance = 30000.00m,
                Status = LoanStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            }).ToList();

            context.Loans.AddRange(loans);
            await context.SaveChangesAsync();

            var service = new LoanService(context);
            var filter = new GetLoansFilter { PageNumber = 2, PageSize = 5 };

            // Act
            var result = await service.GetAllLoansAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Loans.Should().HaveCount(5);
        }

        #region CreateLoanAsync Tests

        [Fact]
        public async Task CreateLoanAsync_WithValidRequest_ShouldCreateLoanSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext("CreateLoan_Success");
            var service = new LoanService(context);
            var request = new CreateLoanRequest
            {
                ApplicantName = "John Doe",
                Amount = 50000.00m
            };

            // Act
            var result = await service.CreateLoanAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Loan created successfully");
            result.Loan.Should().NotBeNull();
            result.Loan!.AppllicantName.Should().Be("John Doe");
            result.Loan.Amount.Should().Be(50000.00m);
            result.Loan.CurrentBalance.Should().Be(50000.00m);
            result.Loan.Status.Should().Be(LoanStatus.Active);
        }

        [Fact]
        public async Task CreateLoanAsync_WithZeroAmount_ShouldReturnError()
        {
            // Arrange
            using var context = GetInMemoryDbContext("CreateLoan_ZeroAmount");
            var service = new LoanService(context);
            var request = new CreateLoanRequest
            {
                ApplicantName = "John Doe",
                Amount = 0
            };

            // Act
            var result = await service.CreateLoanAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Amount must be greater than 0");
            result.Loan.Should().BeNull();
        }

        [Fact]
        public async Task CreateLoanAsync_WithEmptyApplicantName_ShouldReturnError()
        {
            // Arrange
            using var context = GetInMemoryDbContext("CreateLoan_EmptyName");
            var service = new LoanService(context);
            var request = new CreateLoanRequest
            {
                ApplicantName = "",
                Amount = 50000.00m
            };

            // Act
            var result = await service.CreateLoanAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Applicant name is required");
        }

        [Fact]
        public async Task CreateLoanAsync_ShouldSaveLoanToDatabase()
        {
            // Arrange
            using var context = GetInMemoryDbContext("CreateLoan_SaveToDb");
            var service = new LoanService(context);
            var request = new CreateLoanRequest
            {
                ApplicantName = "Jane Smith",
                Amount = 75000.00m
            };

            // Act
            var result = await service.CreateLoanAsync(request);

            // Assert
            var savedLoan = await context.Loans.FirstOrDefaultAsync(l => l.ApplicantName == "Jane Smith");
            savedLoan.Should().NotBeNull();
            savedLoan!.Amount.Should().Be(75000.00m);
            savedLoan.CurrentBalance.Should().Be(75000.00m);
            savedLoan.Status.Should().Be(LoanStatus.Active);
        }

        [Fact]
        public async Task CreateLoanAsync_ShouldSetInitialBalanceEqualToAmount()
        {
            // Arrange
            using var context = GetInMemoryDbContext("CreateLoan_InitialBalance");
            var service = new LoanService(context);
            var request = new CreateLoanRequest
            {
                ApplicantName = "Test User",
                Amount = 100000.00m
            };

            // Act
            var result = await service.CreateLoanAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Loan!.CurrentBalance.Should().Be(result.Loan.Amount);
        }

        #endregion
    }
}
