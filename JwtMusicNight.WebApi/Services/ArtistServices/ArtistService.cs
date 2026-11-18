using AutoMapper;
using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Services.ArtistServices
{
    public class ArtistService : IArtistService
    {
        private readonly JwtContext _context;
        private readonly IMapper _mapper;

        public ArtistService(JwtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateArtistAsync(CreateArtistDto createArtistDto)
        {
            var value = _mapper.Map<Artist>(createArtistDto);
            await _context.Artists.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ResultArtistDto>> GetAllArtistsAsync()
        {
            var values = await _context.Artists
                .AsNoTracking()
                .Include(a => a.Songs)          // SongCount için
                .OrderBy(a => a.ArtistName)
                .ToListAsync();
            return _mapper.Map<List<ResultArtistDto>>(values);
        }

        public async Task<ResultArtistDetailDto?> GetArtistByIdAsync(int id)
        {
            var artist = await _context.Artists
                .AsNoTracking()
                .Include(a => a.Songs).ThenInclude(s => s.Genre)
                .FirstOrDefaultAsync(a => a.ArtistId == id);

            if (artist == null) return null;

            var dto = _mapper.Map<ResultArtistDetailDto>(artist);

            dto.Songs = _mapper.Map<List<ResultSongDto>>(artist.Songs
                .OrderBy(s => s.RequiredPackage)
                .ThenBy(s => s.SongName));

            // Şarkılara sanatçı adını ata
            dto.Songs.ForEach(s =>
            {
                s.ArtistName = artist.ArtistName;
                s.ArtistImageUrl = artist.ArtistImageUrl;
                s.ArtistCoverImageUrl = artist.CoverImageUrl;
            });
            dto.TotalListenCount = artist.Songs.Sum(s => s.ListenCount);

            return dto;
        }
    }
}
