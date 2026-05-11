using MongoDB.Driver;
using TipsAndSteps.Content.Application.Abstractions;
using TipsAndSteps.Content.Domain.Entities;
using TipsAndSteps.Content.Domain.Enums;
using TipsAndSteps.Shared.Infrastructure.MongoDB;
using Microsoft.Extensions.Options;

namespace TipsAndSteps.Content.Infrastructure.Persistence.Repositories;

public sealed class ContentDbContext : MongoDbContext
{
    public IMongoCollection<ContentArticle> Articles     => WriteCollection<ContentArticle>("articles");
    public IMongoCollection<ContentArticle> ArticlesRead => ReadCollection<ContentArticle>("articles");
    public ContentDbContext(IOptions<MongoDbSettings> options) : base(options) 
    { 
        RegisterMap<ContentArticle>();
    }

    private void RegisterMap<T>() where T : class
    {
        if (!MongoDB.Bson.Serialization.BsonClassMap.IsClassMapRegistered(typeof(T)))
        {
            MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<T>(cm => {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                var idMember = cm.GetMemberMap("Id");
                if (idMember != null)
                {
                    idMember.SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(MongoDB.Bson.BsonType.String));
                }
            });
        }
    }
}

public sealed class ContentRepository : IContentRepository, IContentReadRepository
{
    private readonly ContentDbContext _ctx;

    public ContentRepository(ContentDbContext ctx) => _ctx = ctx;

    public Task CreateAsync(ContentArticle article, CancellationToken ct = default)
        => _ctx.Articles.InsertOneAsync(article, cancellationToken: ct);

    public async Task UpdateAsync(ContentArticle article, CancellationToken ct = default)
    {
        var filter = Builders<ContentArticle>.Filter.Eq(a => a.Id, article.Id);
        article.UpdatedAt = DateTime.UtcNow;
        await _ctx.Articles.ReplaceOneAsync(filter, article, cancellationToken: ct);
    }
    
    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _ctx.Articles.DeleteOneAsync(a => a.Id == id, ct);

    // Write primary reads
    public Task<ContentArticle?> FindByIdAsync(string id, CancellationToken ct = default)
        => _ctx.Articles.Find(a => a.Id == id).FirstOrDefaultAsync(ct)!;

    // Read replica reads
    public async Task<IReadOnlyList<ContentArticle>> ListBySectionAsync(
        ContentSection section, int page, int pageSize, ContentStatus? status = null, CancellationToken ct = default)
    {
        var filter = Builders<ContentArticle>.Filter.Eq(a => a.Section, section);
        if (status.HasValue)
        {
            filter &= Builders<ContentArticle>.Filter.Eq(a => a.Status, status.Value);
        }
        
        return await _ctx.ArticlesRead
            .Find(filter)
            .SortByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ContentArticle>> SearchAsync(
        string query, string language, int page, int pageSize, CancellationToken ct = default)
    {
        var field    = language == "ar" ? "TitleAr" : "TitleEn";
        var filter   = Builders<ContentArticle>.Filter.Regex(field, new MongoDB.Bson.BsonRegularExpression(query, "i"));
        return await _ctx.ArticlesRead
            .Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);
    }
    public async Task<IReadOnlyList<ContentArticle>> ListAllAsync(CancellationToken ct = default)
        => await _ctx.ArticlesRead.Find(_ => true).ToListAsync(ct);

    public async Task<Dictionary<string, int>> GetStatsAsync(CancellationToken ct = default)
    {
        var stats = await _ctx.ArticlesRead.Aggregate()
            .Group(a => a.Status, g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return stats.ToDictionary(x => x.Status.ToString(), x => x.Count);
    }
}
