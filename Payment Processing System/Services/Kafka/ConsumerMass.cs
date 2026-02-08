using MassTransit;
using PPS.Common;

namespace Payment_Processing_System.Services.Kafka
{
    public class ConsumerMass : IConsumer<Payment>
    {
        public Task Consume(ConsumeContext<Payment> context)
        {
            Console.WriteLine($"Kafka message: PaymentId={context.Message.PaymentId}, Amount={context.Message.Amount}");
            return Task.CompletedTask;
        }
    }
}
