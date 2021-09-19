using System.Linq;
using BLL.Interfaces;
using DAL.Models;
using Forum_Web_API.ViewModels.Home;
using Forum_Web_API.ViewModels.PostViewModel;
using Forum_Web_API.ViewModels.TopicViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Forum_Web_API.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        public HomeController(ILogger<HomeController> logger, IPostService postService)
        {
            _logger = logger;
            _postService = postService;
        }
        [HttpGet]
        public HomeIndexViewModel Index()
        {
            var model = BuildHome();
            return model;
        }

        private HomeIndexViewModel BuildHome()
        {
            
            var latest =  _postService.GetLatestPosts(10);

            var posts = latest.Select(x => new PostListViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Author = x.Author.Name,
                AuthorId = x.Author.Id,
                DatePosted = x.CreatedAt.ToString(),
                RepliesCount = _postService.GetCommentsCount(x.Id),
               // Topic = BuildTopicList(x),
            });
            
            return new HomeIndexViewModel()
            {
                LatestPosts = posts
            };
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

        // [HttpGet("/Search")]
        // public IActionResult Search(string searchQuery)
        // {
        //     return RedirectToAction("Topic", "Topic", new { searchQuery });
        // }

        // public IActionResult Privacy()
        // {
        //     return View();
        // }
    }
}