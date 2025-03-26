using System;
using System.Collections.Generic;

namespace Symphony_Limited.Models
{
    public partial class Question
    {
        public Question()
        {
            Options = new HashSet<Option>();
        }

        public int Id { get; set; }
        public int ExamId { get; set; }
        public string QuestionText { get; set; } = null!;

        public virtual Exam Exam { get; set; } = null!;
        public virtual ICollection<Option> Options { get; set; }
    }
}
