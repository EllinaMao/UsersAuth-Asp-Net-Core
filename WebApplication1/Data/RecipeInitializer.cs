using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class RecipeInitializer
    {
        public static async Task InitializeAsync(ApplicationContext context)
        {
            if (!await context.Recipes.AnyAsync())
            {
                string userId = context.Users.Select(e => e.Id).FirstOrDefault()!;
                if (userId != null)
                {
                    context.Recipes.AddRange(new Recipe[]
                    {
                new Recipe
                    {
                        UserId = userId,
                        Name = "Chicken soup",
                        Description = "This deliciously comforting chicken soup recipe is easy to make with a whole chicken."
                    },
                new Recipe
                    {
                        UserId = userId,
                        Name = "French fries",
                        Description = "Learn the secret to making the best homemade French fries from russet potatoes."
                    }
                    });
                    await context.SaveChangesAsync();
                }

            }
        }
    }
}
