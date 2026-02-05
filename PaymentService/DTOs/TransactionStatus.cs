namespace PaymentService.DTOs
{
    public static class TransactionStatus
    {
        public const string PendingValidation = "PENDING_VALIDATION";
        public const string PendingFraudCheck = "PENDING_FRAUD_CHECK";
        public const string Completed = "COMPLETED";
        public const string Failed = "FAILED";
        public const string Blocked = "BLOCKED";
    }
}
