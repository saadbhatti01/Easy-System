using EasySystem.EasyAPI;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace EasySystem.Controllers
{
    public class GuidanceController : Controller
    {
        EasySysAPI _api = new EasySysAPI();
        public IActionResult Index()
        {
            List<EasySystem.Models.UserHelpingMaterial> DataList = new List<EasySystem.Models.UserHelpingMaterial>();
            DataList = GetHelpingMaterial();
            if (DataList.Count > 0)
            {
                TempData["List"] = DataList;
            }
            return View();
        }

        public List<EasySystem.Models.UserHelpingMaterial> GetHelpingMaterial()
        {
            List<EasySystem.Models.UserHelpingMaterial> DataList = new List<EasySystem.Models.UserHelpingMaterial>();
            HttpClient client = _api.Initial();
            var Data = client.GetAsync("Public/GetHelpingMaterialInfo");
            Data.Wait();
            var result = Data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                DataList = JsonConvert.DeserializeObject<List<EasySystem.Models.UserHelpingMaterial>>(res);
            }
            return DataList;
        }
    }
}
