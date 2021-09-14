using System;
using System.Collections.Generic;
using Forum.ViewModels.CommentViewModel;

namespace Forum.ViewModels.PostViewModel
{
    public class PostIndexPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImageUrl { get; set; }
        public DateTime CreatedAt{ get; set; }
        public int TopicId { get; set; }
        public string TopicName { get; set; }
        public IEnumerable<CommentIndexViewModel> Comments { get; set; }
    }
}