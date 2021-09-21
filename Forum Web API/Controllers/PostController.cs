using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using Forum_Web_API.ViewModels.CommentViewModel;
using Forum_Web_API.ViewModels.PostViewModel;
using Forum_Web_API.ViewModels.TopicViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum_Web_API.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _service;
        private readonly UserManager<User> _userManager;
        private readonly ITopicService _topicService;
        public PostController(IPostService service, UserManager<User> userManager, ITopicService topicService)
        {
            _topicService = topicService;
            (_service, _userManager) = (service, userManager);
        }
        
        [HttpGet]
        public async Task<PostIndexPostViewModel> Index(int id)
        {
            var post = await _service.GetByIdAsync(id);
            var comments = GetComments(post).OrderBy(x => x.CreatedAt);
            var model = new PostIndexPostViewModel()
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text,
                AuthorId = post.Author.Id,
                AuthorName = post.Author.Name,
                CreatedAt = post.CreatedAt,
                Comments = comments,
                TopicId = post.Topic.Id,
                TopicName = post.Topic.Title
            };
            return model;
        }
        private IEnumerable<CommentIndexViewModel> GetComments(Post post)
        {
            return post.Comments.Select(c => new CommentIndexViewModel()
            {
                Id = c.Id,
                AuthorEmail = c.Author.Name,
                AuthorId = c.Author.Id,
                CreatedAt = c.CreatedAt,
                Content = c.Text
            });
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(int id)
        {
            await _service.DeleteByIdAsync(id);
            return NoContent();
        }
        
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(string userEmail, int id, string content)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            var comment = await _service.GetByIdAsync(id);
            if (comment == null || user.Id != comment.Author.Id) return BadRequest("The comment is null or you are not the owner of comment");
            await _service.UpdateContentAsync(id, content);
            return Ok();
        }
        
       
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost(CreatePostViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.AuthorEmail);
            var post = BuildPost(model, user);

            await _service.AddAsync(post);
            return CreatedAtAction(nameof(Index), new { id = model.Id }, model);
        }
       
        private Post BuildPost(CreatePostViewModel post, User user)
        {
       
            var topic = _topicService.GetByIdAsync(post.TopicId);

            return new Post
            {
                Title = post.Title,
                Text = post.Content,
                CreatedAt = DateTime.Now,
                Topic = topic.Result,
                Author = user,
            };
        }

        [HttpGet("/PostsByUser")]
        public async Task<IEnumerable<PostListViewModel>> GetPostsByUser(string userEmail)
        {
            var posts = await _service.GetPostsByUserEmail(userEmail);
            var postListings = posts.Select(post => new PostListViewModel()
            {
                Id = post.Id,
                Topic = BuildPostListing(post).Result,
                Author = post.Author.Name,
                AuthorId = post.Author.Id,
                Title = post.Title,
                DatePosted = post.CreatedAt.ToString(CultureInfo.InvariantCulture),
                RepliesCount = post.Comments.Count
            });
            return postListings;
        }
        [HttpGet("/PostsByTopic")]
        public async Task<IEnumerable<PostListViewModel>> GetPostsByTopic(int id)
        {
            var posts = await _service.GetPostsByTopicId(id);
            var postListings = posts.Select(post => new PostListViewModel()
            {
                Id = post.Id,
                Topic = BuildPostListing(post).Result,
                Author = post.Author.Name,
                AuthorId = post.Author.Id,
                Title = post.Title,
                DatePosted = post.CreatedAt.ToString(CultureInfo.InvariantCulture),
                RepliesCount = post.Comments.Count
            });
            return postListings;
        }
        private async Task<TopicListViewModel> BuildTopicList(Topic topic)
        {
            var recentPosts = await _topicService.GetRecentPostsAsync(topic.Id, 10);
            return new TopicListViewModel
            {
                Id = topic.Id,
                Name = topic.Title,
                Description = topic.Description,
                NumberOfPosts = topic.Posts.Count,
                NumberOfUsers = _topicService.GetUsers(topic.Id).Count(),
                HasRecentPost =  recentPosts.Any(),
            };
        }

        private async Task<TopicListViewModel> BuildPostListing(Post post)
        {
            var topic = post.Topic;
            return await BuildTopicList(topic);
        }
    }
    }
