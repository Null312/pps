using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment_Processing_System.DTOs.Payment;
using Payment_Processing_System.Services;

namespace Payment_Processing_System.Controllers
{


    [ApiController]
    [Route("api/v1/payments")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (request.Amount <= 0)
                return BadRequest(new { message = "Amount must be greater than 0" });

            var response = await _paymentService.CreatePaymentAsync(request);
            return StatusCode(202, response); // HTTP 202 Accepted
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(string id)
        {
            var payment = await _paymentService.GetPaymentAsync(id);

            if (payment == null)
                return NotFound(new { message = "Payment not found" });

            return Ok(payment);
        }
    }
}
