using System.Collections.Generic;
using Forum_Web_API.ViewModels.PostViewModel;

namespace Forum_Web_API.ViewModels.SearchViewModel
{
    public class SearchResultViewModel
    {
        public IEnumerable<PostListViewModel> Posts { get; set; }
        public string SearchQuery { get; set; }
        public bool EmptySearchResults { get; set; }
    }
}