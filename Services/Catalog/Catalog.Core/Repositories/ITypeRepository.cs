using Catalog.Core.Entities;

namespace Catalog.Core.Repositories
{
    public interface ITypeRepository
    {
        Task<IEnumerable<ProductType>> GetAllTypesAsync();
        Task<ProductType> GetTypeByIdAsync(string id);
    }
}
