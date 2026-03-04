using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class IsRecipeOwnerRequirement : IAuthorizationRequirement { }
    public class IsRecipeOwnerHandler : AuthorizationHandler<IsRecipeOwnerRequirement, Recipe>
    {
        private readonly UserManager<User> _userManager;

        public IsRecipeOwnerHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsRecipeOwnerRequirement requirement, Recipe resource)
        {
            var currentUser = await _userManager.GetUserAsync(context.User);
            if (currentUser == null)
            {
                return;
            }
            if (resource.UserId == currentUser.Id)
            {
                context.Succeed(requirement);
            }
        }
    }
}
