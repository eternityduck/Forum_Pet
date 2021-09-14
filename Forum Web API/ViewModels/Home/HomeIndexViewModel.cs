using System.Collections.Generic;
using Forum.ViewModels.PostViewModel;

namespace Forum.ViewModels.Home
{
    public class HomeIndexViewModel
    {
        public IEnumerable<PostListViewModel> LatestPosts { get; set; }
        public string SearchQuery { get; set; }
    }
}