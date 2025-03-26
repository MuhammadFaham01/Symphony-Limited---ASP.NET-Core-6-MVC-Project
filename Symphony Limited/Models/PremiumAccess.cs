using System;
using System.Collections.Generic;

namespace Symphony_Limited.Models
{
    public partial class PremiumAccess
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TransactionId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }

        public virtual UsersTbl User { get; set; } = null!;
    }
}
