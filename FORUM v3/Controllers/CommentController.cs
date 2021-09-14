using System;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using Forum.ViewModels.CommentViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IPostService _postService;
        private readonly ITopicService _topicService;
        private readonly UserManager<User> _userManager;
        public CommentController(ICommentService service, UserManager<User> userManager, IPostService postService, ITopicService topicService)
        {
            _postService = postService;
            _topicService = topicService;
            (_commentService, _userManager) = (service, userManager);
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([FromForm] CommentIndexViewModel commentIndexModel)
        {
            
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var comment = CommentCreate(commentIndexModel, user);
        
            await _postService.AddCommentAsync(comment);
            
            return RedirectToAction("Index", "Post", new { id = commentIndexModel.PostId });
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
        [Authorize]
        public async Task<IActionResult> Create(int id)
        {
            var post = _postService.GetById(id);
            var topic = await _topicService.GetByIdAsync(post.Topic.Id);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
                 
            var model = new CommentIndexViewModel()
            {
                PostContent = post.Text,
                PostTitle = post.Title,
                PostId = post.Id,
        
                TopicName = topic.Title,
                TopicId = topic.Id,
        
                AuthorName = User.Identity.Name,
               
                AuthorId = user.Id,
                
                CreatedAt = DateTime.Now
            };
        
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {

            var post = await _commentService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        
       
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     await _commentService.DeleteByIdAsync(id);
        //     return RedirectToAction(nameof(Index));
        // }
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _commentService.GetByIdAsync(id));
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id, Title, Text, Author, CreatedAt")] Comment comment)
        {
            await _commentService.UpdateAsync(comment);
            return View(comment);
        }
       
    }
}