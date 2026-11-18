using JwtMusicNight.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebUI.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        public DashboardController(IHttpClientFactory f, IWebHostEnvironment env) : base(f, env) { }

        public async Task<IActionResult> Index()
        {
            var dashboard = await GetAsync<AdminDashboardDto>("api/admin/dashboard") ?? new();
            return View(dashboard);
        }
    }
}
