using JwtMusicNight.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebUI.Areas.Admin.Controllers
{
    public class UsersController : AdminBaseController
    {
        public UsersController(IHttpClientFactory f, IWebHostEnvironment env) : base(f, env) { }

        public async Task<IActionResult> Index(string? search, string? package)
        {
            var url = $"api/admin/users?search={Uri.EscapeDataString(search ?? "")}" +
                      (string.IsNullOrEmpty(package) ? "" : $"&package={package}");

            ViewBag.Search = search;
            ViewBag.Package = package;
            return View(await GetAsync<List<AdminUserDto>>(url) ?? new());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePackage(string id, string packageLevel, string? search, string? package)
        {
            var (ok, result) = await SendAsync(HttpMethod.Put, $"api/admin/users/{id}/package", new { packageLevel });
            if (ok) Success(result?.Message); else Error(result?.Message);
            return RedirectToAction(nameof(Index), new { search, package });
        }
    }
}
