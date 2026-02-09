using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PPS.Common;
using PPS.Common.KafkaDto;

namespace PaymentService.Serivce.Kafka
{
    public class PaymentCreatedConsumer : IConsumer<BaseMessageKafka<PaymentDto>>
    {
        private readonly ILogger<PaymentCreatedConsumer> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly ITopicProducer<BaseMessageKafka<ValidatedPaymentDto>> _producer;
        private readonly ITopicProducer<BaseMessageKafka<FailedPaymentDto>> _producerFailedPayment;

        public PaymentCreatedConsumer(
            ILogger<PaymentCreatedConsumer> logger,
            ApplicationDbContext dbContext,
            ITopicProducer<BaseMessageKafka<ValidatedPaymentDto>> producer, ITopicProducer<BaseMessageKafka<FailedPaymentDto>> producerFailedPayment)
        {
            _logger = logger;
            _dbContext = dbContext;
            _producer = producer;
            _producerFailedPayment = producerFailedPayment;
        }

        public async Task Consume(ConsumeContext<BaseMessageKafka<PaymentDto>> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Получено сообщение о создании платежа. TransactionId: {TransactionId}, Amount: {Amount}",
                message.Payload.Payment_id,
                message.Payload.Amount);

            try
            {

                var from = await _dbContext.Accounts.FirstOrDefaultAsync(f => f.AccountNumber == message.Payload.From_account_id);
                var to = await _dbContext.Accounts.FirstOrDefaultAsync(f => f.AccountNumber == message.Payload.To_account_id);


                if (!await ValidateData(from, to, message.Payload))
                {
                    return;
                }

                
                var messageValidate = new BaseMessageKafka<ValidatedPaymentDto>
                {
                    Event_id = $"evt_{Guid.NewGuid().ToString("N")[..8]}",
                    Event_type = "payment.validated",
                    Timestamp = DateTime.Now,
                    Payload = new ValidatedPaymentDto
                    {
                        Payment_id = message.Payload.Payment_id,
                        From_account_id = from.AccountNumber,
                        To_account_id = to.AccountNumber,
                        Amount = message.Payload.Amount,
                        Currency = message.Payload.Currency,
                        From_user_id = from.UserId.ToString(),
                        From_account_balance = from.Balance,
                        From_account_age_days = (DateTime.Now.Date - from.CreatedAt.Date).Days

                    }
                };

                await _producer.Produce(messageValidate);

                _logger.LogInformation(
                    "Платеж успешно обработан и отправлен в payment.processed. TransactionId: {TransactionId}",
                    message.Payload.Payment_id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка при обработке платежа. TransactionId: {TransactionId}",
                    message.Payload.Payment_id);

                throw; // MassTransit повторит обработку согласно политике retry
            }
        }


        public async Task<bool> ValidateData(DTOs.Account from, DTOs.Account to, PaymentDto message)
        {
            if (from == null)
            {
                await SendFailedPayment(message.Payment_id, "Account not found",
                     $"From account {message.From_account_id} not found");
                return false;
            }

            if (from.IsBlocked)
            {
                await SendFailedPayment(message.Payment_id, "Account is blocked",
                     $"From account {message.From_account_id} is blocked");
                return false;
            }

            if (from.Balance < message.Amount)
            {
                await SendFailedPayment(message.Payment_id, "Insufficient funds",
                     $"Insufficient funds from account  {message.From_account_id}");
                return false;
            }

            if (from.Currency != message.Currency)
            {
                await SendFailedPayment(message.Payment_id, "Currency incorrect",
                     $"From account currency incorrect  {message.Currency}");
                return false;
            }

            if (to == null)
            {
                await SendFailedPayment(message.Payment_id, "Account not found",
                     $"To account {message.From_account_id} not found");
                return false;
            }

            if (to.Currency != message.Currency)
            {
                await SendFailedPayment(message.Payment_id, "Currency incorrect",
                    $"To account currency incorrect  {message.Currency}");
                return false;
            }

            return true;
        }
        

        public async Task SendFailedPayment(string paymentId, string reason, string message)
        {
            var errorMessage = new BaseMessageKafka<FailedPaymentDto>
            {
                Event_id = $"evt_{Guid.NewGuid().ToString("N")[..8]}",
                Event_type = "payment.failed",
                Timestamp = DateTime.Now,
                Payload = new FailedPaymentDto
                {
                    Payment_id = paymentId,
                    Reason = reason,
                    Message = message

                }
            };
            await _producerFailedPayment.Produce(errorMessage);
        }
    }
}
