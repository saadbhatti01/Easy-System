using EasySystem.EasyAPI;
using EasySystem.General;
using EasySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EasySystem.Controllers
{
    public class CoursesController : Controller
    {
        EasySysAPI _api = new EasySysAPI();
        Common com = new Common();


        public IActionResult Index()
        {
            return View();
        }

        public ActionResult _Categories(int Value)
        {
            try
            {
                HttpClient client = _api.Initial();
                Task<HttpResponseMessage> Data;
                Data = client.GetAsync("My/GetMoreSkillType?value=" + Value.ToString());
                Data.Wait();
                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var List = JsonConvert.DeserializeObject<List<SkillType>>(res);
                    if (List.Count == 0)
                    {
                        return PartialView("_Categories", "");
                    }
                    TempData["List"] = List;
                }
                else
                {
                    return PartialView();
                    //var res = result.Content.ReadAsStringAsync().Result;
                    //var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    //TempData["Error"] = "" + errorMsg.message + "";
                }
            }

            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
            }
            return PartialView();
        }

        public IActionResult List(int id, string Category)
        {
            TempData["Category"] = Category;
            TempData["SkillId"] = id;
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("My/EditSkillType?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<SkillType>(res);
                    TempData["Detail"] = data;

                    bool isAvailable = com.CheckTrainingDetails(id);
                    if (isAvailable)
                    {
                        TempData["isAvailable"] = isAvailable;
                    }

                    bool isQuestionnaire = com.CheckQuestionnaire(id);
                    if (isQuestionnaire)
                    {
                        TempData["Questionnaire"] = isQuestionnaire;
                    }

                    return View();
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    //var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    //TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Index");
                }

            }

            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Index");
            }
        }

        public ActionResult _SkillsList(int Value, int Id)
        {
            try
            {
                PageModel data = new PageModel();
                data.Count = Value;
                data.id = Id;
                HttpClient client = _api.Initial();
                Task<HttpResponseMessage> Data;
                Data = client.PostAsJsonAsync("My/GetSkillsData", data);
                Data.Wait();
                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var List = JsonConvert.DeserializeObject<List<SkillType>>(res);
                    if (List.Count == 0)
                    {
                        return PartialView("_SkillsList", "");
                    }
                    TempData["List"] = List;
                }
                else
                {
                    return PartialView();
                }
            }

            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
            }
            return PartialView();
        }

        public IActionResult Material(int id, string topic)
        {
            try
            {
                var Id = HttpContext.Session.GetInt32("ID");
                if (Id != null)
                {
                    HttpClient client = _api.Initial();
                    var data = client.GetAsync("Skills/GetSkillMaterial?id=" + id.ToString());
                    data.Wait();
                    var result = data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var getdata = JsonConvert.DeserializeObject<List<SkillMaterial>>(res);

                        if (getdata.Count > 0)
                        {
                            var getStId = getdata.Select(g => g.StId).FirstOrDefault();
                            TempData["Material"] = getdata;
                            List<SkillMaterialDetail> detail = new List<SkillMaterialDetail>();
                            detail = com.GetSkillMaterialDetail(getStId);
                            if (detail.Count > 0)
                            {
                                TempData["MaterialDetail"] = detail;
                            }
                        }

                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        //var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        //TempData["Error"] = "" + errorMsg.message + "";
                        return RedirectToAction("Trainings");
                    }
                }
                else
                {
                    TempData["Info"] = "Please Login to continue";
                    return RedirectToAction("Logout", "Users");
                }
            }
            catch (Exception)
            {
            }
            return View();
        }

        public ActionResult _MentorData(int Value, int Id)
        {
            try
            {
                PageModel data = new PageModel();
                data.Count = Value;
                data.id = Id;
                HttpClient client = _api.Initial();
                Task<HttpResponseMessage> Data;
                Data = client.PostAsJsonAsync("My/GetMentorData", data);
                Data.Wait();
                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var List = JsonConvert.DeserializeObject<List<Users>>(res);
                    if (List.Count == 0)
                    {
                        return Content("");
                    }
                    TempData["mList"] = List;
                    return PartialView();
                }
                else
                {
                    return Content("");
                }
            }

            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return Content("");
            }

        }

        public ActionResult CheckSession()
        {
            var Id = HttpContext.Session.GetInt32("ID");
            if (Id != null)
            {
                return Json(new { Success = "true" });
            }
            else
            {
                return Json(new { Success = "false" });
            }

        }


        public ActionResult All()
        {
            try
            {
                var List = com.GetSkillTypeList();
                TempData["List"] = List;
            }
            catch (Exception)
            {

            }
            return View();
        }

        public IActionResult Search()
        {
            try
            {
                var List = com.GetSearchCourses();
                if (List.Count > 0)
                {
                    TempData["List"] = List;
                }
            }
            catch (Exception)
            {

            }

            return View();

        }

        public JsonResult SearchCourse(string term)
        {
            List<string> msg = new List<string> { "No Record found" };
            try
            {
                List<string> AutoCourse;
                HttpClient client = _api.Initial();
                Task<HttpResponseMessage> Data;
                Data = client.GetAsync("Skills/SearchForCourse?term=" + term.ToString());
                Data.Wait();
                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    AutoCourse = JsonConvert.DeserializeObject<List<string>>(res);
                    if (AutoCourse.Count == 0 || AutoCourse == null)
                    {
                        return Json(msg);
                    }
                    else
                    {
                        return Json(AutoCourse);
                    }
                }
                else
                {
                    return Json(msg);
                }
            }
            catch (Exception)
            {
                return Json(msg);
            }

        }

        public IActionResult Get(string Course)
        {

            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("Skills/SearchedSkill?Course=" + Course.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<SkillType>(res);
                    if (data != null)
                    {
                        return RedirectToAction("List", new { id = data.StId, Category = data.StName });
                    }
                    else
                    {
                        TempData["Info"] = "No record found";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    //var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    //TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Index");
                }

            }

            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Index");
            }
        }
    }



}
