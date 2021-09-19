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
    public class PostService : IPostService
    {
        private readonly ForumContext _context;
        private readonly Mapper _mapper;

        public PostService( ForumContext context)
        {
            _context = context;
        }
        

        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }
        
        public async Task EditPostContent(int id, string content)
        {
            var post = await GetByIdAsync(id);
            post.Text = content;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public int GetCommentsCount(int id)
        {
            return GetById(id).Comments.Count;
        }

        public IEnumerable<Post> GetPostsByUserId(int id)
        {
            return _context.Posts.Where(x => x.Author.Id == id.ToString());
        }

        public IEnumerable<Post> GetPostsByTopicId(int id)
        {
            return _context.Topics.Include(x => x.Posts).First(x => x.Id == id).Posts;
        }
        

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts
                .Include(x => x.Topic).Include(x => x.Author)
                .Include(x => x.Comments).ThenInclude(x => x.Author)
                .ToListAsync();
        }

        public IEnumerable<Post> GetAll()
        {
            return _context.Posts
                .Include(x => x.Author)
                .Include(x => x.Comments).ThenInclude(x => x.Author).Include(x => x.Topic);
        }
        public async Task<Post> GetByIdAsync(int id)
        {
            return await _context.Posts.Where(x => x.Id == id)
                .Include(x => x.Author)
                .Include(post => post.Comments).ThenInclude(x => x.Author)
                .Include(post => post.Topic).FirstOrDefaultAsync();
        }
        public Post GetById(int id)
        {
            return _context.Posts.Where(post=>post.Id == id)
                .Include(post=>post.Author)
                .Include(post=>post.Comments).ThenInclude(reply => reply.Author)
                .Include(post=>post.Topic)
                .First();
        }
        public IEnumerable<Post> GetLatestPosts(int count)
        {
            return GetAll().OrderByDescending(p => p.CreatedAt).Take(count).ToList();
        }
        public IEnumerable<User> GetAllUsers(IEnumerable<Post> posts)
        {
            var users = new List<User>();

            foreach(var post in posts)
            {
                users.Add(post.Author);

                if (post.Comments == null) continue;

                users.AddRange(post.Comments.Select(reply => reply.Author));
            }

            return users.Distinct();
        }

        

        public async Task AddAsync(Post model)
        {
            await _context.Posts.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post model)
        {
            _context.Entry(model).State = EntityState.Modified;
            _context.Posts.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int modelId)
        {
            var model = await _context.Posts.Include(x => x.Comments).SingleOrDefaultAsync(x=>x.Id == modelId);
            _context.Remove(model);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Post> GetFilteredPosts(string searchQuery)
        {
            var query = searchQuery.ToLower();

            return _context.Posts
                .Include(post => post.Topic)
                .Include(post => post.Author)
                .Include(post => post.Comments)
                .Where(post => 
                    post.Title.ToLower().Contains(query) 
                    || post.Text.ToLower().Contains(query));
        }
    }
}