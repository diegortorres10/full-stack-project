using System.ComponentModel.DataAnnotations.Schema;

namespace Fundo.Core.Models.Loan
{
    public class LoanPaymentDto
    {
        public long LoanPaymentId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
