namespace WebApplication1.Models
{
    public class Friendship
    {
        public string UserId { get; set; }
        public User User { get; set; } = null!;

        public string FriendId { get; set; }
        public User Friend { get; set; } = null!;

        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }
    }
}
