using System.ComponentModel.DataAnnotations;

namespace Fundo.Core.Models.Common
{
    public class PaginationFilter
    {
        [Required]
        public int PageNumber { get; set; } = 1;
        [Required]
        public int PageSize { get; set; } = 10;
    }
}
