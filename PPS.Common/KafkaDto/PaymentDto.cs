namespace PPS.Common
{
    public class PaymentDto
    {
        public string Payment_id { get; set; } = string.Empty;
        public string From_account_id { get; set; } = string.Empty;
        public string To_account_id { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Description { get; set; } = string.Empty;
        //public string Status { get; set; } = "PROCESSING"; // PROCESSING, COMPLETED, FAILED
        //public DateTime CreatedAt { get; set; } 
        //public DateTime? CompletedAt { get; set; }
    }
}
