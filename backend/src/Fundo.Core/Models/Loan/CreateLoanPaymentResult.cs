using Fundo.Core.Models.Common;

namespace Fundo.Core.Models.Loan
{
    public class CreateLoanPaymentResult : BaseResult
    {
        public LoanPaymentDto? LoanPayment { get; set; }
    }
}
