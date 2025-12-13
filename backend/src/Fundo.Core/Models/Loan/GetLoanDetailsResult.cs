using Fundo.Core.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fundo.Core.Models.Loan
{
    public class GetLoanDetailsResult : BaseResult
    {
        public List<LoanPaymentDto> Details { get; set; } = new List<LoanPaymentDto>();
        public int TotalItems { get; set; }
    }
}
