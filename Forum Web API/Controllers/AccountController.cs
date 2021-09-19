﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Forum_Web_API.ViewModels;
using Forum_Web_API.ViewModels.UserViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Forum_Web_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        

        [HttpPost("/Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)  
        {  
            var userExists = await _userManager.FindByNameAsync(model.Email);  
            if (userExists != null)  
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });  
  
            User user = new User()  
            {  
                Email = model.Email,  
                SecurityStamp = Guid.NewGuid().ToString(),  
                UserName = model.Email,
                Name = model.Name,
                MemberSince = DateTime.Now
            };  
            var result = await _userManager.CreateAsync(user, model.Password);  
            if (!result.Succeeded)  
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
  
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });  
        }  
        // public async Task<ActionResult<RegisterViewModel>> Register(RegisterViewModel model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         User user = new User {Email = model.Email, UserName = model.Email, Name = model.Name};
        //
        //         var result = await _userManager.CreateAsync(user, model.Password);
        //         if (result.Succeeded)
        //         {
        //             await _signInManager.SignInAsync(user, false);
        //             return RedirectToAction("Index", "Home");
        //         }
        //
        //         foreach (var error in result.Errors)
        //         {
        //             ModelState.AddModelError(string.Empty, error.Description);
        //         }
        //     }
        //
        //     return model;
        // }
        

        [HttpPost("/Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)  
        {  
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password)) return Unauthorized();
            var userRoles = await _userManager.GetRolesAsync(user);  
  
            var authClaims = new List<Claim>  
            {  
                new Claim(ClaimTypes.Name, user.UserName),  
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  
            };
            
            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));  
  
            var token = new JwtSecurityToken(  
                issuer: _configuration["JWT:ValidIssuer"],  
                audience: _configuration["JWT:ValidAudience"],  
                expires: DateTime.Now.AddHours(3),  
                claims: authClaims,  
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)  
            );  
  
            return Ok(new  
            {  
                token = new JwtSecurityTokenHandler().WriteToken(token),  
                expiration = token.ValidTo  
            });
        }  
        //[ValidateAntiForgeryToken]
        // public async Task<ActionResult<LoginViewModel>> Login(LoginViewModel model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var result =
        //             await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
        //         if (result.Succeeded)
        //         {
        //             if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        //             {
        //                 return Redirect(model.ReturnUrl);
        //             }
        //
        //             return RedirectToAction("Index", "Home");
        //         }
        //
        //         ModelState.AddModelError("", "Incorrect login or password");
        //     }
        //
        //     return model;
        // }

        [HttpPost("/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
       
        [HttpGet("/Details")]
        [Authorize]
        public async Task<ActionResult<Profile>> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var model = new Profile
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                AccountCreatedAt = user.MemberSince,
            };
            return model;
        }
    }
}