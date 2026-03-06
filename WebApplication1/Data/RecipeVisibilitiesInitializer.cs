using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class RecipeVisibilitiesInitializer
    {
        public static async Task InitializeAsync(ApplicationContext context)
        {
            if (!await context.RecipeVisibilities.AnyAsync())
            {
                var visibilities = new List<RecipeVisibility>
                {
                    new RecipeVisibility { Name = "Public" },
                    new RecipeVisibility { Name = "Private" },
                    new RecipeVisibility { Name = "FriendsOnly" }
                };

                context.RecipeVisibilities.AddRange(visibilities);
                await context.SaveChangesAsync();
            }
        }
    }
}