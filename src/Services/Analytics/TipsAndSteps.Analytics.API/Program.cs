using TipsAndSteps.Shared.Infrastructure.Jwt;
using Serilog;
using TipsAndSteps.Shared.Infrastructure;
using TipsAndSteps.Shared.Infrastructure.MongoDB;
using TipsAndSteps.Analytics.Infrastructure.Persistence;
using TipsAndSteps.Analytics.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext()
    .WriteTo.Console().WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://seq:80"));

builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<AnalyticsDbContext>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddSharedInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Analytics Service", Version = "v1" }));

var app = builder.Build();

// Seed Analytics Data
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IAnalyticsRepository>();
    var existing = await repo.GetAllKPIsAsync();
    if (!existing.Any())
    {
        await repo.UpdateKPIAsync("totalUsers", 1250);
        await repo.UpdateKPIAsync("activeUsers", 450);
        await repo.UpdateKPIAsync("totalContent", 85);
        await repo.UpdateKPIAsync("publishedContent", 72);
        await repo.UpdateKPIAsync("pendingReview", 13);
        await repo.UpdateKPIAsync("totalQuestionnaires", 24);
        await repo.UpdateKPIAsync("totalResponses", 1120);
        await repo.UpdateKPIAsync("healthSignals", 56);
    }
}

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "analytics" }));
app.Run();
