namespace Catalog.Infrastructure.Settings
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ProductCollectionName { get; set; }
        public string BrandCollectionName { get; set; }
        public string TypeCollectionName { get; set; }
    }
}
