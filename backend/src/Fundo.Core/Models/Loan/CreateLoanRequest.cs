using System.ComponentModel.DataAnnotations;

namespace Fundo.Core.Models.Loan
{
    public class CreateLoanRequest
    {
        [Required(ErrorMessage = "Applicant name is required")]
        [StringLength(100, ErrorMessage = "Applicant name cannot exceed 100 characters")]
        public string ApplicantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}
