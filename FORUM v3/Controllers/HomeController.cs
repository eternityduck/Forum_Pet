using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using Forum.ViewModels;
using Forum.ViewModels.Home;
using Forum.ViewModels.PostViewModel;
using Forum.ViewModels.TopicViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Forum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        public HomeController(ILogger<HomeController> logger, IPostService postService)
        {
            _logger = logger;
            _postService = postService;
        }

        public IActionResult Index()
        {
            var model = BuildHome();
            return View(model);
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
                //Topic = BuildTopicList(x)
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

        [HttpPost]
        public IActionResult Search(string searchQuery)
        {
            return RedirectToAction("Topic", "Topic", new { searchQuery });
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}