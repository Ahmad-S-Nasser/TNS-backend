using TipsAndSteps.Shared.Infrastructure.Jwt;
using Serilog;
using TipsAndSteps.Shared.Infrastructure;
using TipsAndSteps.Shared.Infrastructure.MongoDB;
using TipsAndSteps.Notification.Infrastructure.Persistence;
using TipsAndSteps.Notification.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext()
    .WriteTo.Console().WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://seq:80"));

builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<NotificationDbContext>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddSharedInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Notification Service", Version = "v1" }));

var app = builder.Build();

// Seed Notification Data
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
    var existing = await repo.GetAdminNotificationsAsync();
    if (!existing.Any())
    {
        await repo.CreateAsync(new() { Title = "Welcome to TipsAndSteps", Message = "The system is ready.", Type = "info" });
        await repo.CreateAsync(new() { Title = "Growth Matrix Alert", Message = "5 children need immediate attention.", Type = "warning" });
    }
}

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "notification" }));
app.Run();
