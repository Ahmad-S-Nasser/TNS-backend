using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TipsAndSteps.Analytics.Domain.Entities;
using TipsAndSteps.Shared.Infrastructure.MongoDB;

namespace TipsAndSteps.Analytics.Infrastructure.Persistence;

public sealed class AnalyticsDbContext : MongoDbContext
{
    public IMongoCollection<AnalyticsKPI> KPIs => WriteCollection<AnalyticsKPI>("analytics_kpis");
    public IMongoCollection<MonthlyMetric> MonthlyMetrics => WriteCollection<MonthlyMetric>("monthly_metrics");
    public IMongoCollection<ContentEngagementMetric> ContentMetrics => WriteCollection<ContentEngagementMetric>("content_engagement");
    
    public AnalyticsDbContext(IOptions<MongoDbSettings> options) : base(options) 
    { 
        RegisterMap<AnalyticsKPI>();
        RegisterMap<MonthlyMetric>();
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
                    idMember.SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance)
                             .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                }
            });
        }
    }
}

public interface IAnalyticsRepository
{
    Task<IReadOnlyList<AnalyticsKPI>> GetAllKPIsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MonthlyMetric>> GetMonthlyGrowthAsync(int months = 6, CancellationToken ct = default);
    Task<IReadOnlyList<ContentEngagementMetric>> GetContentEngagementAsync(CancellationToken ct = default);
    Task UpdateKPIAsync(string key, long value, CancellationToken ct = default);
}

public sealed class AnalyticsRepository : IAnalyticsRepository
{
    private readonly AnalyticsDbContext _ctx;
    public AnalyticsRepository(AnalyticsDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<AnalyticsKPI>> GetAllKPIsAsync(CancellationToken ct = default)
        => await _ctx.KPIs.Find(_ => true).ToListAsync(ct);

    public async Task<IReadOnlyList<MonthlyMetric>> GetMonthlyGrowthAsync(int months = 6, CancellationToken ct = default)
        => await _ctx.MonthlyMetrics.Find(_ => true).SortByDescending(m => m.YearMonth).Limit(months).ToListAsync(ct);

    public async Task<IReadOnlyList<ContentEngagementMetric>> GetContentEngagementAsync(CancellationToken ct = default)
        => await _ctx.ContentMetrics.Find(_ => true).ToListAsync(ct);

    public async Task UpdateKPIAsync(string key, long value, CancellationToken ct = default)
    {
        var filter = Builders<AnalyticsKPI>.Filter.Eq(k => k.Key, key);
        var update = Builders<AnalyticsKPI>.Update
            .Set(k => k.Value, value)
            .Set(k => k.LastUpdated, DateTime.UtcNow);
        
        await _ctx.KPIs.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, ct);
    }
}
