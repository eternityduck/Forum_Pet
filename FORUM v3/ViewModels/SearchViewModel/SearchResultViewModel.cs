using System.Collections.Generic;
using Forum.ViewModels.PostViewModel;

namespace Forum.ViewModels.SearchViewModel
{
    public class SearchResultViewModel
    {
        public IEnumerable<PostListViewModel> Posts { get; set; }
        public string SearchQuery { get; set; }
        public bool EmptySearchResults { get; set; }
    }
}