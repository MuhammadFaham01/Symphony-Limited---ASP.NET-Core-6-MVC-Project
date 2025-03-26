using System;
using System.Collections.Generic;

namespace Symphony_Limited.Models
{
    public partial class Course
    {
        public Course()
        {
            Exams = new HashSet<Exam>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
        public string Type { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Exam> Exams { get; set; }
    }
}
