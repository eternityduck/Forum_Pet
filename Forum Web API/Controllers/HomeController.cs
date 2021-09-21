using System.Linq;
using System.Threading.Tasks;
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
        public async Task<HomeIndexViewModel> Index()
        {
            var model = await BuildHome();
            return model;
        }

        private async Task<HomeIndexViewModel> BuildHome()
        {
            
            var latest = await _postService.GetLatestPosts(10);

            var posts = latest.Select(x => new PostListViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Author = x.Author.Name,
                AuthorId = x.Author.Id,
                DatePosted = x.CreatedAt.ToString(),
                RepliesCount = _postService.GetCommentsCount(x.Id),
            });
            
            return new HomeIndexViewModel()
            {
                LatestPosts = posts
            };
        }
    }
}