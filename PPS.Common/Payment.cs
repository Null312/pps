namespace PPS.Common
{
    public class Payment
    {
        public string PaymentId { get; set; } = string.Empty;
        public string FromAccountId { get; set; } = string.Empty;
        public string ToAccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "PROCESSING"; // PROCESSING, COMPLETED, FAILED
        public DateTime CreatedAt { get; set; } 
        public DateTime? CompletedAt { get; set; }
    }
}
