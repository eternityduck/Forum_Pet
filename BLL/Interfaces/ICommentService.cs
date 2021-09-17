using System.Threading.Tasks;
using BLL.Models;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> GetByIdAsync(int id);
        Task DeleteByIdAsync(int modelId);
         Task UpdateAsync(int id, string message);
         Comment GetById(int id);
    }
}