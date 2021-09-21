using System;

namespace Forum_Web_API.ViewModels.CommentViewModel
{
    public class CommentIndexViewModel
    {
        public int Id { get; set; }

        public string AuthorId { get; set; }
        public string AuthorEmail { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }

        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }

        public string TopicName { get; set; }
        public int TopicId { get; set; }
    }
}