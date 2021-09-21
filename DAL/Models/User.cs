using System;
using System.Collections;
using Microsoft.AspNetCore.Identity;

namespace DAL.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public DateTime MemberSince { get; set; }
        
        
    }
}