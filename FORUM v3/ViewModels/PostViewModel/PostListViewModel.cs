using Forum.ViewModels.TopicViewModel;

namespace Forum.ViewModels.PostViewModel
{
    public class PostListViewModel
    {
        public TopicListViewModel Topic { get; set; }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
       
        public string AuthorId { get; set; }
        public string DatePosted { get; set; }

        public int TopicId { get; set; }
        public string TopicName { get; set; }

        public int RepliesCount { get; set; }
    }
}