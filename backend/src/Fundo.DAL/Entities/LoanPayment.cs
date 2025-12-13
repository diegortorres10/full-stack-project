using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fundo.DAL.Entities
{
    public class LoanPayment
    {
        [Key]
        public long LoanPaymentId { get; set; }

        [Required]
        public Loan Loan { get; set; } = null!;
        public long LoanId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
