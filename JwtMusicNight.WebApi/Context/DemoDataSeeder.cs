using JwtMusicNight.WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Context
{
    // ML.NET önerileri için örnek dinleyiciler ve dinleme geçmişi ekler (bir kez çalışır).
    public static class DemoDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<JwtContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            if (await userManager.FindByNameAsync("listener01") != null) return;

            var songs = await context.Songs.Include(s => s.Genre).ToListAsync();
            var rnd = new Random(42);

            var profiles = new[]
            {
                new[] { "Pop", "R&B" },
                new[] { "Rock", "Rap" },
                new[] { "Electronic", "Pop" },
                new[] { "Jazz", "Classical" },
                new[] { "Rap", "Electronic" }
            };

            for (int i = 1; i <= 25; i++)
            {
                var user = new AppUser
                {
                    UserName = $"listener{i:00}",
                    Email = $"listener{i:00}@musicnight.com",
                    Name = "Dinleyici",
                    Surname = i.ToString("00"),
                    PackageLevel = PackageLevel.Elit
                };
                var result = await userManager.CreateAsync(user, "Password123*");
                if (!result.Succeeded) continue;

                var profile = profiles[i % profiles.Length];

                // Zevkine uygun 3-5 şarkı + 1 rastgele şarkı
                var picks = songs.Where(s => profile.Contains(s.Genre.GenreName))
                                 .OrderBy(_ => rnd.Next()).Take(rnd.Next(3, 6)).ToList();
                picks.Add(songs[rnd.Next(songs.Count)]);

                // Youngblood dinleyenlerin çoğu Taki Taki'yi de dinler
                if (profile.Contains("Pop"))
                {
                    picks.Add(songs.First(s => s.SongName == "Youngblood"));
                    if (rnd.NextDouble() < 0.8) picks.Add(songs.First(s => s.SongName == "Taki Taki"));
                }

                foreach (var song in picks.DistinctBy(s => s.SongId))
                {
                    for (int k = 0; k < rnd.Next(1, 4); k++)
                    {
                        context.ListenHistories.Add(new ListenHistory
                        {
                            AppUserId = user.Id,
                            SongId = song.SongId,
                            ListenedAt = DateTime.Now.AddDays(-rnd.Next(1, 60)).AddMinutes(-rnd.Next(0, 1440))
                        });
                    }
                }
            }

            await context.SaveChangesAsync();

            // ListenCount'u dinleme geçmişiyle senkronla
            await context.Database.ExecuteSqlRawAsync(
                "UPDATE Songs SET ListenCount = (SELECT COUNT(*) FROM ListenHistories h WHERE h.SongId = Songs.SongId)");
        }
    }
}
