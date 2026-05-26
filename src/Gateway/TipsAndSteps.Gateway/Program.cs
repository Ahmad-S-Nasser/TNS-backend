using TipsAndSteps.Shared.Infrastructure.Jwt;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://localhost:6093"));

// ── Custom JWT Authentication ────────────────────────────────────────────────
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// ── YARP Reverse Proxy ────────────────────────────────────────────────────────
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ── Swagger Aggregation ───────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── OpenTelemetry ─────────────────────────────────────────────────────────────
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("tns-gateway"))
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter());

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(
        builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? ["http://localhost:3000"])
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// ── Swagger UI Aggregation ────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/usermgmt/v1/swagger.json", "UserManagement Service");
    c.SwaggerEndpoint("/swagger/content/v1/swagger.json", "Content Service");
    c.SwaggerEndpoint("/swagger/growth/v1/swagger.json", "Growth Service");
    c.SwaggerEndpoint("/swagger/notification/v1/swagger.json", "Notification Service");
    c.SwaggerEndpoint("/swagger/analytics/v1/swagger.json", "Analytics Service");
    c.SwaggerEndpoint("/swagger/healthintel/v1/swagger.json", "HealthIntelligence Service");
    c.SwaggerEndpoint("/swagger/qa/v1/swagger.json", "QA Service");
    c.RoutePrefix = "swagger";
});

app.MapReverseProxy();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "gateway" }));

app.Run();
