using AutoMapper;
using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Services.GenreServices
{
    public class GenreService : IGenreService
    {
        private readonly JwtContext _context;
        private readonly IMapper _mapper;

        public GenreService(JwtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ResultGenreDto>> GetAllGenresAsync()
        {
            var values = await _context.Genres
                .AsNoTracking()
                .Include(g => g.Songs)
                .OrderBy(g => g.GenreName)
                .ToListAsync();
            return _mapper.Map<List<ResultGenreDto>>(values);
        }

        public async Task<ResultGenreDto?> GetGenreByIdAsync(int id)
        {
            var value = await _context.Genres
                .AsNoTracking()
                .Include(g => g.Songs)
                .FirstOrDefaultAsync(g => g.GenreId == id);
            return value == null ? null : _mapper.Map<ResultGenreDto>(value);
        }
    }
}
