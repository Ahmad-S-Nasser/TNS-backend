using TipsAndSteps.Shared.Infrastructure.Jwt;
using MediatR;
using Serilog;
using TipsAndSteps.QA.API.Hubs;
using TipsAndSteps.QA.API.Services;
using TipsAndSteps.QA.Application.Abstractions;
using TipsAndSteps.QA.Application.Queries.GetQuestions;
using TipsAndSteps.QA.Infrastructure.Persistence.Repositories;
using TipsAndSteps.Shared.Infrastructure;
using TipsAndSteps.Shared.Infrastructure.MongoDB;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext()
    .WriteTo.Console().WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://seq:80"));

// SignalR
builder.Services.AddSignalR();

// Auth
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<QADbContext>();
builder.Services.AddScoped<IQARepository, QARepository>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetQuestionsHandler).Assembly));

// Services
builder.Services.AddScoped<IQANotifier, QANotifier>();

builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "QA Service", Version = "v1" }));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<QAHub>("/hubs/qa");
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "qa" }));

app.Run();
