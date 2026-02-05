namespace Payment_Processing_System.DTOs.Payment
{
    public class PaymentResponse
    {
        public string PaymentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
