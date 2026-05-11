using TipsAndSteps.UserManagement.Domain.Entities;
using BCryptNet = BCrypt.Net.BCrypt;
using TipsAndSteps.UserManagement.Domain.Enums;
using TipsAndSteps.UserManagement.Application.Abstractions;
using MongoDB.Driver;

namespace TipsAndSteps.UserManagement.API.Infrastructure;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        Console.WriteLine("[DbSeeder] Starting seeding...");
        using var scope = services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var readRepo = scope.ServiceProvider.GetRequiredService<IUserReadRepository>();

        var passwordHash = BCryptNet.HashPassword("admin123");
        var adminUsers = new List<User>
        {
            new User {
                Id = "admin-1",
                Email = "dr.mohamed@tns.com",
                PasswordHash = passwordHash,
                FirstName = "Mohamed",
                LastName = "Ali",
                Role = UserRole.Doctor,
                RoleCategory = RoleCategory.Doctor,
                AccountStatus = "active",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            },
            new User {
                Id = "admin-2",
                Email = "sara.marketing@tns.com",
                PasswordHash = passwordHash,
                FirstName = "Sara",
                LastName = "Khaled",
                Role = UserRole.Admin,
                RoleCategory = RoleCategory.Marketing,
                AccountStatus = "active",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new User {
                Id = "admin-3",
                Email = "reviewer.fatma@tns.com",
                PasswordHash = passwordHash,
                FirstName = "Fatma",
                LastName = "Zahra",
                Role = UserRole.Admin,
                RoleCategory = RoleCategory.ContentReviewer,
                AccountStatus = "active",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-1)
            },
            new User {
                Id = "admin-4",
                Email = "support.it@tns.com",
                PasswordHash = passwordHash,
                FirstName = "Ahmed",
                LastName = "Hassan",
                Role = UserRole.Admin,
                RoleCategory = RoleCategory.ItSupport,
                AccountStatus = "active",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            },
            new User {
                Id = "admin-5",
                Email = "super.admin@tns.com",
                PasswordHash = passwordHash,
                FirstName = "System",
                LastName = "Administrator",
                Role = UserRole.SuperAdmin,
                RoleCategory = RoleCategory.SuperAdmin,
                AccountStatus = "active",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow.AddYears(-1)
            }
        };

        foreach (var user in adminUsers)
        {
            var existing = await readRepo.FindByEmailAsync(user.Email);
            if (existing == null)
            {
                Console.WriteLine($"[DbSeeder] Creating admin user: {user.Email}");
                await repo.CreateAsync(user);
            }
            else if (string.IsNullOrEmpty(existing.PasswordHash))
            {
                Console.WriteLine($"[DbSeeder] Updating password for existing user: {user.Email}");
                existing.PasswordHash = passwordHash;
                await repo.UpdateAsync(existing);
            }
            else 
            {
                Console.WriteLine($"[DbSeeder] Admin user already exists with password: {user.Email}");
            }
        }
        Console.WriteLine("[DbSeeder] Seeding completed.");

        // Seed Role Defaults
        var ctx = scope.ServiceProvider.GetRequiredService<TipsAndSteps.UserManagement.Infrastructure.Persistence.UserManagementDbContext>();
        if (!(await ctx.RoleDefaults.Find(_ => true).AnyAsync()))
        {
            await ctx.RoleDefaults.InsertManyAsync(new List<RoleDefault>
            {
                new RoleDefault { Id = "1", Category = RoleCategory.Doctor, Permissions = new List<string> { "questions.answer", "questions.create", "content.review", "health_intelligence.view" } },
                new RoleDefault { Id = "2", Category = RoleCategory.Marketing, Permissions = new List<string> { "content.create", "analytics.view", "faqs.manage" } },
                new RoleDefault { Id = "3", Category = RoleCategory.ContentReviewer, Permissions = new List<string> { "content.review", "content.publish", "content.approve", "analytics.view" } },
                new RoleDefault { Id = "4", Category = RoleCategory.ItSupport, Permissions = new List<string> { "users.manage", "system.logs.view", "settings.manage", "analytics.view" } },
                new RoleDefault { Id = "5", Category = RoleCategory.SuperAdmin, Permissions = new List<string> { "content.create", "content.publish", "content.review", "content.delete", "content.approve", "questions.create", "questions.answer", "questionnaires.manage", "faqs.manage", "analytics.view", "health_intelligence.view", "users.manage", "rbac.manage", "system.logs.view", "settings.manage" } }
            });
        }
    }
}
