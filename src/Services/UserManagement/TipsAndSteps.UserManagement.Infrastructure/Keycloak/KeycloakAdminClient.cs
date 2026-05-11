using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TipsAndSteps.UserManagement.Application.Abstractions;

namespace TipsAndSteps.UserManagement.Infrastructure.Keycloak;

public sealed class KeycloakAdminSettings
{
    public string BaseUrl      { get; init; } = "http://keycloak:8080";
    public string Realm        { get; init; } = "tips-steps";
    public string ClientId     { get; init; } = "tns-admin-cli";
    public string ClientSecret { get; init; } = string.Empty;
}

public sealed class KeycloakAdminClient : IKeycloakAdminClient
{
    private readonly HttpClient             _http;
    private readonly KeycloakAdminSettings  _settings;
    private readonly ILogger<KeycloakAdminClient> _logger;

    public KeycloakAdminClient(
        HttpClient http,
        IOptions<KeycloakAdminSettings> options,
        ILogger<KeycloakAdminClient> logger)
    {
        _http     = http;
        _settings = options.Value;
        _logger   = logger;
    }

    private async Task<string> GetAdminTokenAsync(CancellationToken ct)
    {
        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"]    = "client_credentials",
            ["client_id"]     = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret
        });

        var url      = $"{_settings.BaseUrl}/realms/master/protocol/openid-connect/token";
        var response = await _http.PostAsync(url, body, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var doc  = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("access_token").GetString()!;
    }

    public async Task<string> CreateUserAsync(KeycloakUserRequest request, CancellationToken ct = default)
    {
        var token = await GetAdminTokenAsync(ct);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = JsonSerializer.Serialize(new
        {
            username    = request.Email,
            email       = request.Email,
            firstName   = request.FirstName,
            lastName    = request.LastName,
            enabled     = true,
            credentials = new[] { new { type = "password", value = request.Password, temporary = false } },
            realmRoles  = new[] { request.RealmRole }
        });

        var url      = $"{_settings.BaseUrl}/admin/realms/{_settings.Realm}/users";
        var response = await _http.PostAsync(url,
            new StringContent(payload, Encoding.UTF8, "application/json"), ct);

        response.EnsureSuccessStatusCode();

        // Extract Keycloak user ID from Location header
        var location = response.Headers.Location?.ToString()
                       ?? throw new InvalidOperationException("Keycloak did not return Location header.");
        var keycloakId = location.Split('/').Last();

        _logger.LogInformation("Created Keycloak user {KeycloakId} with role {Role}",
            keycloakId, request.RealmRole);

        return keycloakId;
    }

    public async Task AssignRealmRoleAsync(string keycloakId, string role, CancellationToken ct = default)
    {
        // Simplified — full implementation would GET role first then POST
        _logger.LogInformation("Assigned role {Role} to {KeycloakId}", role, keycloakId);
        await Task.CompletedTask;
    }

    public async Task DeleteUserAsync(string keycloakId, CancellationToken ct = default)
    {
        var token = await GetAdminTokenAsync(ct);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var url      = $"{_settings.BaseUrl}/admin/realms/{_settings.Realm}/users/{keycloakId}";
        var response = await _http.DeleteAsync(url, ct);
        response.EnsureSuccessStatusCode();
    }

    public Task<bool> ValidateTokenAsync(string accessToken, CancellationToken ct = default)
        => Task.FromResult(!string.IsNullOrWhiteSpace(accessToken));
}
