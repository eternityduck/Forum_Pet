using System.Linq;
using BLL.Interfaces;
using DAL.Models;
using Forum.ViewModels.PostViewModel;
using Forum.ViewModels.SearchViewModel;
using Forum.ViewModels.TopicViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPostService _postService;

        public SearchController(IPostService postService)
        {
            _postService = postService;
        }

        // GET
        public IActionResult Results(string query)
        {
            var posts = _postService.GetFilteredPosts(query).ToList();
            var noResults = !string.IsNullOrEmpty(query) && !posts.Any();
            var postList = posts.Select(x => new PostListViewModel()
            {
                Id = x.Id,
                AuthorId = x.Author.Id,
                Author = x.Author.Name,
                Title = x.Title,
                DatePosted = x.CreatedAt.ToString(),
                RepliesCount = x.Comments.Count,
                Topic = BuildTopicList(x)
            });
            var result = new SearchResultViewModel
            {
                Posts = postList,
                SearchQuery = query,
                EmptySearchResults = noResults
            };
            return View(result);
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
        [HttpPost]
        public IActionResult Search(string query)
        {
            return RedirectToAction("Results", new {query});
        }
    }
}