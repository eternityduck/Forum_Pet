using System.Globalization;
using System.Linq;
using BLL.Interfaces;
using DAL.Models;
using Forum_Web_API.ViewModels.PostViewModel;
using Forum_Web_API.ViewModels.SearchViewModel;
using Forum_Web_API.ViewModels.TopicViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Forum_Web_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IPostService _postService;

        public SearchController(IPostService postService)
        {
            _postService = postService;
        }

        
        [HttpGet]
        public ActionResult<SearchResultViewModel> Results(string searchQuery)
        {
            var posts = _postService.GetFilteredPosts(searchQuery).ToList();
            var noResults = !string.IsNullOrEmpty(searchQuery) && !posts.Any();
            var postList = posts.Select(x => new PostListViewModel
            {
                Id = x.Id,
                AuthorId = x.Author.Id,
                Author = x.Author.Name,
                Title = x.Title,
                DatePosted = x.CreatedAt.ToString(CultureInfo.CurrentCulture),
                RepliesCount = x.Comments.Count,
                Topic = BuildTopicList(x)
            });
            var result = new SearchResultViewModel
            {
                Posts = postList,
                SearchQuery = searchQuery,
                EmptySearchResults = noResults
            };
            return result;
        }
        private TopicListViewModel BuildTopicList(Post post)
        {
            var topic = post.Topic;
            var topicList = new TopicListViewModel()
            {
                Name = topic.Title,
                Id = topic.Id,
                Description = topic.Description
            };
            return topicList;
        }
    }
}