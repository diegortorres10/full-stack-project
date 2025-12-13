using Fundo.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fundo.DAL.Entities
{
    public class Loan
    {
        [Key]
        public long LoanId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; }

        public LoanStatus Status { get; set; }

        public string ApplicantName { get; set; } = string.Empty;
        
        [Required]
        public DateTime CreatedAt { get; set; }

        public ICollection<LoanPayment> LoanPayments { get; set; } = new List<LoanPayment>();
    }
}
