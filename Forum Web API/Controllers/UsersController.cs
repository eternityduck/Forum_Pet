using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Forum.ViewModels.UserViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public List<User> Index() => _userManager.Users.ToList();
        
        // [HttpGet("/Create")]
        // public IActionResult Create() => Ok();

        [HttpPost("/Add")]
        public async Task<ActionResult<CreateUserViewModel>> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User {Email = model.Email, UserName = model.Email, Name = model.Name };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return model;
        }
        [HttpGet("/Edit/{id}")]
        public async Task<ActionResult<EditUserViewModel>> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
        
            EditUserViewModel model = new EditUserViewModel
                {Id = user.Id, Email = user.Email, UserName = user.Email};
            return model;
        }

        [HttpPut("/Edit")]
        public async Task<ActionResult<EditUserViewModel>> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Name = model.UserName;


                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
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
        
            return RedirectToAction("Index");
        }
        // [HttpGet]
        // public async Task<ActionResult<ChangePasswordViewModel>> ChangePassword(string id)
        // {
        //     User user = await _userManager.FindByIdAsync(id);
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     ChangePasswordViewModel model = new ChangePasswordViewModel {Id = user.Id, Email = user.Email};
        //     return model;
        // }
        
        [HttpPost("/ChangePassword")]
        public async Task<ActionResult<ChangePasswordViewModel>> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
        
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "user not found");
                }
            }
        
            return model;
        }
    }
}