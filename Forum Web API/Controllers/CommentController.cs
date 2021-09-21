using System;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using Forum_Web_API.ViewModels.CommentViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum_Web_API.Controllers
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

        [HttpGet]
        public async Task<CommentIndexViewModel> Index(int id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            
            var model = new CommentIndexViewModel()
            {
                Id = comment.Id,
                Content = comment.Text,
                AuthorId = comment.Author.Id,
                AuthorEmail = comment.Author.Name,
                CreatedAt = comment.CreatedAt,
            };
            return model;
        }
        [HttpPost]
        [Route("~/Comment")]
        [Authorize]
        //Type the email not the name
        public async Task<IActionResult> AddComment(CommentIndexViewModel commentIndexModel)
        {
            var user = await _userManager.FindByEmailAsync(commentIndexModel.AuthorEmail);
            var comment = CommentCreate(commentIndexModel, user);

            await _postService.AddCommentAsync(comment);

            return CreatedAtAction(nameof(Index),  new { id = comment.Id }, comment);
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
        
        [HttpDelete("/Delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var comment = await _commentService.GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            await _commentService.DeleteByIdAsync(id);
            return Ok();
        }


        

        [HttpPut]
        [Route("/Edit/{id}")]
        [Authorize]
        public async Task<ActionResult<Comment>> EditAsync(string userEmail, int id, string message)
        {
            //var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByEmailAsync(userEmail);
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null || user.Id != comment.Author.Id) return BadRequest("The comment is null or you are not the owner of comment");
            await _commentService.UpdateContentAsync(id, message);
            return Ok();
        }
    }
}