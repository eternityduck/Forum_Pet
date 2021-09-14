using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using Forum.ViewModels.PostViewModel;
using Forum.ViewModels.TopicViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }
        
        [HttpGet]
        [Route("~/Topic")]
        public TopicIndexViewModel Index()
        {
            var forums = _topicService.GetAllAsync().Result.Select(topic => new TopicListViewModel
            {
                Id = topic.Id,
                Name = topic.Title,
                Description = topic.Description,
                NumberOfPosts = topic.Posts?.Count() ?? 0,
                NumberOfUsers = _topicService.GetUsers(topic.Id).Count(),
            });

            var forumListingModels = forums as IList<TopicListViewModel> ?? forums.ToList();

            var model = new TopicIndexViewModel()
            {
                TopicList = forumListingModels.OrderBy(forum=>forum.Name),
                NumberOfTopics = forumListingModels.Count()
            };

            return model;
        }
        [HttpPost]
        [Route("~/Topic/Search")]
        public IActionResult Search(int id, string searchQuery)
        {
            return RedirectToAction("Topic", new {id, searchQuery});
        }
        [Route("~/Topic/{id}")]
        public async Task<TopicResultViewModel> Topic(int id, string searchQuery)
        {
            var topic =  await _topicService.GetByIdAsync(id);
            var posts = _topicService.GetFilteredPosts(id, searchQuery).ToList();
            var noResults = (!string.IsNullOrEmpty(searchQuery) && !posts.Any());

            var postListings = posts.Select(post => new PostListViewModel()
            {
                Id = post.Id,
                Topic = BuildPostListing(post),
                Author = post.Author.Name,
                AuthorId = post.Author.Id,
                Title = post.Title,
                DatePosted = post.CreatedAt.ToString(CultureInfo.InvariantCulture),
                RepliesCount = post.Comments.Count()
            }).OrderByDescending(post=>post.DatePosted);

            var model = new TopicResultViewModel()
            {
                EmptySearchResults = noResults,
                Posts = postListings,
                SearchQuery = searchQuery,
                Topic = BuildTopicList(topic)
            };

            return model;
        }
        private static TopicListViewModel BuildTopicList(Topic topic)
        {
            return new TopicListViewModel
            {
                Id = topic.Id,
                Name = topic.Title,
                Description = topic.Description,
            };
        }

        private static TopicListViewModel BuildPostListing(Post post)
        {
            var topic = post.Topic;
            return BuildTopicList(topic);
        }

      

    }
}