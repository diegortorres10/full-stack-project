using Fundo.Core.Interfaces;
using Fundo.Core.Models.Loan;
using Fundo.DAL;
using Fundo.DAL.Entities;
using Fundo.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Core.Services
{
    public class LoanService : ILoanService
    {
        private readonly FundoDbContext _context;

        public LoanService(FundoDbContext context)
        {
            _context = context;
        }

        public async Task<GetAllLoansResult> GetAllLoansAsync(GetLoansFilter filter)
        {
            try
            {
                var query = _context.Loans.AsQueryable();

                // DEfault sort by createdAt
                query = query.OrderByDescending(l => l.CreatedAt);

                // Pagination
                var totalCount = await query.CountAsync();
                var skip = (filter.PageNumber - 1) * filter.PageSize;
                var loans = await query
                    .Skip(skip)
                    .Take(filter.PageSize)
                    .Select(l => new LoanDto
                    {
                        LoanId = l.LoanId,
                        AppllicantName = l.ApplicantName,
                        Amount = l.Amount,
                        CurrentBalance = l.CurrentBalance,
                        Status = l.Status,
                        CreatedAt = l.CreatedAt
                    })
                    .ToListAsync();

                if (totalCount == 0)
                {
                    return new GetAllLoansResult
                    {
                        Success = true,
                        Message = "There are no records of loans",
                        Loans = new List<LoanDto>()
                    };
                }

                return new GetAllLoansResult
                {
                    Success = true,
                    Message = "Loans retrieved successfully",
                    Loans = loans,
                    TotalItems = totalCount
                };
            }
            catch (Exception ex)
            {
                return new GetAllLoansResult
                {
                    Success = false,
                    Message = $"Error retrieving loans: {ex.Message}",
                    Loans = new List<LoanDto>()
                };
            }
        }

        public async Task<CreateLoanResult> CreateLoanAsync(CreateLoanRequest request)
        {
            try
            {
                // Validation
                if (request.Amount <= 0)
                {
                    return new CreateLoanResult
                    {
                        Success = false,
                        Message = "Amount must be greater than 0"
                    };
                }

                if (string.IsNullOrWhiteSpace(request.ApplicantName))
                {
                    return new CreateLoanResult
                    {
                        Success = false,
                        Message = "Applicant name is required"
                    };
                }

                // Create new Loan
                var loan = new Loan
                {
                    ApplicantName = request.ApplicantName.Trim(),
                    Amount = request.Amount,
                    CurrentBalance = request.Amount, // Initial balance equal to amount
                    Status = LoanStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Loans.Add(loan);
                await _context.SaveChangesAsync();

                // Return success with new loan
                return new CreateLoanResult
                {
                    Success = true,
                    Message = "Loan created successfully",
                    Loan = new LoanDto
                    {
                        LoanId = loan.LoanId,
                        AppllicantName = loan.ApplicantName,
                        Amount = loan.Amount,
                        CurrentBalance = loan.CurrentBalance,
                        Status = loan.Status,
                        CreatedAt = loan.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new CreateLoanResult
                {
                    Success = false,
                    Message = $"Error creating loan: {ex.Message}"
                };
            }
        }
    }
}
