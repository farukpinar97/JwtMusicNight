using System.Security.Claims;
using System.Text;

namespace JwtMusicNight.WebUI.Models
{
    // Kullanıcı adına göre sabit avatar (avatar1-7.png)
    public static class AvatarHelper
    {
        private const int AvatarCount = 7;
        private const string Salt = "mn152:";

        public static string For(string? userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return "/assets/img/avatars/avatar1.png";

            // FNV-1a (kararlı hash)
            uint hash = 2166136261;
            foreach (var b in Encoding.UTF8.GetBytes(Salt + userName.ToLowerInvariant()))
            {
                hash ^= b;
                hash *= 16777619;
            }
            return $"/assets/img/avatars/avatar{hash % AvatarCount + 1}.png";
        }

        public static string For(ClaimsPrincipal user) => For(user.Identity?.Name);
    }
}
