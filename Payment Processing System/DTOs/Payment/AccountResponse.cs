namespace Payment_Processing_System.DTOs.Payment
{
    public class AccountResponse
    {
        public string AccountId { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
