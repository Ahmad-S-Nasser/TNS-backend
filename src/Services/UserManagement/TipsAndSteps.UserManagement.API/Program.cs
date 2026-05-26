using FluentValidation;
using TipsAndSteps.Shared.Infrastructure.Jwt;
using MediatR;
using Serilog;
using TipsAndSteps.UserManagement.Application.Commands.RegisterUser;
using TipsAndSteps.UserManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://seq:80"));

// ── MediatR + FluentValidation ────────────────────────────────────────────────
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
    typeof(RegisterUserHandler).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();

// ── Custom JWT Auth ───────────────────────────────────────────────────────────
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// ── Infrastructure ────────────────────────────────────────────────────────────
builder.Services.AddUserManagementInfrastructure(builder.Configuration);

// ── Controllers + Swagger ─────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "UserManagement Service", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type   = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

// ── Seed Users ────────────────────────────────────────────────────────────────
await TipsAndSteps.UserManagement.API.Infrastructure.DbSeeder.SeedAsync(app.Services);

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "usermgmt" }));

app.Run();
