using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using DAL;
using DAL.Models;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ForumContext _context;
        private readonly Mapper _mapper;

        public UserService( Mapper mapper, ForumContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        
    }
}