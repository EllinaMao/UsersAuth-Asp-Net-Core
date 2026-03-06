using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.ViewModels.Profile;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IAuthorizationService _authService;


        public ProfileController(ApplicationContext context, IAuthorizationService authService)
        {
            _context = context;
            _authService = authService;
        }


        public async Task<IActionResult> Profile(string id = null)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = id ?? currentUserId;

            if (userId == null)
                return NotFound();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            var isOwnProfile = userId == currentUserId;

            // Получаем только принятых друзей
            var friends = await _context.Friendships
                .Where(f => f.Status == FriendshipStatus.Accepted && 
                           (f.UserId == userId || f.FriendId == userId))
                .Select(f => f.UserId == userId ? f.Friend : f.User)
                .Select(friend => new FriendViewModel
                {
                    Id = friend.Id,
                    Email = friend.Email,
                    UserName = friend.UserName,
                    Year = friend.Year
                })
                .ToListAsync();

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Year = user.Year,
                Friends = friends,
                IsOwnProfile = isOwnProfile
            };


            // Если это свой профиль - показываем входящие и исходящие запросы
            if (isOwnProfile)
            {
                // Входящие запросы (кто нам отправил)
                viewModel.PendingRequests = await _context.Friendships
                    .Where(f => f.FriendId == currentUserId && f.Status == FriendshipStatus.Pending)
                    .Select(f => f.User)
                    .Select(u => new FriendViewModel
                    {
                        Id = u.Id,
                        Email = u.Email,
                        UserName = u.UserName,
                        Year = u.Year
                    })
                    .ToListAsync();

                // Исходящие запросы (кому мы отправили)
                viewModel.SentRequests = await _context.Friendships
                    .Where(f => f.UserId == currentUserId && f.Status == FriendshipStatus.Pending)
                    .Select(f => f.Friend)
                    .Select(u => new FriendViewModel
                    {
                        Id = u.Id,
                        Email = u.Email,
                        UserName = u.UserName,
                        Year = u.Year
                    })
                    .ToListAsync();
            }
            else
            {
                // Проверяем статус дружбы с текущим пользователем
                var friendship = await _context.Friendships
                    .FirstOrDefaultAsync(f => 
                        (f.UserId == currentUserId && f.FriendId == userId) ||
                        (f.UserId == userId && f.FriendId == currentUserId));

                if (friendship != null)
                {
                    viewModel.IsFriend = friendship.Status == FriendshipStatus.Accepted;
                    viewModel.HasPendingRequest = friendship.Status == FriendshipStatus.Pending;
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(string friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null || friendId == null || userId == friendId)
                return BadRequest("Invalid request");

            var friendExists = await _context.Users.AnyAsync(u => u.Id == friendId);
            if (!friendExists)
                return NotFound("User not found");

            // Проверяем, нет ли уже связи между пользователями
            var existingFriendship = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    (f.UserId == userId && f.FriendId == friendId) ||
                    (f.UserId == friendId && f.FriendId == userId));

            if (existingFriendship != null)
            {
                if (existingFriendship.Status == FriendshipStatus.Accepted)
                    return BadRequest("Already friends");
                if (existingFriendship.Status == FriendshipStatus.Pending)
                    return BadRequest("Friend request already sent");
            }

            var friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId,
                Status = FriendshipStatus.Pending
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile), new { id = friendId });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(string friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null || friendId == null)
                return BadRequest("Invalid request");

            // Ищем запрос, где friendId - отправитель, а userId - получатель
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);

            if (friendship == null)
                return NotFound("Friend request not found");

            friendship.Status = FriendshipStatus.Accepted;
            friendship.AcceptedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        public async Task<IActionResult> RejectFriendRequest(string friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null || friendId == null)
                return BadRequest("Invalid request");

            // Ищем запрос, где friendId - отправитель, а userId - получатель
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);

            if (friendship == null)
                return NotFound("Friend request not found");

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(string friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null || friendId == null)
                return BadRequest("Invalid request");

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    ((f.UserId == userId && f.FriendId == friendId) ||
                     (f.UserId == friendId && f.FriendId == userId)) &&
                    f.Status == FriendshipStatus.Accepted);

            if (friendship == null)
                return NotFound("Friendship not found");

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        public async Task<IActionResult> CancelFriendRequest(string friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null || friendId == null)
                return BadRequest("Invalid request");

            // Ищем запрос, где userId - отправитель
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId && f.Status == FriendshipStatus.Pending);

            if (friendship == null)
                return NotFound("Friend request not found");

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile), new { id = friendId });
        }

        [AllowAnonymous]
        public async Task<IActionResult> SearchUsers(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new List<FriendViewModel>());

            var users = await _context.Users
                .Where(u => u.Email.Contains(query) || u.UserName.Contains(query))
                .Take(20)
                .Select(u => new FriendViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    Year = u.Year
                })
                .ToListAsync();

            return View(users);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

