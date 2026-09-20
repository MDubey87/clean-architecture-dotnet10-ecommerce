using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IMongoCollection<ProductBrand> _brandsCollection;
        public BrandRepository(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient client)
        {
            var settings = mongoDBSettings.Value;
            var database = client.GetDatabase(settings.DatabaseName);
            _brandsCollection = database.GetCollection<ProductBrand>(settings.BrandCollectionName);
        }
        public async Task<IEnumerable<ProductBrand>> GetAllBrandsAsync()
        {
            return await _brandsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<ProductBrand> GetBrandByIdAsync(string id)
        {
            return await _brandsCollection.Find(brand => brand.Id == id).FirstOrDefaultAsync();
        }
    }
}
