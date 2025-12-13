using Fundo.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace Fundo.Core.Models.Loan
{
    public class GetLoansFilter
    {
        // TODO: PaginationDTO
        [Required]
        public int PageNumber { get; set; } = 1;
        [Required]
        public int PageSize { get; set; } = 10;
    }
}
