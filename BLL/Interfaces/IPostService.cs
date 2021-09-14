using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Models;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface IPostService : ICrud<Post>
    {
        Task AddCommentAsync(Comment comment);
        Task EditPostContent(int id, string content);
        int GetCommentsCount(int id);
        IEnumerable<Post> GetPostsByUserId(int id);
        IEnumerable<Post> GetPostsByTopicId(int id);
        IEnumerable<Post> GetFilteredPosts(string searchQuery);
        Post GetById(int id);
        IEnumerable<User> GetAllUsers(IEnumerable<Post> posts);
        IEnumerable<Post> GetLatestPosts(int count);
        IEnumerable<Post> GetAll();
    }
}