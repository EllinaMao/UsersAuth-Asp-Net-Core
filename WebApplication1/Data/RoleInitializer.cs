using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@gmail.com", adminPassword = "qwerty";
            string userEmail = "user@gmail.com", userPassword = "qwerty";

            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (await roleManager.FindByNameAsync("RegisteredUser") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("RegisteredUser"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail };
                User user = new User { Email = userEmail, UserName = userEmail };
                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
                IdentityResult result2 = await userManager.CreateAsync(user, userPassword);
                if (result2.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "RegisteredUser");
                }

            }
        }
    }
}
