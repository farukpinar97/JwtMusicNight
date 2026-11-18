namespace JwtMusicNight.WebUI.Dtos
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
    }

    public class ApiErrorDto
    {
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}

namespace JwtMusicNight.WebUI.Dtos
{
    // API'nin 403 cevabı
    public class StreamErrorDto
    {
        public string? Message { get; set; }
        public string? UserPackage { get; set; }
        public string? RequiredPackage { get; set; }
    }
}
