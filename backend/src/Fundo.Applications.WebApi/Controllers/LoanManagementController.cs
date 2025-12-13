using Fundo.Core.Interfaces;
using Fundo.Core.Models.Common;
using Fundo.Core.Models.Loan;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Controllers
{
    [Route("/loans")]
    public class LoanManagementController : Controller
    {
        private readonly ILoanService _loanService;

        public LoanManagementController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetLoansFilter filter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _loanService.GetAllLoansAsync(filter);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateLoanRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _loanService.CreateLoanAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(Get), new { id = result.Loan?.LoanId }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id, [FromQuery] PaginationFilter pagination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _loanService.GetLoanDetailsAsync(id, pagination);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("{id}/payment")]
        public async Task<IActionResult> CreatePayment(long id, [FromBody] CreateLoanPaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _loanService.CreateLoanPaymentAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id }, result);
        }
    }
}