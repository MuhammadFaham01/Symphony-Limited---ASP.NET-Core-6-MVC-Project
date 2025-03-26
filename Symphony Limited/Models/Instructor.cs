using System;
using System.Collections.Generic;

namespace Symphony_Limited.Models
{
    public partial class Instructor
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? ImagePath { get; set; }
    }
}
