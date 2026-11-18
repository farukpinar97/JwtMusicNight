using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace JwtMusicNight.WebUI.Controllers
{
    // API çağıran controller'ların tabanı; 401'de oturumu kapatır.
    public abstract class MusicBaseController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        protected MusicBaseController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected HttpClient Api => _httpClientFactory.CreateClient("MusicApi");

        // GET isteği atar; 404'te null döner, 401'de oturumu kapatır
        protected async Task<T?> GetAsync<T>(string url)
        {
            var response = await Api.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiUnauthorizedException();

            if (response.StatusCode == HttpStatusCode.NotFound || !response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ApiUnauthorizedException)
            {
                context.Result = RedirectToAction("Logout", "Login");
                context.ExceptionHandled = true;
            }
            base.OnActionExecuted(context);
        }
    }

    public class ApiUnauthorizedException : Exception { }
}
