using System.ComponentModel.DataAnnotations;

namespace JwtMusicNight.WebUI.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        public string Password { get; set; }
    }
}
