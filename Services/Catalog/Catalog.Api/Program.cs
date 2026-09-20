using Catalog.Application.Handlers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Register Custom Serializer for MongoDB ObjectId
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add Swagger Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Mediator
var assemblies = new Assembly[]
{
    Assembly.GetExecutingAssembly(),
    typeof(GetAllBrandsHandler).Assembly
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

// Custom Servcies
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ITypeRepository, TypeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();


// Add MongoDB settings configuration
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
// Add MongoDB client as a singleton service
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});


var app = builder.Build();

//Seed MongoDB on startup
using(var scope = app.Services.CreateScope())
{
    var options = scope.ServiceProvider.GetRequiredService<IOptions<MongoDBSettings>>();
    var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
    await DatabaseSeeder.SeedDataAsync(options, mongoClient);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
