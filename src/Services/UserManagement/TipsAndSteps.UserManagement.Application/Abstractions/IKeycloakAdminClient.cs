namespace TipsAndSteps.UserManagement.Application.Abstractions;

public sealed class KeycloakUserRequest
{
    public string Email     { get; init; } = string.Empty;
    public string Password  { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName  { get; init; } = string.Empty;
    public string RealmRole { get; init; } = string.Empty; // parent | doctor | admin | superadmin
}

public interface IKeycloakAdminClient
{
    Task<string> CreateUserAsync(KeycloakUserRequest request, CancellationToken ct = default);
    Task AssignRealmRoleAsync(string keycloakId, string role, CancellationToken ct = default);
    Task DeleteUserAsync(string keycloakId, CancellationToken ct = default);
    Task<bool> ValidateTokenAsync(string accessToken, CancellationToken ct = default);
}
