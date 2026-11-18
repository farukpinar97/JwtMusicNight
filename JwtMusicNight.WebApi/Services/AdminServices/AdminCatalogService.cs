using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Services.AdminServices
{
    public class AdminCatalogService : IAdminCatalogService
    {
        private readonly JwtContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminCatalogService(JwtContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private bool AudioExists(string? fileName) =>
            !string.IsNullOrEmpty(fileName) &&
            File.Exists(Path.Combine(_env.ContentRootPath, "Musics", fileName));

        // =============== SONGS ===============
        public async Task<List<AdminSongDto>> GetSongsAsync(string? search)
        {
            var query = _context.Songs.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s => s.SongName.Contains(search) || s.Artists.ArtistName.Contains(search));

            var songs = await query
                .OrderBy(s => s.SongId)
                .Select(s => new AdminSongDto
                {
                    SongId = s.SongId,
                    SongName = s.SongName,
                    CoverImageUrl = s.CoverImageUrl,
                    AudioUrl = s.AudioUrl,
                    Duration = s.Duration,
                    ReleaseDate = s.ReleaseDate,
                    RequiredPackage = s.RequiredPackage,
                    ArtistId = s.ArtistId,
                    GenreId = s.GenreId,
                    ArtistName = s.Artists.ArtistName,
                    GenreName = s.Genre.GenreName,
                    ListenCount = s.ListenCount
                }).ToListAsync();

            songs.ForEach(s => s.HasAudioFile = AudioExists(s.AudioUrl));
            return songs;
        }

        public async Task<AdminSongDto?> GetSongAsync(int id)
        {
            var s = await _context.Songs.AsNoTracking()
                .Include(x => x.Artists).Include(x => x.Genre)
                .FirstOrDefaultAsync(x => x.SongId == id);
            if (s == null) return null;

            return new AdminSongDto
            {
                SongId = s.SongId,
                SongName = s.SongName,
                CoverImageUrl = s.CoverImageUrl,
                AudioUrl = s.AudioUrl,
                Duration = s.Duration,
                ReleaseDate = s.ReleaseDate,
                RequiredPackage = s.RequiredPackage,
                Lyrics = s.Lyrics,
                ArtistId = s.ArtistId,
                GenreId = s.GenreId,
                ArtistName = s.Artists.ArtistName,
                GenreName = s.Genre.GenreName,
                ListenCount = s.ListenCount,
                HasAudioFile = AudioExists(s.AudioUrl)
            };
        }

        public async Task<int> CreateSongAsync(AdminSongDto dto)
        {
            var song = new Song
            {
                SongName = dto.SongName,
                CoverImageUrl = dto.CoverImageUrl ?? "/assets/img/c1.jpg",
                AudioUrl = dto.AudioUrl ?? "",
                Duration = dto.Duration,
                ReleaseDate = dto.ReleaseDate,
                RequiredPackage = dto.RequiredPackage,
                Lyrics = dto.Lyrics,
                ArtistId = dto.ArtistId,
                GenreId = dto.GenreId
            };
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();
            return song.SongId;
        }

        public async Task<bool> UpdateSongAsync(int id, AdminSongDto dto)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null) return false;

            song.SongName = dto.SongName;
            song.CoverImageUrl = dto.CoverImageUrl ?? song.CoverImageUrl;
            song.Duration = dto.Duration;
            song.ReleaseDate = dto.ReleaseDate;
            song.RequiredPackage = dto.RequiredPackage;
            song.Lyrics = dto.Lyrics;
            song.ArtistId = dto.ArtistId;
            song.GenreId = dto.GenreId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> DeleteSongAsync(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null) return null;

            var fileName = song.AudioUrl;
            _context.Songs.Remove(song);                  // ListenHistories cascade ile silinir
            await _context.SaveChangesAsync();
            return fileName ?? "";
        }

        public async Task<bool> SetAudioFileAsync(int id, string fileName)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null) return false;
            song.AudioUrl = fileName;
            await _context.SaveChangesAsync();
            return true;
        }

        // =============== ARTISTS ===============
        public Task<List<AdminArtistDto>> GetArtistsAsync() =>
            _context.Artists.AsNoTracking()
                .OrderBy(a => a.ArtistName)
                .Select(a => new AdminArtistDto
                {
                    ArtistId = a.ArtistId,
                    ArtistName = a.ArtistName,
                    ArtistImageUrl = a.ArtistImageUrl,
                    CoverImageUrl = a.CoverImageUrl,
                    Bio = a.Bio,
                    Country = a.Country,
                    IsVerified = a.IsVerified,
                    SongCount = a.Songs.Count
                }).ToListAsync();

        public async Task<AdminArtistDto?> GetArtistAsync(int id) =>
            (await GetArtistsAsync()).FirstOrDefault(a => a.ArtistId == id);

        public async Task<int> CreateArtistAsync(AdminArtistDto dto)
        {
            var artist = new Artist
            {
                ArtistName = dto.ArtistName,
                ArtistImageUrl = dto.ArtistImageUrl ?? "/assets/img/b1.jpg",
                CoverImageUrl = dto.CoverImageUrl ?? "/assets/img/b11.jpg",
                Bio = dto.Bio ?? "",
                Country = dto.Country ?? "",
                IsVerified = dto.IsVerified,
                CreatedDate = DateTime.Now
            };
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            return artist.ArtistId;
        }

        public async Task<bool> UpdateArtistAsync(int id, AdminArtistDto dto)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return false;

            artist.ArtistName = dto.ArtistName;
            artist.ArtistImageUrl = dto.ArtistImageUrl ?? artist.ArtistImageUrl;
            artist.CoverImageUrl = dto.CoverImageUrl ?? artist.CoverImageUrl;
            artist.Bio = dto.Bio ?? "";
            artist.Country = dto.Country ?? "";
            artist.IsVerified = dto.IsVerified;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool ok, string? error)> DeleteArtistAsync(int id)
        {
            var artist = await _context.Artists.Include(a => a.Songs).FirstOrDefaultAsync(a => a.ArtistId == id);
            if (artist == null) return (false, "Sanatçı bulunamadı.");
            if (artist.Songs.Any())
                return (false, $"Bu sanatçının {artist.Songs.Count} şarkısı var. Önce şarkıları silin veya başka sanatçıya taşıyın.");

            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        // =============== GENRES ===============
        public Task<List<AdminGenreDto>> GetGenresAsync() =>
            _context.Genres.AsNoTracking()
                .OrderBy(g => g.GenreName)
                .Select(g => new AdminGenreDto
                {
                    GenreId = g.GenreId,
                    GenreName = g.GenreName,
                    ImageUrl = g.ImageUrl,
                    SongCount = g.Songs.Count
                }).ToListAsync();

        public async Task<AdminGenreDto?> GetGenreAsync(int id) =>
            (await GetGenresAsync()).FirstOrDefault(g => g.GenreId == id);

        public async Task<int> CreateGenreAsync(AdminGenreDto dto)
        {
            var genre = new Genre { GenreName = dto.GenreName, ImageUrl = dto.ImageUrl };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            return genre.GenreId;
        }

        public async Task<bool> UpdateGenreAsync(int id, AdminGenreDto dto)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return false;
            genre.GenreName = dto.GenreName;
            genre.ImageUrl = dto.ImageUrl;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool ok, string? error)> DeleteGenreAsync(int id)
        {
            var genre = await _context.Genres.Include(g => g.Songs).FirstOrDefaultAsync(g => g.GenreId == id);
            if (genre == null) return (false, "Tür bulunamadı.");
            if (genre.Songs.Any())
                return (false, $"Bu türde {genre.Songs.Count} şarkı var. Önce şarkıların türünü değiştirin.");

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}
