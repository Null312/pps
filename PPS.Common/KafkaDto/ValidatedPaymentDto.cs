namespace PPS.Common.KafkaDto
{
    public class ValidatedPaymentDto
    {
        public string Payment_id { get; set; }
        public string From_account_id { get; set; }
        public string To_account_id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string From_user_id { get; set; }
        public decimal From_account_balance { get; set; }
        public int From_account_age_days { get; set; }
        

    }
}
