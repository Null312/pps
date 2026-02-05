using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment_Processing_System.DTOs.Payment;
using Payment_Processing_System.Services;

namespace Payment_Processing_System.Controllers
{


    [ApiController]
    [Route("api/v1/accounts")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(string id)
        {
            var account = await _accountService.GetAccountAsync(id);

            if (account == null)
                return NotFound(new { message = "Account not found" });

            var response = new AccountResponse
            {
                AccountId = account.AccountId,
                Balance = account.Balance,
                Currency = account.Currency,
                Status = account.Status,
                CreatedAt = account.CreatedAt
            };

            return Ok(response);
        }
    }
}
