using Fundo.DAL.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fundo.Core.Models.Loan
{
    public class LoanDto
    {
        public long LoanId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; }

        public LoanStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string AppllicantName { get; set; } = string.Empty;
    }
}
