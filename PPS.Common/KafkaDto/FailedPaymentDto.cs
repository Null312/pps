namespace PPS.Common.KafkaDto
{
    public class FailedPaymentDto
    {
        public string Payment_id { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }
    }
}
