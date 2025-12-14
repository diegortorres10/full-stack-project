using Fundo.Core.Models.Common;

namespace Fundo.Core.Models.Loan
{
    public class GetLoansFilter : PaginationFilter
    {
        public string? ApplicantName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
