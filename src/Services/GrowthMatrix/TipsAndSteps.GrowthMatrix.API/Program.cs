using FluentValidation;
using TipsAndSteps.Shared.Infrastructure.Jwt;
using MediatR;
using Serilog;
using TipsAndSteps.GrowthMatrix.Application.Commands.RecordAssessment;
using TipsAndSteps.GrowthMatrix.Application.Abstractions;
using TipsAndSteps.GrowthMatrix.Application.Services;
using TipsAndSteps.GrowthMatrix.Infrastructure.Persistence.Repositories;
using TipsAndSteps.GrowthMatrix.Infrastructure.Kafka;
using TipsAndSteps.Shared.Infrastructure;
using TipsAndSteps.Shared.Infrastructure.MongoDB;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext()
    .WriteTo.Console().WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://seq:80"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RecordAssessmentHandler).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<RecordAssessmentHandler>();
builder.Services.AddSingleton<GrowthScoringEngine>();

builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<GrowthDbContext>();
builder.Services.AddScoped<IGrowthSkillRepository, GrowthSkillRepository>();
builder.Services.AddScoped<IGrowthAssessmentRepository, GrowthAssessmentRepository>();
builder.Services.AddScoped<IGrowthAgeGroupRepository, GrowthAgeGroupRepository>();
builder.Services.AddScoped<IGrowthMatrixCategoryRepository, GrowthMatrixCategoryRepository>();
builder.Services.AddScoped<IGrowthRuleRepository, GrowthRuleRepository>();
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddScoped<IGrowthEventPublisher, GrowthEventPublisher>();

builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "GrowthMatrix Service", Version = "v1" }));

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "growth-matrix" }));
app.Run();
