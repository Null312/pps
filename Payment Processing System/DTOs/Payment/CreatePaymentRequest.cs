namespace Payment_Processing_System.DTOs.Payment
{
    public record CreatePaymentRequest
    {
        public string FromAccountId { get; set; } = string.Empty;
        public string ToAccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Description { get; set; } = string.Empty;
    }
}
