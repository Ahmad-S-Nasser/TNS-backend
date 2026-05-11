using FluentValidation;
using TipsAndSteps.Shared.Infrastructure.Jwt;
using MediatR;
using Serilog;
using TipsAndSteps.Content.Application.Commands.CreateArticle;
using TipsAndSteps.Content.Application.Abstractions;
using TipsAndSteps.Content.Infrastructure.Persistence.Repositories;
using TipsAndSteps.Content.Infrastructure.Cache;
using TipsAndSteps.Content.Infrastructure.Kafka;
using TipsAndSteps.Shared.Infrastructure;
using TipsAndSteps.Shared.Infrastructure.MongoDB;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://seq:80"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateArticleHandler).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<CreateArticleHandler>();

builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<ContentDbContext>();
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<IContentReadRepository, ContentRepository>();

// Cache & Kafka
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddScoped<IContentCacheService, ContentCacheService>();
builder.Services.AddScoped<IContentEventPublisher, ContentEventPublisher>();

builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Content Service", Version = "v1" }));

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "content" }));
app.Run();
