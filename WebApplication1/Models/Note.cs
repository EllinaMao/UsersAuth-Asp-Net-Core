namespace WebApplication1.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
