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
        
        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public Task AddAsync(User model)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(User model)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteByIdAsync(int modelId)
        {
            throw new System.NotImplementedException();
        }
    }
}