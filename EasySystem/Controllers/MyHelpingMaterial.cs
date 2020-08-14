using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EasySystem.EasyAPI;
using EasySystem.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EasySystem.Controllers
{
    public class MyHelpingMaterial : Controller
    {
        EasySysAPI _api = new EasySysAPI();

        public IActionResult Index()
        {
            try
            {
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("MyHelpingMaterial/GetMyHelpingMaterial");
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<UserHelpingMaterial>>(res);
                    TempData["HelpingMaterial"] = list;
                    return View();
                }

                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return View();
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                //return View();
            }
            return View();
        }


        public IActionResult Single(string Path)
        {
            try
            {
                if(Path != null && Path != "")
                {
                    TempData["Path"] = Path;
                }

                return View();

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                //return View();
            }
            return View();
        }
    }
}