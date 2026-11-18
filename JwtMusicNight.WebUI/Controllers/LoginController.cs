using JwtMusicNight.WebUI.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace JwtMusicNight.WebUI.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Music");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginDto dto, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = _httpClientFactory.CreateClient("MusicApi");
            var response = await client.PostAsJsonAsync("api/Login", dto);

            if (response.StatusCode == HttpStatusCode.Unauthorized || !response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View(dto);
            }

            var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
            if (string.IsNullOrEmpty(result?.Token))
            {
                ModelState.AddModelError("", "Token alınamadı.");
                return View(dto);
            }

            // Token'ı çöz, içindeki claim'leri cookie oturumuna aktar
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
            // Claim adı kısa veya URI biçiminde olabilir
            string Claim(params string[] types) =>
                jwt.Claims.FirstOrDefault(c => types.Contains(c.Type))?.Value ?? "";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Claim("nameid", ClaimTypes.NameIdentifier)),
                new Claim(ClaimTypes.Name, Claim("unique_name", ClaimTypes.Name)),     // kullanıcı adı
                new Claim(ClaimTypes.GivenName, Claim("given_name", ClaimTypes.GivenName)),
                new Claim(ClaimTypes.Surname, Claim("family_name", ClaimTypes.Surname)),
                new Claim("package", Claim("package")),
                new Claim("packageLevel", Claim("packageLevel")),
                new Claim("access_token", result.Token)               // API çağrılarında kullanılacak
            };

            // Rolleri cookie'ye aktar
            var roles = jwt.Claims
                .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .Distinct()
                .ToList();
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = jwt.ValidTo        // cookie, token ile aynı anda biter
                });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Admin doğrudan yönetim paneline
            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return RedirectToAction("Index", "Music");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = _httpClientFactory.CreateClient("MusicApi");
            var response = await client.PostAsJsonAsync("api/Register", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorDto>();
                foreach (var e in error?.Errors ?? new List<string> { "Kayıt sırasında bir hata oluştu." })
                    ModelState.AddModelError("", e);
                return View(dto);
            }

            TempData["Success"] = "Kayıt başarılı! Basic paketle giriş yapabilirsiniz.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}
