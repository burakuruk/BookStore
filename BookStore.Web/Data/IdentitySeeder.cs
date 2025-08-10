using Microsoft.AspNetCore.Identity;

namespace BookStore.Web.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var roles = new[] { "Admin", "User" };
            foreach (var r in roles)
            {
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole(r));
            }

            var adminEmail = "admin@bookstore.local";
            var admin = await userMgr.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userMgr.CreateAsync(admin, "Admin123!" );
                await userMgr.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}


