using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Forum_Web_API.ViewModels.UserViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum_Web_API.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public ActionResult<List<User>> Index() => _userManager.Users.ToList();
        

        [HttpPost("/Add")]
        public async Task<ActionResult<CreateUserViewModel>> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Incorrect data");
            
            User user = new User {Email = model.Email, UserName = model.Email, Name = model.Name };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return model;
        }
        [HttpGet("/User/{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }
        
        [HttpPut("/Edit")]
        public async Task<ActionResult<EditUserViewModel>> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return model;
            User user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return model;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.Name = model.Name;


            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("Successfully edited user");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return model;
        }

        [HttpDelete]
        public async Task<ActionResult<ChangePasswordViewModel>> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        
            return Ok("Successfully deleted");
        }
        
        
    }
}