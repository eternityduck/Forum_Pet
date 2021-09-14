using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using BLL.Validation;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly Mapper _mapper;
        private readonly ForumContext _context;
        public CommentService( Mapper mapper, ForumContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        
        
        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _context.Comments.Include(x => x.Post).ThenInclude(x => x.Topic)
                .Include(x => x.Post).ThenInclude(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
        }
        
        public async Task UpdateAsync(Comment model)
        {
            _context.Comments.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int modelId)
        {
            _context.Remove(await GetByIdAsync(modelId));
            await _context.SaveChangesAsync();
        }

        public async Task Edit(int id, string message)
        {
            var comment = await GetByIdAsync(id);
            await _context.SaveChangesAsync();
            comment.Text = message;
            _context.Update(comment);
            await _context.SaveChangesAsync();
        }

        public Comment GetById(int id)
        {
            return _context.Comments.Include(x => x.Post).ThenInclude(x => x.Topic)
                .Include(x => x.Post).ThenInclude(x => x.Author).FirstOrDefault(x => x.Id == id);
        }
    }
}