using Helth.Models;
using Microsoft.AspNetCore.Identity;

namespace Helth.Data;

public static class DbSeeder
{
    public const string DefaultAdminUsername = "admin";
    public const string DefaultAdminPassword = "Admin@12345";

    public static void SeedAdmin(ApplicationDbContext context)
    {
        if (context.Admins.Any())
        {
            return;
        }

        var hasher = new PasswordHasher<Admin>();
        var admin = new Admin
        {
            Username = DefaultAdminUsername
        };
        admin.PasswordHash = hasher.HashPassword(admin, DefaultAdminPassword);

        context.Admins.Add(admin);
        context.SaveChanges();
    }
}
