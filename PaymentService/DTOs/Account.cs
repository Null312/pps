using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.DTOs
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        public string AccountNumber { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "USD";

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DailyLimit { get; set; } = 10000;

        public bool IsBlocked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public ICollection<PaymentTransaction> TransactionsFrom { get; set; } = new List<PaymentTransaction>();
        public ICollection<PaymentTransaction> TransactionsTo { get; set; } = new List<PaymentTransaction>();
    }
}
