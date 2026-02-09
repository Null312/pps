namespace PPS.Common.KafkaDto
{
    public class BaseMessageKafka<T>
    {
        public string Event_id { get; set; }
        public string Event_type { get; set; }
        public DateTime Timestamp { get; set; }
        public T Payload { get; set; }
    }
}
