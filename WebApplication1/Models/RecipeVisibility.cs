using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class RecipeVisibility
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
}
