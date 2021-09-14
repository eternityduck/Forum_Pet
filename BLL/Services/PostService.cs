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

        public PostService(Mapper mapper, ForumContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // public async Task<IEnumerable<PostModel>> GetAllAsync()
        // {
        //     var result = _mapper.Map<IEnumerable<Post>, List<PostModel>>(await _unitOfWork.Posts.GetAllAsync());
        //     
        //     return result;
        // }
        //
        // Task<Post> ICrud<Post>.GetByIdAsync(int id)
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        
        //
        // public async Task<PostModel> GetByIdAsync(int id) =>
        //     _mapper.Map<PostModel>(await _unitOfWork.Posts.GetByIdAsync(id));
        //
        // public async Task AddAsync(PostModel model)
        // {
        //     if (model == null)
        //         throw new ForumException("Can not be null");
        //     if (string.IsNullOrEmpty(model.Text))
        //         throw new ForumException("The post can not be null");
        //     if (string.IsNullOrEmpty(model.Title))
        //         throw new ForumException("The title can not be empty");
        //
        //     var item = _mapper.Map<Post>(model);
        //     await _unitOfWork.Posts.AddAsync(item);
        //     await _unitOfWork.SaveAsync();
        // }
        //
        // public async Task UpdateAsync(PostModel model)
        // {
        //     if (model == null)
        //         throw new ForumException("Can not be null");
        //     if (string.IsNullOrEmpty(model.Text))
        //         throw new ForumException("The post can not be null");
        //     if (string.IsNullOrEmpty(model.Title))
        //         throw new ForumException("The title can not be empty");
        //     
        //     _unitOfWork.Posts.Update(_mapper.Map<Post>(model));
        //     await _unitOfWork.SaveAsync();
        // }
        //
        // public async Task DeleteByIdAsync(int modelId)
        // {
        //     await _mapper.Map<Task>(_unitOfWork.Posts.RemoveByIdAsync(modelId));
        //     await _unitOfWork.SaveAsync();
        // }

        // public async Task AddCommentAsync(Comment comment)
        // {
        //     await _unitOfWork.Comments.AddAsync(_mapper.Map<Comment>(comment));
        //     await _unitOfWork.SaveAsync();
        // }

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
            return _context.Topics.First(x => x.Id == id).Posts;
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

                if (!post.Comments.Any()) continue;

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
            _context.Posts.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int modelId)
        {
            _context.Posts.Remove(await GetByIdAsync(modelId));
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