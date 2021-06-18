using Microsoft.AspNetCore.Mvc;

namespace EasySystem.Controllers
{
    public class SignUpController : Controller
    {
        public IActionResult M(string C)
        {
            if (C != null)
            {
                string code = C;
                TempData["Info"] = "Please register your phone number first";
                return RedirectToAction("SignUp", "Users", new { code });
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }

        }
    }
}