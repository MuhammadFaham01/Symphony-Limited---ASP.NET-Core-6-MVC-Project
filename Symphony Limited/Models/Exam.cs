using System;
using System.Collections.Generic;

namespace Symphony_Limited.Models
{
    public partial class Exam
    {
        public Exam()
        {
            ExamResults = new HashSet<ExamResult>();
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public int CourseId { get; set; }
        public DateTime? ExamDate { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<ExamResult> ExamResults { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
