using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Interfaces;
using DAL.Models;
using Forum.ViewModels.CommentViewModel;
using Forum.ViewModels.PostViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Forum.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _service;
        private readonly UserManager<User> _userManager;
        private readonly ITopicService _topicService;
        public PostController(IPostService service, UserManager<User> userManager, ITopicService topicService)
        {
            _topicService = topicService;
            (_service, _userManager) = (service, userManager);
        }


        public async Task<IActionResult> Index(int id)
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
            return View(model);
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


        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([FromForm] Post postModel)
        // {
        //     var user = await _userManager.GetUserAsync(HttpContext.User);
        //     postModel.Author = user;
        //     await _service.AddAsync(postModel);
        //     return RedirectToAction(nameof(Index));
        // }
        // [Authorize]
        // public IActionResult Create()
        // {
        //     return View();
        // }

       

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _service.GetByIdAsync(id));
        }
        
        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Title, Text, Author, CreatedAt")] Post post)
        {
            await _service.UpdateAsync(post);
            return RedirectToAction(nameof(Index));
        }
       
        public async Task<IActionResult> Create(int id)
        {
           
            var forum = await _topicService.GetByIdAsync(id);

            var model = new CreatePostViewModel()
            {
                TopicName = forum.Title,
                TopicId = forum.Id,
                AuthorName = User.Identity.Name,
                
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(CreatePostViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var post = BuildPost(model, user);

            await _service.AddAsync(post);
            return RedirectToAction("Index", "Topic", model.TopicId);
        }

        public Post BuildPost(CreatePostViewModel post, User user)
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
