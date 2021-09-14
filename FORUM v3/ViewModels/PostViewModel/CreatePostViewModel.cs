using System;

namespace Forum.ViewModels.PostViewModel
{
    public class CreatePostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TopicName { get; set; }
        public string TopicImageUrl { get; set; }
        public int TopicId { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }
        public string AuthorName { get; set; }
    }
}