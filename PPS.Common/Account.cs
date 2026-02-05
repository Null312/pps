using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPS.Common
{
    public class Account
    {
        public string AccountId { get; set; } = string.Empty;
        public Guid UserId { get; set; } 
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = "ACTIVE";
        public DateTime CreatedAt { get; set; }
    }
}
