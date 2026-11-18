using JwtMusicNight.WebApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace JwtMusicNight.WebApi.Context
{
    // Admin rolünü ve admin kullanıcısını oluşturur.
    public static class AdminSeeder
    {
        public const string AdminRole = "Admin";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            if (!await roleManager.RoleExistsAsync(AdminRole))
                await roleManager.CreateAsync(new IdentityRole(AdminRole));

            var admin = await userManager.FindByNameAsync("admin");
            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@musicnight.com",
                    Name = "Site",
                    Surname = "Yöneticisi",
                    PackageLevel = PackageLevel.Elit
                };
                var result = await userManager.CreateAsync(admin, "Admin123*");
                if (!result.Succeeded) return;
            }

            if (!await userManager.IsInRoleAsync(admin, AdminRole))
                await userManager.AddToRoleAsync(admin, AdminRole);
        }
    }
}
