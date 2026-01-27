using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyShowWeb.Pages
{
    public class ClearCacheModel : PageModel
    {
        public void OnGet()
        {
            // Clear all cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            
            // Set cache control headers
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";
            Response.Headers["Clear-Site-Data"] = "\"cache\", \"cookies\", \"storage\"";
        }
    }
}
