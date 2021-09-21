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
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public List<IdentityRole> Index() => _roleManager.Roles.ToList();

        [HttpPost]
        public async Task<ActionResult<string>> Create(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return name;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role == null) return BadRequest("Incorrect Id");
            await _roleManager.DeleteAsync(role);
            return Ok("Successfully deleted");

        }

        [HttpGet("/Roles/Users")]
        public ActionResult<List<User>> UserList() => _userManager.Users.ToList();

        [HttpGet("{userId}")]
        public async Task<ActionResult<ChangeRoleViewModel>> Edit(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return model;
            }

            return NotFound();
        }

        [HttpPut("/User/{userId}")]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                

                var addedRoles = roles.Except(userRoles);

                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return Ok($"Successfully updated the role list of {user.Name}");
            }

            return NotFound();
        }
    }
}