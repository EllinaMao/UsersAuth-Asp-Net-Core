namespace WebApplication1.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public int Year { get; set; }
        public List<FriendViewModel> Friends { get; set; } = new List<FriendViewModel>();
        public List<FriendViewModel> PendingRequests { get; set; } = new List<FriendViewModel>();
        public List<FriendViewModel> SentRequests { get; set; } = new List<FriendViewModel>();
        public bool IsOwnProfile { get; set; }
        public bool IsFriend { get; set; }
        public bool HasPendingRequest { get; set; }
    }
}
