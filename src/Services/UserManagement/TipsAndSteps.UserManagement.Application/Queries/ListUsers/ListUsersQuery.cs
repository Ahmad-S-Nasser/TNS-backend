using MediatR;
using TipsAndSteps.UserManagement.Application.Queries.GetUser;

namespace TipsAndSteps.UserManagement.Application.Queries.ListUsers;

public sealed record ListUsersQuery(int Page = 1, int PageSize = 20) : IRequest<IReadOnlyList<UserDto>>;
