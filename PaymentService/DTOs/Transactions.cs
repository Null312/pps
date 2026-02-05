using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.DTOs
{
    public class PaymentTransaction
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = TransactionStatus.PendingValidation;

        public int? FraudRiskScore { get; set; }
        public string[]? FraudReasons { get; set; }
        public string? FailureReason { get; set; }
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(FromAccountId))]
        public Account? FromAccount { get; set; }

        [ForeignKey(nameof(ToAccountId))]
        public Account? ToAccount { get; set; }
    }
}
