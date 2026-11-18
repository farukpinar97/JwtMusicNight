using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Context
{
    // Veritabanı boşsa örnek tür, sanatçı, şarkı, kullanıcı ve dinleme geçmişi ekler.
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<JwtContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            await context.Database.MigrateAsync();

            await SeedGenresAsync(context);
            await SeedArtistsAsync(context);
            await SeedSongsAsync(context);
            await SeedUsersAsync(userManager);
            await SeedListenHistoryAsync(context, userManager);
        }

        // ---------------- GENRES ----------------
        private static async Task SeedGenresAsync(JwtContext context)
        {
            if (await context.Genres.AnyAsync()) return;

            context.Genres.AddRange(
                new Genre { GenreName = "Pop",        ImageUrl = "/assets/img/a1.jpg" },
                new Genre { GenreName = "Rock",       ImageUrl = "/assets/img/a2.jpg" },
                new Genre { GenreName = "Rap",        ImageUrl = "/assets/img/a3.jpg" },
                new Genre { GenreName = "Jazz",       ImageUrl = "/assets/img/a4.jpg" },
                new Genre { GenreName = "Electronic", ImageUrl = "/assets/img/a5.jpg" },
                new Genre { GenreName = "R&B",        ImageUrl = "/assets/img/a6.jpg" },
                new Genre { GenreName = "Classical",  ImageUrl = "/assets/img/a7.jpg" }
            );
            await context.SaveChangesAsync();
        }

        // ---------------- ARTISTS ----------------
        private static async Task SeedArtistsAsync(JwtContext context)
        {
            if (await context.Artists.AnyAsync()) return;

            context.Artists.AddRange(
                CreateArtist("Dua Lipa", "İngiltere", "Londra doğumlu pop şarkıcısı ve söz yazarı.", 1),
                CreateArtist("5 Seconds of Summer", "Avustralya", "Sidney çıkışlı pop-rock grubu.", 2),
                CreateArtist("DJ Snake", "Fransa", "Fransız DJ ve prodüktör; elektronik ve hip-hop karışımlarıyla tanınır.", 3),
                CreateArtist("Imagine Dragons", "ABD", "Las Vegas çıkışlı rock grubu.", 4),
                CreateArtist("Eminem", "ABD", "Detroit'li rapçi, söz yazarı ve prodüktör.", 5),
                CreateArtist("Norah Jones", "ABD", "Caz, pop ve soul'u harmanlayan şarkıcı ve piyanist.", 6),
                CreateArtist("The Weeknd", "Kanada", "Torontolu R&B ve pop sanatçısı.", 7),
                CreateArtist("Ludovico Einaudi", "İtalya", "İtalyan besteci ve piyanist; minimalist klasik eserleriyle bilinir.", 8),
                CreateArtist("Avicii", "İsveç", "İsveçli DJ ve prodüktör; elektronik dans müziğinin öncü isimlerinden.", 9)
            );
            await context.SaveChangesAsync();
        }

        private static Artist CreateArtist(string name, string country, string bio, int img) => new Artist
        {
            ArtistName = name,
            Country = country,
            Bio = bio,
            ArtistImageUrl = $"/assets/img/b{img}.jpg",
            CoverImageUrl = $"/assets/img/b{img + 10}.jpg",
            CreatedDate = DateTime.Now,
            IsVerified = true
        };

        // ---------------- SONGS (20 adet) ----------------
        private static async Task SeedSongsAsync(JwtContext context)
        {
            if (await context.Songs.AnyAsync()) return;

            var artists = await context.Artists.ToDictionaryAsync(a => a.ArtistName, a => a.ArtistId);
            var genres = await context.Genres.ToDictionaryAsync(g => g.GenreName, g => g.GenreId);

            // (Şarkı, Sanatçı, Tür, Paket, Süre "dk:sn", Yıl)
            var data = new (string Song, string Artist, string Genre, PackageLevel Package, string Duration, int Year)[]
            {
                // BASIC (5)
                ("Levitating",        "Dua Lipa",            "Pop",        PackageLevel.Basic,   "3:23", 2020),
                ("Youngblood",        "5 Seconds of Summer", "Pop",        PackageLevel.Basic,   "3:23", 2018),
                ("Taki Taki",         "DJ Snake",            "Electronic", PackageLevel.Basic,   "3:32", 2018),
                ("Blinding Lights",   "The Weeknd",          "R&B",        PackageLevel.Basic,   "3:20", 2019),
                ("Wake Me Up",        "Avicii",              "Electronic", PackageLevel.Basic,   "4:07", 2013),

                // GOLD (5)
                ("Don't Start Now",   "Dua Lipa",            "Pop",        PackageLevel.Gold,    "3:03", 2019),
                ("Let Me Love You",   "DJ Snake",            "Electronic", PackageLevel.Gold,    "3:25", 2016),
                ("Believer",          "Imagine Dragons",     "Rock",       PackageLevel.Gold,    "3:24", 2017),
                ("Don't Know Why",    "Norah Jones",         "Jazz",       PackageLevel.Gold,    "3:06", 2002),
                ("Experience",        "Ludovico Einaudi",    "Classical",  PackageLevel.Gold,    "5:15", 2013),

                // PREMIUM (5)
                ("Teeth",             "5 Seconds of Summer", "Pop",        PackageLevel.Premium, "3:24", 2019),
                ("Thunder",           "Imagine Dragons",     "Rock",       PackageLevel.Premium, "3:07", 2017),
                ("Lose Yourself",     "Eminem",              "Rap",        PackageLevel.Premium, "5:26", 2002),
                ("Save Your Tears",   "The Weeknd",          "R&B",        PackageLevel.Premium, "3:35", 2020),
                ("Levels",            "Avicii",              "Electronic", PackageLevel.Premium, "3:19", 2011),

                // ELIT (5)
                ("Radioactive",       "Imagine Dragons",     "Rock",       PackageLevel.Elit,    "3:06", 2012),
                ("Without Me",        "Eminem",              "Rap",        PackageLevel.Elit,    "4:50", 2002),
                ("Come Away With Me", "Norah Jones",         "Jazz",       PackageLevel.Elit,    "3:18", 2002),
                ("Nuvole Bianche",    "Ludovico Einaudi",    "Classical",  PackageLevel.Elit,    "5:57", 2004),
                ("Lean On",           "DJ Snake",            "Electronic", PackageLevel.Elit,    "2:56", 2015),
            };

            var songs = data.Select((d, i) => new Song
            {
                SongName = d.Song,
                ArtistId = artists[d.Artist],
                GenreId = genres[d.Genre],
                RequiredPackage = d.Package,
                Duration = TimeSpan.ParseExact(d.Duration, @"m\:ss", null),
                ReleaseDate = new DateTime(d.Year, 1, 1),
                CoverImageUrl = $"/assets/img/c{i + 1}.jpg",
                AudioUrl = $"song{(i + 1):00}.mp3",   // WebApi/Musics/song01.mp3 ... song20.mp3
                ListenCount = 0
            });

            context.Songs.AddRange(songs);
            await context.SaveChangesAsync();
        }

        // ---------------- TEST KULLANICILARI ----------------
        // Şifre hepsi için: Password123*
        private static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            var users = new[]
            {
                ("basicuser",   "Basic",   PackageLevel.Basic),
                ("golduser",    "Gold",    PackageLevel.Gold),
                ("premiumuser", "Premium", PackageLevel.Premium),
                ("elituser",    "Elit",    PackageLevel.Elit),
            };

            foreach (var (username, name, package) in users)
            {
                if (await userManager.FindByNameAsync(username) != null) continue;

                var user = new AppUser
                {
                    UserName = username,
                    Email = $"{username}@musicnight.com",
                    Name = name,
                    Surname = "Kullanıcı",
                    PackageLevel = package
                };
                await userManager.CreateAsync(user, "Password123*");
            }
        }

        // ---------------- ÖRNEK DİNLEME GEÇMİŞİ ----------------
        private static async Task SeedListenHistoryAsync(JwtContext context, UserManager<AppUser> userManager)
        {
            if (await context.ListenHistories.AnyAsync()) return;

            var songIds = await context.Songs.ToDictionaryAsync(s => s.SongName, s => s.SongId);

            var history = new Dictionary<string, string[]>
            {
                ["golduser"]    = new[] { "Youngblood", "Taki Taki", "Believer" },
                ["premiumuser"] = new[] { "Youngblood", "Taki Taki", "Lose Yourself" },
                ["elituser"]    = new[] { "Youngblood", "Taki Taki", "Radioactive", "Levels" },
            };

            foreach (var (username, songNames) in history)
            {
                var user = await userManager.FindByNameAsync(username);
                if (user == null) continue;

                foreach (var songName in songNames)
                {
                    context.ListenHistories.Add(new ListenHistory
                    {
                        AppUserId = user.Id,
                        SongId = songIds[songName],
                        ListenedAt = DateTime.Now.AddDays(-Random.Shared.Next(1, 30))
                    });
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
