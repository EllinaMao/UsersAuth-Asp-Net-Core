using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Authorization
{
    public class ViewRecipeRequirement : IAuthorizationRequirement { }

    public class EditRecipeRequirement : IAuthorizationRequirement { }

    public class DeleteRecipeRequirement : IAuthorizationRequirement { }

    public class HideRecipeRequirement : IAuthorizationRequirement { }
}
