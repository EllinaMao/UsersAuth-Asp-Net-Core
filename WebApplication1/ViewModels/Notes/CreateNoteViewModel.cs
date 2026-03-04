using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class CreateNoteViewModel
    {
        [Required(ErrorMessage = "Введите название заметки")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите описание заметки")]
        [Display(Name = "Описание")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }
}
