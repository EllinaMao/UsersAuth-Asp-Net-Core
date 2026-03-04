using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class NotesInitializer
    {
        public static async Task InitializeAsync(ApplicationContext context)
        {
            if (!await context.Notes.AnyAsync())
            {
                string userId = context.Users.Select(e => e.Id).FirstOrDefault()!;
                if (userId != null)
                {
                    context.Notes.AddRange(
                    [
                new Note{
                        UserId = userId,
                        Name = "Day One",
                        Description = "We found on the streets lost parrot!"

                        },
                       new Note{
                        UserId = userId,
                        Name = "Two hours after first note",
                        Description = "Now he flying on the kitchen. We gave him some food!"

                        }
                    ]);
                    await context.SaveChangesAsync();
                }

            }
        }
    }
}
