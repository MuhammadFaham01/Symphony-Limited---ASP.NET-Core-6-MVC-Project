using System;
using System.Collections.Generic;

namespace Symphony_Limited.Models
{
    public partial class UsersTbl
    {
        public UsersTbl()
        {
            ExamResults = new HashSet<ExamResult>();
            PremiumAccesses = new HashSet<PremiumAccess>();
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<ExamResult> ExamResults { get; set; }
        public virtual ICollection<PremiumAccess> PremiumAccesses { get; set; }
    }
}
