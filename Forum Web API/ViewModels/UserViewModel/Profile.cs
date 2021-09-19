using System;

namespace Forum_Web_API.ViewModels.UserViewModel
{
    public class Profile
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime AccountCreatedAt { get; set; }
        
    }
}