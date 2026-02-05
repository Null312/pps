using MassTransit;
using Payment_Processing_System.DTOs.Payment;

namespace Payment_Processing_System.Services.Kafka
{
    public class ConsumerMass : IConsumer<CreatePaymentRequest>
    {
        public Task Consume(ConsumeContext<CreatePaymentRequest> context)
        {
            Console.WriteLine($"Kafka message: {context.Message}");
            return Task.CompletedTask;
        }
    }
}
