using Fundo.Core.Models.Common;
using Fundo.Core.Models.Loan;

namespace Fundo.Core.Interfaces
{
    public interface ILoanService
    {
        Task<GetAllLoansResult> GetAllLoansAsync(GetLoansFilter filter);
        Task<CreateLoanResult> CreateLoanAsync(CreateLoanRequest request);
        Task<GetLoanDetailsResult> GetLoanDetailsAsync(long loanId, PaginationFilter pagination);
        Task<CreateLoanPaymentResult> CreateLoanPaymentAsync(long loanId, CreateLoanPaymentRequest request);
    }
}
