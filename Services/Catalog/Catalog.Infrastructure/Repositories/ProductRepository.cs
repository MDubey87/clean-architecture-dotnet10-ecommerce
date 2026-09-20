using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specifications;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _productsCollection;
        private readonly IMongoCollection<ProductType> _typesCollection;
        private readonly IMongoCollection<ProductBrand> _brandsCollection;

        public ProductRepository(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient client)
        {
            var settings = mongoDBSettings.Value;
            var database = client.GetDatabase(settings.DatabaseName);
            _productsCollection = database.GetCollection<Product>(settings.ProductCollectionName);
            _brandsCollection = database.GetCollection<ProductBrand>(settings.BrandCollectionName);
            _typesCollection = database.GetCollection<ProductType>(settings.TypeCollectionName);
        }
        public async Task<Product> CreateProductAsync(Product product)
        {
            await _productsCollection.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            var deleteResult = await _productsCollection.DeleteOneAsync(product => product.Id == productId);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            return await _productsCollection.Find(product => product.Id == productId).FirstOrDefaultAsync();
        }

        public async Task<Pagination<Product>> GetProductsAsync(CatalogSpecParams catalogSpecParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;
            if(!string.IsNullOrEmpty(catalogSpecParams.Search))
            {
                filter &= builder.Regex(product => product.Name, new BsonRegularExpression($".*{catalogSpecParams.Search}*", "i"));
            }
            if (!string.IsNullOrEmpty(catalogSpecParams.BrandId))
            {
                filter &= builder.Eq(product => product.Brand.Id, catalogSpecParams.BrandId);
            }
            if (!string.IsNullOrEmpty(catalogSpecParams.TypeId))
            {
                filter &= builder.Eq(product => product.Type.Id, catalogSpecParams.TypeId);
            }
            var totalItems = await _productsCollection.CountDocumentsAsync(filter);
            var data = await ApplyDataFilterAsync(catalogSpecParams, filter);
            return new Pagination<Product>(
                catalogSpecParams.PageIndex,
                catalogSpecParams.PageSize,
                (int)totalItems,
                data.ToList()
            );
        }
        

        public async Task<IEnumerable<Product>> GetProductsByBrandAsync(string brandName)
        {
            return await _productsCollection.Find(product => product.Brand.Name.ToLower() == brandName.ToLower()).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            var filter = Builders<Product>.Filter.Regex(product => product.Name, new BsonRegularExpression($".*{name}*", "i"));
            return await _productsCollection.Find(filter).ToListAsync();
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var updateResult = await _productsCollection.ReplaceOneAsync(p => p.Id == product.Id, product);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<ProductBrand> GetBrandByIdAsync(string brandId)
        {
            return await _brandsCollection.Find(brand => brand.Id == brandId).FirstOrDefaultAsync();
        }

        public async Task<ProductType> GetTypeByIdAsync(string typeId)
        {
            return await _typesCollection.Find(type => type.Id == typeId).FirstOrDefaultAsync();
        }

        #region private methods
        private async Task<IReadOnlyCollection<Product>> ApplyDataFilterAsync(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
        {
            var sortDefinition = Builders<Product>.Sort.Ascending(product => product.Name);
            if (!string.IsNullOrEmpty(catalogSpecParams.Sort))
            {
                sortDefinition = catalogSpecParams.Sort.ToLower() switch
                {
                    "priceasc" => Builders<Product>.Sort.Ascending(product => product.Price),
                    "pricedesc" => Builders<Product>.Sort.Descending(product => product.Price),
                    _ => Builders<Product>.Sort.Ascending(product => product.Name)
                };
            }
            return await _productsCollection.Find(filter)
                .Sort(sortDefinition)
                .Skip((catalogSpecParams.PageIndex - 1) * catalogSpecParams.PageSize)
                .Limit(catalogSpecParams.PageSize)
                .ToListAsync();
        }
        #endregion
    }
}

