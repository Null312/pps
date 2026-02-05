namespace Payment_Processing_System.DTOs.Payment
{
    public class PaymentDetailsResponse
    {
        public string PaymentId { get; set; } = string.Empty;
        public string FromAccountId { get; set; } = string.Empty;
        public string ToAccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
