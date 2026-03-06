using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class User:IdentityUser
    {
        public int Year { get; set; }

        public ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
        public ICollection<Friendship> FriendOf { get; set; } = new List<Friendship>();
    }
}
