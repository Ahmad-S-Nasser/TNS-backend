using MongoDB.Driver;
using TipsAndSteps.QA.Application.Abstractions;
using TipsAndSteps.QA.Domain.Entities;
using TipsAndSteps.Shared.Infrastructure.MongoDB;
using Microsoft.Extensions.Options;

namespace TipsAndSteps.QA.Infrastructure.Persistence.Repositories;

public sealed class QADbContext : MongoDbContext
{
    public IMongoCollection<Question> Questions => WriteCollection<Question>("questions");
    public QADbContext(IOptions<MongoDbSettings> options) : base(options) 
    { 
        RegisterMap<Question>();
    }

    private void RegisterMap<T>() where T : class
    {
        if (!MongoDB.Bson.Serialization.BsonClassMap.IsClassMapRegistered(typeof(T)))
        {
            MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<T>(cm => {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}

public sealed class QARepository : IQARepository
{
    private readonly QADbContext _ctx;
    public QARepository(QADbContext ctx) => _ctx = ctx;

    public Task<Question?> GetByIdAsync(string id, CancellationToken ct = default)
        => _ctx.Questions.Find(x => x.Id == id).FirstOrDefaultAsync(ct)!;

    public async Task<IReadOnlyList<Question>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Questions.Find(_ => true).SortByDescending(x => x.SubmittedAt).ToListAsync(ct);

    public Task CreateAsync(Question question, CancellationToken ct = default)
        => _ctx.Questions.InsertOneAsync(question, cancellationToken: ct);

    public Task UpdateAsync(Question question, CancellationToken ct = default)
        => _ctx.Questions.ReplaceOneAsync(x => x.Id == question.Id, question, cancellationToken: ct);
}
