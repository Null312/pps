using MassTransit;
using PPS.Common;
using PPS.Common.KafkaDto;

namespace Payment_Processing_System.Services.Kafka
{
    public class ConsumerMass : IConsumer<BaseMessageKafka<PaymentDto>>
    {
        public Task Consume(ConsumeContext<BaseMessageKafka<PaymentDto>> context)
        {
            Console.WriteLine($"Kafka message: PaymentId={context.Message.Payload.Payment_id}, Amount={context.Message.Payload.Amount}");
            return Task.CompletedTask;
        }
    }
}
