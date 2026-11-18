using AutoMapper;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;

namespace JwtMusicNight.WebApi.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            // Song
            CreateMap<Song, ResultSongDto>()
                .ForMember(d => d.ArtistName, o => o.MapFrom(s => s.Artists.ArtistName))
                .ForMember(d => d.ArtistImageUrl, o => o.MapFrom(s => s.Artists.ArtistImageUrl))
                .ForMember(d => d.ArtistCoverImageUrl, o => o.MapFrom(s => s.Artists.CoverImageUrl))
                .ForMember(d => d.GenreName, o => o.MapFrom(s => s.Genre.GenreName));
            CreateMap<CreateSongDto, Song>();

            // Artist
            CreateMap<Artist, ResultArtistDto>()
                .ForMember(d => d.SongCount, o => o.MapFrom(s => s.Songs.Count));
            CreateMap<Artist, ResultArtistDetailDto>()
                .ForMember(d => d.SongCount, o => o.MapFrom(s => s.Songs.Count))
                .ForMember(d => d.Songs, o => o.Ignore());   // serviste dolduruluyor
            CreateMap<CreateArtistDto, Artist>();

            // Genre
            CreateMap<Genre, ResultGenreDto>()
                .ForMember(d => d.SongCount, o => o.MapFrom(s => s.Songs.Count));
        }
    }
}