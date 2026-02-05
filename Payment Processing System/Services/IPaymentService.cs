using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Payment_Processing_System.Data;
using Payment_Processing_System.DTOs.Payment;
using PPS.Common;

namespace Payment_Processing_System.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
        Task<PaymentDetailsResponse?> GetPaymentAsync(string paymentId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly ITopicProducer<CreatePaymentRequest> _producer;

        public PaymentService(ApplicationDbContext context, IAccountService accountService, ITopicProducer<CreatePaymentRequest> producer)
        {
            _context = context;
            _accountService = accountService;
            _producer = producer;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {

            if (string.IsNullOrEmpty(request.FromAccountId ) ||string.IsNullOrEmpty(request.ToAccountId))
                throw new InvalidOperationException("One or both accounts not found");

            if (request.Amount<1)
                throw new InvalidOperationException("Insufficient funds");

            var payment = new Payment
            {
                PaymentId = $"pay_{Guid.NewGuid().ToString("N")[..8]}",
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                Description = request.Description,
                Status = "PROCESSING",
                CreatedAt = DateTime.UtcNow
            };

            
            await _producer.Produce(payment);

            return new PaymentResponse
            {
                PaymentId = payment.PaymentId,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            };
        }

        public async Task<PaymentDetailsResponse?> GetPaymentAsync(string paymentId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                return null;

            return new PaymentDetailsResponse
            {
                PaymentId = payment.PaymentId,
                FromAccountId = payment.FromAccountId,
                ToAccountId = payment.ToAccountId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Description = payment.Description,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt,
                CompletedAt = payment.CompletedAt
            };

        }
    }
}
