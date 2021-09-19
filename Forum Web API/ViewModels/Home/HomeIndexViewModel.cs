using System.Collections.Generic;
using Forum_Web_API.ViewModels.PostViewModel;

namespace Forum_Web_API.ViewModels.Home
{
    public class HomeIndexViewModel
    {
        public IEnumerable<PostListViewModel> LatestPosts { get; set; }
        public string SearchQuery { get; set; }
    }
}