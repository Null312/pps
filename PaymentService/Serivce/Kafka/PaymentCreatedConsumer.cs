using MassTransit;
using PaymentService.Data;

namespace PaymentService.Serivce.Kafka
{
    public class PaymentCreatedConsumer : IConsumer<CreatePaymentRequest>
    {
        private readonly ILogger<PaymentCreatedConsumer> _logger;
        private readonly ApplicationDbContext _dbContext;

        public PaymentCreatedConsumer(
            ILogger<PaymentCreatedConsumer> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<CreatePaymentRequest> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Получено сообщение о создании платежа. TransactionId: {TransactionId}, Amount: {Amount}",
                message.PaymentId,
                message.Amount);

            try
            {
                // Ваша бизнес-логика обработки платежа
                //var transaction = new PaymentTransaction
                //{
                //    Id = message.PaymentId,
                //    FromAccountId = message.FromAccountId,
                //    ToAccountId = message.ToAccountId,
                //    Amount = message.Amount,
                //    Currency = message.Currency,
                //    Description = message.Description,
                //    Status = TransactionStatus.PendingValidation,
                //    CreatedAt = message.CreatedAt,
                //    UpdatedAt = DateTime.UtcNow
                //};

                //await _dbContext.Transactions.AddAsync(transaction);
                //await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "Платеж успешно обработан. TransactionId: {TransactionId}",
                    message.PaymentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка при обработке платежа. TransactionId: {TransactionId}",
                    message.PaymentId);

                throw; // MassTransit повторит обработку согласно политике retry
            }
        }
    }
}
