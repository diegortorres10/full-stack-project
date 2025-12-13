using Fundo.Core.Models.Common;

namespace Fundo.Core.Models.Loan
{
    public class CreateLoanResult : BaseResult
    {
        public LoanDto? Loan { get; set; }
    }
}
