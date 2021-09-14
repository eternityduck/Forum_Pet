using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Models;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface ITopicService : ICrud<Topic>
    {
        Task UpdateTopicTitle(int id, string title);
        Task UpdateTopicDescription(int id, string desc);
       
        IEnumerable<Post> GetFilteredPosts(string searchQuery);
        IEnumerable<Post> GetFilteredPosts(int forumId, string modelSearchQuery);
        IEnumerable<User> GetUsers(int forumId);
    }
}