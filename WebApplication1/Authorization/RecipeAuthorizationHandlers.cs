using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Authorization
{
    // Обработчик для просмотра рецепта
    public class ViewRecipeHandler : AuthorizationHandler<ViewRecipeRequirement, Recipe>
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;

        public ViewRecipeHandler(UserManager<User> userManager, ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            ViewRecipeRequirement requirement, 
            Recipe resource)
        {
            // Admin и Moderator видят все
            if (context.User.IsInRole("Admin") || context.User.IsInRole("Moderator"))
            {
                context.Succeed(requirement);
                return;
            }

            // Загружаем информацию о видимости рецепта
            var recipe = await _context.Recipes
                .Include(r => r.RecipeVisibility)
                .FirstOrDefaultAsync(r => r.Id == resource.Id);

            if (recipe == null)
            {
                return;
            }

            // Public рецепты видят все
            if (recipe.RecipeVisibility?.Name == "Public")
            {
                context.Succeed(requirement);
                return;
            }

            var currentUser = await _userManager.GetUserAsync(context.User);
            if (currentUser == null)
            {
                return;
            }

            // Private - только владелец
            if (recipe.RecipeVisibility?.Name == "Private")
            {
                if (recipe.UserId == currentUser.Id)
                {
                    context.Succeed(requirement);
                }
                return;
            }

            // FriendsOnly - владелец или друзья
            if (recipe.RecipeVisibility?.Name == "FriendsOnly")
            {
                if (recipe.UserId == currentUser.Id)
                {
                    context.Succeed(requirement);
                    return;
                }

                // TODO: Здесь нужно проверить, является ли currentUser другом владельца рецепта
                // Для этого понадобится добавить систему дружбы
                // Пример: var isFriend = await _context.Friendships
                //     .AnyAsync(f => f.UserId == recipe.UserId && f.FriendId == currentUser.Id);
                // if (isFriend)
                // {
                //     context.Succeed(requirement);
                // }
            }
        }
    }

    // Обработчик для редактирования рецепта
    public class EditRecipeHandler : AuthorizationHandler<EditRecipeRequirement, Recipe>
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;

        public EditRecipeHandler(UserManager<User> userManager, ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            EditRecipeRequirement requirement, 
            Recipe resource)
        {
            var currentUser = await _userManager.GetUserAsync(context.User);
            if (currentUser == null)
            {
                return;
            }

            // Admin может редактировать все
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            // Moderator может редактировать публичные рецепты
            if (context.User.IsInRole("Moderator"))
            {
                var recipe = await _context.Recipes
                    .Include(r => r.RecipeVisibility)
                    .FirstOrDefaultAsync(r => r.Id == resource.Id);

                if (recipe?.RecipeVisibility?.Name == "Public")
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            // Владелец может редактировать свой рецепт
            if (resource.UserId == currentUser.Id)
            {
                context.Succeed(requirement);
            }
        }
    }

    // Обработчик для удаления рецепта
    public class DeleteRecipeHandler : AuthorizationHandler<DeleteRecipeRequirement, Recipe>
    {
        private readonly UserManager<User> _userManager;

        public DeleteRecipeHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            DeleteRecipeRequirement requirement, 
            Recipe resource)
        {
            // Admin может удалять все
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            var currentUser = await _userManager.GetUserAsync(context.User);
            if (currentUser == null)
            {
                return;
            }

            // Владелец может удалять свой рецепт
            if (resource.UserId == currentUser.Id)
            {
                context.Succeed(requirement);
            }
        }
    }

    // Обработчик для скрытия рецепта (модератор)
    public class HideRecipeHandler : AuthorizationHandler<HideRecipeRequirement, Recipe>
    {
        private readonly ApplicationContext _context;

        public HideRecipeHandler(ApplicationContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            HideRecipeRequirement requirement, 
            Recipe resource)
        {
            // Только Moderator и Admin могут скрывать рецепты
            if (context.User.IsInRole("Admin") || context.User.IsInRole("Moderator"))
            {
                var recipe = await _context.Recipes
                    .Include(r => r.RecipeVisibility)
                    .FirstOrDefaultAsync(r => r.Id == resource.Id);

                // Moderator может скрывать только публичные рецепты
                if (context.User.IsInRole("Moderator") && recipe?.RecipeVisibility?.Name == "Public")
                {
                    context.Succeed(requirement);
                    return;
                }

                // Admin может скрывать любые рецепты
                if (context.User.IsInRole("Admin"))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
