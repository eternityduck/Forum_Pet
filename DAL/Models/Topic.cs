using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public ICollection<Post> Posts { get; set; }
    }
}