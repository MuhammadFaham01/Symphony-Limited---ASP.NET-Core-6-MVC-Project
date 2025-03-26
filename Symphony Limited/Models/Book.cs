using System;
using System.Collections.Generic;

namespace Symphony_Limited.Models
{
    public partial class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Isbn { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
