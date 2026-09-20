using Catalog.Core.Entities;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedDataAsync(IOptions<MongoDBSettings> options, IMongoClient client)
        {
            var settings = options.Value;
            var database = client.GetDatabase(settings.DatabaseName);
            var productsCollection = database.GetCollection<Product>(settings.ProductCollectionName);
            var brandsCollection = database.GetCollection<ProductBrand>(settings.BrandCollectionName);
            var typesCollection = database.GetCollection<ProductType>(settings.TypeCollectionName);
            var seedBasePath = Path.Combine(AppContext.BaseDirectory,"Data", "SeedData");
            // Seed Brands
            if (await brandsCollection.CountDocumentsAsync(FilterDefinition<ProductBrand>.Empty) == 0)
            {
                var brandsFilePath = Path.Combine(seedBasePath, "brands.json");
                var brandsData = await File.ReadAllTextAsync(brandsFilePath);
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                if (brands != null)
                {
                    await brandsCollection.InsertManyAsync(brands);
                }
            }
            // Seed Types
            if (await typesCollection.CountDocumentsAsync(FilterDefinition<ProductType>.Empty) == 0)
            {
                var typesFilePath = Path.Combine(seedBasePath, "types.json");
                var typesData = await File.ReadAllTextAsync(typesFilePath);
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                if (types != null)
                {
                    await typesCollection.InsertManyAsync(types);
                }
            }
            // Seed Products
            if (await productsCollection.CountDocumentsAsync(FilterDefinition<Product>.Empty) == 0)
            {
                var productsFilePath = Path.Combine(seedBasePath, "products.json");
                var productsData = await File.ReadAllTextAsync(productsFilePath);
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                if (products != null)
                {
                    foreach (var product in products)
                    {
                        //Reset Id to let Mongo generate one
                        product.Id = null;
                        //Default Created Date if not set
                        if (product.CreatedDate == default)
                            product.CreatedDate = DateTime.UtcNow;
                    }
                    await productsCollection.InsertManyAsync(products);
                }
            }
        }
    }
}
