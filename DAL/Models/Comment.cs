using System;

namespace DAL.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public Post Post { get; set; }
        public User Author { get; set; }
    }
}