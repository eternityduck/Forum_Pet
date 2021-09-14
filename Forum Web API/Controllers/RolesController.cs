using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Forum.ViewModels.UserViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<User> _userManager;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet]
        public List<IdentityRole> Index() => _roleManager.Roles.ToList();
 
        //public ActionResult Create() => View();
        [HttpPost]
        public async Task<ActionResult<string>> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
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
            return name;
        }
         
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
               await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }
        [HttpGet("/Roles/Users")]
        public ActionResult<List<User>> UserList() => _userManager.Users.ToList();
        [HttpGet("{userId}")]
 
        public async Task<ActionResult<ChangeRoleViewModel>> Edit(string userId)
        {
            
            User user = await _userManager.FindByIdAsync(userId);
            if(user!=null)
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
        [HttpPut("/User/{userId}/{roles?}")]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
         
            User user = await _userManager.FindByIdAsync(userId);
            if(user!=null)
            {
                
                var userRoles = await _userManager.GetRolesAsync(user);
                
                var allRoles = _roleManager.Roles.ToList();
                
                var addedRoles = roles.Except(userRoles);
                
                var removedRoles = userRoles.Except(roles);
 
                await _userManager.AddToRolesAsync(user, addedRoles);
 
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
 
                return RedirectToAction("UserList");
            }
 
            return NotFound();
        }
    }
}