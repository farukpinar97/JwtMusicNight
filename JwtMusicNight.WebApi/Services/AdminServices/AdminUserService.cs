using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Services.AdminServices
{
    public class AdminUserService : IAdminUserService
    {
        private readonly JwtContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminUserService(JwtContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var dto = new AdminDashboardDto
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalSongs = await _context.Songs.CountAsync(),
                TotalArtists = await _context.Artists.CountAsync(),
                TotalGenres = await _context.Genres.CountAsync(),
                TotalListens = await _context.ListenHistories.CountAsync()
            };

            // Paket bazında kullanıcı ve şarkı sayıları
            var usersByPackage = await _context.Users.GroupBy(u => u.PackageLevel)
                .Select(g => new { g.Key, Count = g.Count() }).ToListAsync();
            var songsByPackage = await _context.Songs.GroupBy(s => s.RequiredPackage)
                .Select(g => new { g.Key, Count = g.Count() }).ToListAsync();

            foreach (var level in Enum.GetValues<PackageLevel>())
            {
                dto.UsersByPackage[level.ToString()] = usersByPackage.FirstOrDefault(x => x.Key == level)?.Count ?? 0;
                dto.SongsByPackage[level.ToString()] = songsByPackage.FirstOrDefault(x => x.Key == level)?.Count ?? 0;
            }

            // Son 7 günün dinlenmeleri
            var start = DateTime.Today.AddDays(-6);
            var recent = await _context.ListenHistories
                .Where(h => h.ListenedAt >= start)
                .Select(h => h.ListenedAt)
                .ToListAsync();

            for (var day = start; day <= DateTime.Today; day = day.AddDays(1))
                dto.ListensLast7Days.Add(new DailyListenDto { Date = day, Count = recent.Count(d => d.Date == day) });

            dto.TopSongs = await _context.Songs
                .OrderByDescending(s => s.ListenCount)
                .Take(5)
                .Select(s => new TopItemDto
                {
                    Id = s.SongId, Name = s.SongName, SubTitle = s.Artists.ArtistName,
                    ImageUrl = s.CoverImageUrl, ListenCount = s.ListenCount
                }).ToListAsync();

            dto.TopArtists = await _context.Artists
                .Select(a => new TopItemDto
                {
                    Id = a.ArtistId, Name = a.ArtistName, SubTitle = a.Country,
                    ImageUrl = a.ArtistImageUrl, ListenCount = a.Songs.Sum(s => (int?)s.ListenCount) ?? 0
                })
                .OrderByDescending(a => a.ListenCount)
                .Take(5)
                .ToListAsync();

            return dto;
        }

        public async Task<List<AdminUserDto>> GetUsersAsync(string? search, PackageLevel? package)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u => u.UserName!.Contains(search) || u.Email!.Contains(search)
                                      || u.Name.Contains(search) || u.Surname.Contains(search));

            if (package.HasValue)
                query = query.Where(u => u.PackageLevel == package.Value);

            var adminIds = (await _userManager.GetUsersInRoleAsync(AdminSeeder.AdminRole))
                .Select(u => u.Id).ToHashSet();

            var users = await query
                .OrderByDescending(u => u.PackageLevel).ThenBy(u => u.UserName)
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    UserName = u.UserName!,
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,
                    PackageLevel = u.PackageLevel,
                    ListenCount = u.ListenHistories.Count
                }).ToListAsync();

            users.ForEach(u => u.IsAdmin = adminIds.Contains(u.Id));
            return users;
        }

        public async Task<bool> UpdatePackageAsync(string userId, PackageLevel package)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.PackageLevel = package;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
