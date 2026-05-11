using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TipsAndSteps.Notification.Domain.Entities;
using TipsAndSteps.Shared.Infrastructure.MongoDB;

namespace TipsAndSteps.Notification.Infrastructure.Persistence;

public sealed class NotificationDbContext : MongoDbContext
{
    public IMongoCollection<AdminNotification> AdminNotifications => WriteCollection<AdminNotification>("admin_notifications");
    public NotificationDbContext(IOptions<MongoDbSettings> options) : base(options) 
    { 
        if (!MongoDB.Bson.Serialization.BsonClassMap.IsClassMapRegistered(typeof(AdminNotification)))
        {
            MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<AdminNotification>(cm => {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance)
                  .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(MongoDB.Bson.BsonType.ObjectId));
            });
        }
    }
}

public interface INotificationRepository
{
    Task<IReadOnlyList<AdminNotification>> GetAdminNotificationsAsync(CancellationToken ct = default);
    Task CreateAsync(AdminNotification notification, CancellationToken ct = default);
}

public sealed class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _ctx;
    public NotificationRepository(NotificationDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<AdminNotification>> GetAdminNotificationsAsync(CancellationToken ct = default)
        => await _ctx.AdminNotifications.Find(_ => true).SortByDescending(n => n.CreatedAt).ToListAsync(ct);

    public Task CreateAsync(AdminNotification notification, CancellationToken ct = default)
        => _ctx.AdminNotifications.InsertOneAsync(notification, cancellationToken: ct);
}
