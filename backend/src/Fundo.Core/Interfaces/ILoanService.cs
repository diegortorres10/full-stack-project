using Fundo.Core.Models.Loan;

namespace Fundo.Core.Interfaces
{
    public interface ILoanService
    {
        Task<GetAllLoansResult> GetAllLoansAsync(GetLoansFilter filter);
        Task<CreateLoanResult> CreateLoanAsync(CreateLoanRequest request);
    }
}
