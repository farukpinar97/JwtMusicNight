using System.Security.Claims;

namespace JwtMusicNight.WebUI.Models
{
    // Paket adı, seviye ve rozet rengi (yetki kontrolü API'de)
    public static class PackageHelper
    {
        private static readonly Dictionary<string, int> Levels = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Basic"] = 1, ["Gold"] = 2, ["Premium"] = 3, ["Elit"] = 4
        };

        public static int ToLevel(string? package)
            => package != null && Levels.TryGetValue(package, out var l) ? l : 1;

        public static int UserLevel(ClaimsPrincipal user)
            => int.TryParse(user.FindFirst("packageLevel")?.Value, out var l) ? l : 1;

        public static string UserPackage(ClaimsPrincipal user)
            => user.FindFirst("package")?.Value ?? "Basic";

        public static bool CanListen(ClaimsPrincipal user, string songPackage)
            => UserLevel(user) >= ToLevel(songPackage);

        public static string BadgeCss(string? package) => package switch
        {
            "Gold" => "badge-warning",
            "Premium" => "badge-info",
            "Elit" => "badge-danger",
            _ => "badge-secondary"
        };
    }
}
