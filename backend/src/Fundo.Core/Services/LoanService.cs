using Fundo.Core.Interfaces;
using Fundo.Core.Models.Common;
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

        public async Task<GetLoanDetailsResult> GetLoanDetailsAsync(long loanId, PaginationFilter pagination)
        {
            try
            {
                // Check loan exists
                var loan = _context.Loans
                    .Where(l => l.LoanId == loanId)
                    .FirstOrDefault();

                if (loan == null)
                {
                    return new GetLoanDetailsResult
                    {
                        Success = false,
                        Message = "Loan doesn't exists. Check the information"
                    };
                }

                var query = _context.LoanPayments
                    .Where(l => l.LoanId == loanId)
                    .AsQueryable();

                query = query.OrderByDescending(l => l.CreatedAt);

                // Pagination
                var totalCount = await query.CountAsync();
                var skip = (pagination.PageNumber - 1) * pagination.PageSize;
                var loanDetails = await query
                    .Skip(skip)
                    .Take(pagination.PageSize)
                    .Select(l => new LoanPaymentDto
                    {
                        LoanPaymentId = l.LoanPaymentId,
                        Amount = l.Amount,
                        CreatedAt = l.CreatedAt
                    })
                    .ToListAsync();

                if (totalCount == 0)
                {
                    return new GetLoanDetailsResult
                    {
                        Success = true,
                        Message = "No loan payments have been recorded yet",
                        Details = new List<LoanPaymentDto>()
                    };
                }

                return new GetLoanDetailsResult
                {
                    Success = true,
                    Message = "Loan payments retrieved successfully",
                    Details = loanDetails,
                    TotalItems = totalCount
                };
            }
            catch (Exception ex)
            {
                return new GetLoanDetailsResult
                {
                    Success = false,
                    Message = $"Error getting loan details: {ex.Message}"
                };
            }
        }

        public async Task<CreateLoanPaymentResult> CreateLoanPaymentAsync(long loanId, CreateLoanPaymentRequest request)
        {
            try
            {
                // Validation
                if (request.Amount <= 0)
                {
                    return new CreateLoanPaymentResult
                    {
                        Success = false,
                        Message = "Amount must be greater than 0"
                    };
                }

                // Check loan exists
                var loan = await _context.Loans
                    .Where(l => l.LoanId == loanId)
                    .FirstOrDefaultAsync();

                if (loan == null)
                {
                    return new CreateLoanPaymentResult
                    {
                        Success = false,
                        Message = "Loan doesn't exists. Check the information"
                    };
                }

                // Check if loan is already paid
                if (loan.Status == LoanStatus.Paid)
                {
                    return new CreateLoanPaymentResult
                    {
                        Success = false,
                        Message = "This loan has already been paid in full"
                    };
                }

                // Check if payment amount exceeds current balance
                if (request.Amount > loan.CurrentBalance)
                {
                    return new CreateLoanPaymentResult
                    {
                        Success = false,
                        Message = $"Payment amount {request.Amount} exceeds current balance {loan.CurrentBalance}"
                    };
                }

                // Create new payment loan
                var loanPayment = new LoanPayment
                {
                    LoanId = loanId,
                    Amount = request.Amount,
                    CreatedAt = DateTime.UtcNow
                };

                // Deduct from current balance
                loan.CurrentBalance -= request.Amount;

                // Update loan status to Paid if balance reaches zero
                if (loan.CurrentBalance == 0)
                {
                    loan.Status = LoanStatus.Paid;
                }

                // Add payment and update loan in a single transaction. If either fails, both will be rolled back
                _context.LoanPayments.Add(loanPayment);
                await _context.SaveChangesAsync();

                // Return success with new loan payment
                return new CreateLoanPaymentResult
                {
                    Success = true,
                    Message = "Loan payment created successfully",
                    LoanPayment = new LoanPaymentDto
                    {
                        LoanPaymentId = loanPayment.LoanPaymentId,
                        Amount = loanPayment.Amount,
                        CreatedAt = loanPayment.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new CreateLoanPaymentResult
                {
                    Success = false,
                    Message = $"Error creating loan payment: {ex.Message}"
                };
            }
        }
    }
}
