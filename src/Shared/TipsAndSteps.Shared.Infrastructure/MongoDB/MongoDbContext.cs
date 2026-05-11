using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TipsAndSteps.Shared.Infrastructure.MongoDB;

/// <summary>
/// Base MongoDB context providing write (primary) and read (replica) clients.
/// CQRS: Commands use WriteDatabase, Queries use ReadDatabase.
/// </summary>
public abstract class MongoDbContext
{
    protected readonly IMongoDatabase WriteDatabase;
    protected readonly IMongoDatabase ReadDatabase;

    protected MongoDbContext(IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;

        var writeClient = new MongoClient(settings.ConnectionString);
        WriteDatabase = writeClient.GetDatabase(settings.DatabaseName);

        var readClient = new MongoClient(settings.ReadReplicaConnectionString);
        ReadDatabase = readClient.GetDatabase(settings.DatabaseName);
    }

    protected IMongoCollection<T> WriteCollection<T>(string name)
        => WriteDatabase.GetCollection<T>(name);

    protected IMongoCollection<T> ReadCollection<T>(string name)
        => ReadDatabase.GetCollection<T>(name);
}
