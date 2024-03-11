using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Shift_Tech.Models.Localization;

namespace Shift_Tech.Controllers;
public class LanguageController : Controller
{
    public LanguageController()
    {
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public IActionResult ChangeLanguage([FromBody] string selectedLanguage)
    {
        // Store the selected language in a cookie for persistence.
        Response.Cookies.Append("SelectedLanguage", selectedLanguage, new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddYears(1),
            Path = "/",
            HttpOnly = false, 
        });
        CultureInfo.CurrentCulture = new CultureInfo(selectedLanguage);
        CultureInfo.CurrentUICulture = new CultureInfo(selectedLanguage);

        return Ok();
    }


}
