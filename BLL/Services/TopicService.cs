using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class TopicService : ITopicService
    {
        private readonly ForumContext _context;
        private readonly Mapper _mapper;
        private readonly IPostService _postService;
        public TopicService(Mapper mapper, ForumContext context, IPostService postService)
        {
            _mapper = mapper;
            _context = context;
            _postService = postService;
        }
        
        
        public async Task<IEnumerable<Topic>> GetAllAsync()
        {
            return await _context.Topics.Include(x => x.Posts).ToListAsync();
        }

        public async Task<Topic> GetByIdAsync(int id)
        {
            return await _context.Topics.Where(x => x.Id == id).Include(t => t.Posts).ThenInclude(x => x.Author)
                .Include(t => t.Posts).ThenInclude(x => x.Comments).ThenInclude(x => x.Author).Include(x => x.Posts)
                .ThenInclude(x => x.Topic).FirstOrDefaultAsync();
        }

        public async Task AddAsync(Topic model)
        {
            await _context.Topics.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Topic model)
        { 
            _context.Topics.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int modelId)
        {
            _context.Topics.Remove(await GetByIdAsync(modelId));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTopicTitle(int id, string title)
        {
            var topic =await GetByIdAsync(id);
            topic.Title = title;
            
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTopicDescription(int id, string desc)
        {
            var topic =await GetByIdAsync(id);
            topic.Title = desc;
            
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
        }

        
        public Topic GetById(int id)
        {
            var topic = _context.Topics
                .Where(f => f.Id == id)
                .Include(f=>f.Posts)
                .ThenInclude(f=>f.Author)
                .Include(f=>f.Posts)
                .ThenInclude(f=>f.Comments)
                .ThenInclude(f=>f.Author)
                .Include(f=>f.Posts)
                .ThenInclude(p=>p.Topic)
                .FirstOrDefault();

            if(topic.Posts == null)
            {
                topic.Posts = new List<Post>();
            }

            return topic;
        }

        public IEnumerable<Post> GetFilteredPosts(string searchQuery)
        {
            return _postService.GetFilteredPosts(searchQuery);
        }

        public IEnumerable<Post> GetFilteredPosts(int topicId, string searchQuery)
        {
            if (topicId == 0) return _postService.GetFilteredPosts(searchQuery);

            var topic = GetById(topicId);

            return string.IsNullOrEmpty(searchQuery)
                ? topic.Posts
                : topic.Posts.Where(post
                    => post.Title.Contains(searchQuery) || post.Text.Contains(searchQuery));
        }

        public IEnumerable<User> GetUsers(int topicId)
        {
            var posts = GetById(topicId).Posts;

            if (posts is null || !posts.Any())
                return new List<User>();
            return _postService.GetAllUsers(posts);
        }
    }
}