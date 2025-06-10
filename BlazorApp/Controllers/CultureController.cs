using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Controllers;

[Route("[controller]/[action]")]
public class CultureController : Controller
{
    [HttpGet]
    public IActionResult Set(string culture, string redirectUri)
    {
        if (!string.IsNullOrEmpty(culture))
        {
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(culture, culture));
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                cookieValue,
                new CookieOptions { Path = "/", Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }

        return LocalRedirect(redirectUri);
    }
}
