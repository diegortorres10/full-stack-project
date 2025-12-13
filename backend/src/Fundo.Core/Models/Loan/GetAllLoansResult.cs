using Fundo.Core.Models.Common;

namespace Fundo.Core.Models.Loan
{
    public class GetAllLoansResult : BaseResult
    {
        public List<LoanDto> Loans { get; set; } = new List<LoanDto>();
        public int TotalItems { get; set; }
    }
}
