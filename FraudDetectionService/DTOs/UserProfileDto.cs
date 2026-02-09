namespace FraudDetectionService.DTOs
{
    public class UserProfileDto
    {
        public string User_id { get; set; }
        public decimal Avg_transaction_amount_30d { get; set; }
        public int Transaction_count_24h { get; set; }
        public int Transaction_count_1h { get; set; }
        public List<string> Typical_recipients { get; set; }
        public List<int> Usual_activity_hours { get; set; }
        public int  account_age_days { get; set; }
        public DateTime last_updated { get; set; }
    }
}
