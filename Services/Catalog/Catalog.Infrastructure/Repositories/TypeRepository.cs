using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class TypeRepository : ITypeRepository
    {
        private readonly IMongoCollection<ProductType> _typesCollection;
        public TypeRepository(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient client)
        {
            var settings = mongoDBSettings.Value;
            var database = client.GetDatabase(settings.DatabaseName);
            _typesCollection = database.GetCollection<ProductType>(settings.TypeCollectionName);
        }
        public async Task<IEnumerable<ProductType>> GetAllTypesAsync()
        {
            return await _typesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<ProductType> GetTypeByIdAsync(string id)
        {
            return await _typesCollection.Find(type => type.Id == id).FirstOrDefaultAsync();
        }
    }
}
