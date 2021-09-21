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
        //private readonly Mapper _mapper;
        private readonly ForumContext _context;
        public CommentService(ForumContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _context.Comments.Include(x => x.Post).ThenInclude(x => x.Topic)
                .Include(x => x.Post).ThenInclude(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Comment model)
        {
            await _context.Comments.AddAsync(model);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteByIdAsync(int modelId)
        {
            try
            {
                _context.Remove(await GetByIdAsync(modelId));
                await _context.SaveChangesAsync();
            }
            catch (ForumException)
            {
                throw new ForumException("Invalid id");
            }
        }

        public async Task UpdateContentAsync(int id, string message)
        {
            try
            {
                var comment = await GetByIdAsync(id);
                comment.Text = message;
                _context.Update(comment);
                await _context.SaveChangesAsync();
            }
            catch(ForumException)
            {
                throw new ForumException("Invalid id");
            }
        }

        // public Comment GetById(int id)
        // {
        //     return _context.Comments.Include(x => x.Post).ThenInclude(x => x.Topic)
        //         .Include(x => x.Post).ThenInclude(x => x.Author).FirstOrDefault(x => x.Id == id);
        // }
    }
}