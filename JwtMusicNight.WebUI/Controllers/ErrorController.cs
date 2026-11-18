using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebUI.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // /Error/404, /Error/500 ... (Program.cs: UseStatusCodePagesWithReExecute)
        [Route("Error/{code:int}")]
        public IActionResult Index(int code)
        {
            Response.StatusCode = code;
            ViewData["Code"] = code;
            return View();
        }
    }
}
