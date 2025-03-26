using System;
using System.Collections.Generic;

namespace Symphony_Limited.Models
{
    public partial class ExamResult
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; }
        public DateTime? ResultDate { get; set; }

        public virtual Exam Exam { get; set; } = null!;
        public virtual UsersTbl User { get; set; } = null!;
    }
}
