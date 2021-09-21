using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICrud<TModel> where TModel : class
    {
        Task<IEnumerable<TModel>>  GetAllAsync();

        Task<TModel> GetByIdAsync(int id);

        Task AddAsync(TModel model);

        Task UpdateContentAsync(int id, string content);

        Task DeleteByIdAsync(int modelId);
    }

}