using MassTransit;
using PaymentService.Data;
using PPS.Common;

namespace PaymentService.Serivce.Kafka
{
    public class PaymentCreatedConsumer : IConsumer<Payment>
    {
        private readonly ILogger<PaymentCreatedConsumer> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly ITopicProducer<Payment> _producer;

        public PaymentCreatedConsumer(
            ILogger<PaymentCreatedConsumer> logger,
            ApplicationDbContext dbContext,
            ITopicProducer<Payment> producer)
        {
            _logger = logger;
            _dbContext = dbContext;
            _producer = producer;
        }

        public async Task Consume(ConsumeContext<Payment> context)
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

                message.Status = "COMPLETED";
                message.CompletedAt = DateTime.UtcNow;

                await _producer.Produce(message);

                _logger.LogInformation(
                    "Платеж успешно обработан и отправлен в payment.processed. TransactionId: {TransactionId}",
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
