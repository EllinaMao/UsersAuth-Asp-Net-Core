namespace WebApplication1.Authorization
{
    public static class RecipeOperations
    {
        public const string View = "ViewRecipe";
        public const string Edit = "EditRecipe";
        public const string Delete = "DeleteRecipe";
        public const string Hide = "HideRecipe";
    }

    public static class RecipeVisibilityTypes
    {
        public const string Public = "Public";
        public const string Private = "Private";
        public const string FriendsOnly = "FriendsOnly";
    }
}
