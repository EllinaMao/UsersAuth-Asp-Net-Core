namespace WebApplication1.Models
{
    //enum RecipeVisibility
    //{
    //    Private,    // Только владелец
    //    Friends,    // Только друзья
    //    Public      // Все пользователи
    //}
    public class Recipe
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }


        public int RecipeVisibilityId { get; set; }
        public RecipeVisibility RecipeVisibility { get; set; }

    }
}
