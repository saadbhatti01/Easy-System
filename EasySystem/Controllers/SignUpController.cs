using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EasySystem.Controllers
{
    public class SignUpController : Controller
    {
        public IActionResult M(string C)
        {
            if(C != null)
            {
                string code = C;
                TempData["Info"] = "Please register you Phone Number first";
                return RedirectToAction("SignUp", "Users", new { code });
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }
           
        }
    }
}