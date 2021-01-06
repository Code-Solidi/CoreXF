using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miscelaneous
{
    public interface IManager<TModel, TDto>
    {
        Task<IEnumerable<TModel>> GetAllAsync();

        Task<TModel> GetByIdAsync(Guid id);

        Task<TModel> Insert(TDto value);

        Task<TModel> UpdateAsync(Guid id, TDto value);

        Task<TModel> DeleteAsync(Guid id);
    }
}