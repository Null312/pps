using FraudDetectionService.DTOs;
using MassTransit;
using PPS.Common.KafkaDto;

namespace FraudDetectionService.Service
{
    public class FraudConsumer : IConsumer<BaseMessageKafka<ValidatedPaymentDto>>
    {
        private readonly ILogger<FraudConsumer> _logger;
        private readonly IRedisService _redis;

        private readonly ITopicProducer<BaseMessageKafka<FraudPaymentDto>> _producer;

        public FraudConsumer(
            ILogger<FraudConsumer> logger,

            ITopicProducer<BaseMessageKafka<FraudPaymentDto>> producer, IRedisService redis)
        {
            _logger = logger;

            _producer = producer;
            _redis = redis;
        }

        public async Task Consume(ConsumeContext<BaseMessageKafka<ValidatedPaymentDto>> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Получено сообщение о создании платежа. TransactionId: {TransactionId}, Amount: {Amount}",
                message.Payload.Payment_id,
                message.Payload.Amount);

            try
            {
                var user = await GetUserProfileAsync(message.Payload.From_user_id);



                var messageValidate = new BaseMessageKafka<ValidatedPaymentDto>
                {
                    Event_id = $"evt_{Guid.NewGuid().ToString("N")[..8]}",
                    Event_type = "payment.fraud-checked",
                    Timestamp = DateTime.Now,
                    Payload = new ValidatedPaymentDto
                    {
                        //Payment_id = message.Payload.Payment_id,
                        //From_account_id = from.AccountNumber,
                        //To_account_id = to.AccountNumber,
                        //Amount = message.Payload.Amount,
                        //Currency = message.Payload.Currency,
                        //From_user_id = from.UserId.ToString(),
                        //From_account_balance = from.Balance,
                        //From_account_age_days = (DateTime.Now.Date - from.CreatedAt.Date).Days

                    }
                };

                await _producer.Produce(messageValidate);

                user.account_age_days++;

                user.last_updated=DateTime.Now;

                await UpdateUserProfileAsync(user);

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

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            var cacheKey = $"user_profile:{userId}";

            // Пытаемся получить из кеша
            var cachedProfile = await _redis.GetAsync<UserProfileDto>(cacheKey);
            if (cachedProfile != null)
            {
                return cachedProfile;
            }

            UserProfileDto profile = new UserProfileDto() { User_id = userId };

            // Сохраняем в кеш на 1 час
            await _redis.SetAsync(cacheKey, profile, TimeSpan.FromHours(1));

            return profile;
        }

        public async Task UpdateUserProfileAsync(UserProfileDto profile)
        {
            // Обновляем кеш
            var cacheKey = $"user_profile:{profile.User_id}";
            await _redis.SetAsync(cacheKey, profile, TimeSpan.FromHours(1));
        }
    }
}
