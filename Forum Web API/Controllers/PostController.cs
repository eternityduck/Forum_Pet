using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using Forum_Web_API.ViewModels.CommentViewModel;
using Forum_Web_API.ViewModels.PostViewModel;
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
                AuthorName = c.Author.Name,
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
        public async Task<IActionResult> Edit(int id, string content)
        {
            await _service.EditPostContent(id, content);
            return Ok();
        }
        
       
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost(CreatePostViewModel model)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(model.AuthorName);
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
        
    }
    }
