using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Models;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface IPostService : ICrud<Post>
    {
        Task AddCommentAsync(Comment comment);
        
        int GetCommentsCount(int id);
        Task<IEnumerable<Post>> GetPostsByUserEmail(string userEmail);
        Task<IEnumerable<Post>> GetPostsByTopicId(int id);
        IEnumerable<Post> GetFilteredPosts(string searchQuery);
        Post GetById(int id);
        IEnumerable<User> GetAllUsers(IEnumerable<Post> posts);
        Task<IEnumerable<Post>> GetLatestPosts(int count);
        
    }
}