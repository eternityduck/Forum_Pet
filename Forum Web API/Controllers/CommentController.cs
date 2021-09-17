﻿using System;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using Forum.ViewModels.CommentViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Forum.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IPostService _postService;
        private readonly ITopicService _topicService;
        private readonly UserManager<User> _userManager;

        public CommentController(ICommentService service, UserManager<User> userManager, IPostService postService,
            ITopicService topicService)
        {
            _postService = postService;
            _topicService = topicService;
            (_commentService, _userManager) = (service, userManager);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("~/Comment")]
        [Authorize]
        public async Task<IActionResult> AddComment(CommentIndexViewModel commentIndexModel)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var comment = CommentCreate(commentIndexModel, user);

            await _postService.AddCommentAsync(comment);

            return RedirectToAction("Index", "Post", new {id = commentIndexModel.PostId});
        }

        private Comment CommentCreate(CommentIndexViewModel model, User user)
        {
            var post = _postService.GetById(model.PostId);
            return new Comment()
            {
                Post = post,
                Text = model.Content,
                CreatedAt = DateTime.Now,
                Author = user
            };
        }

        // [HttpGet("/Comment/Create/{id}")]
        // [Authorize]
        // public async Task<CommentIndexViewModel> Create(int id)
        // {
        //     var post = _postService.GetById(id);
        //     var topic = await _topicService.GetByIdAsync(post.Topic.Id);
        //     var user = await _userManager.FindByNameAsync(User.Identity.Name);
        //          
        //     var model = new CommentIndexViewModel()
        //     {
        //         PostContent = post.Text,
        //         PostTitle = post.Title,
        //         PostId = post.Id,
        //
        //         TopicName = topic.Title,
        //         TopicId = topic.Id,
        //
        //         AuthorName = User.Identity.Name,
        //        
        //         AuthorId = user.Id,
        //         
        //         CreatedAt = DateTime.Now
        //     };
        //
        //     return model;
        // }
        [HttpDelete("/Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var post = await _commentService.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            await _commentService.DeleteByIdAsync(id);
            return Ok();
        }


        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin")]
        // [Route("Comment/Delete/{id}")]
        // public async Task<ActionResult> DeleteConfirmed(int id)
        // {
        //     await _commentService.DeleteByIdAsync(id);
        //     return Ok();
        // }

        [HttpPut]
        [Route("/Edit/{id}")]
        [Authorize]
        public async Task<ActionResult<Comment>> EditAsync(int id, string message)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var comment = await _commentService.GetByIdAsync(id);
            if (comment != null || user.Id != comment.Author.Id) return BadRequest();
            await _commentService.UpdateAsync(id, message);
            return Ok();
        }
    }
}