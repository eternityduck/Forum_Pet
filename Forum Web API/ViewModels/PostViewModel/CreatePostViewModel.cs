using System;

namespace Forum_Web_API.ViewModels.PostViewModel
{
    public class CreatePostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TopicName { get; set; }
        public int TopicId { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }
        public string AuthorEmail { get; set; }
    }
}