using JwtMusicNight.WebApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Context
{
    public class JwtContext : IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-BUFC83E\\SQLEXPRESS;initial catalog=JwtMusicNightDB;integrated security=true;TrustServerCertificate=True");
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ListenHistory> ListenHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity tabloları için şart

            builder.Entity<Song>()
                .HasOne(s => s.Artists).WithMany(a => a.Songs)
                .HasForeignKey(s => s.ArtistId);

            builder.Entity<Song>()
                .HasOne(s => s.Genre).WithMany(g => g.Songs)
                .HasForeignKey(s => s.GenreId);

            builder.Entity<ListenHistory>()
                .HasOne(l => l.AppUser).WithMany(u => u.ListenHistories)
                .HasForeignKey(l => l.AppUserId);

            builder.Entity<ListenHistory>()
                .HasOne(l => l.Song).WithMany(s => s.ListenHistories)
                .HasForeignKey(l => l.SongId);
        }
    }
}
