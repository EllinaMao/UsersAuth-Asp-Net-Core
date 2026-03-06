using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Models
{
    public class ViewRecipeRequirement : IAuthorizationRequirement { }

    public class EditRecipeRequirement : IAuthorizationRequirement { }

    public class DeleteRecipeRequirement : IAuthorizationRequirement { }

    public class HideRecipeRequirement : IAuthorizationRequirement { }
}
