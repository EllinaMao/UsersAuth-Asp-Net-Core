using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class IsNoteOwnerRequirement : IAuthorizationRequirement { }
    public class IsNoteOwnerHandler : AuthorizationHandler<IsNoteOwnerRequirement, Note>
    {
        private readonly UserManager<User> _userManager;

        public IsNoteOwnerHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsNoteOwnerRequirement requirement, Note resource)
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
