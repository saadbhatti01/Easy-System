using EasySystem.EasyAPI;
using EasySystem.General;
using EasySystem.Models;
using EasySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EasySystem.Controllers
{
    [SessionCheckForAdmin]
    public class QuestionnaireController : Controller
    {
        EasySysAPI _api = new EasySysAPI();
        Common comm = new Common();
        public IActionResult Create()
        {
            List<SkillType> skillsList = new List<SkillType>();
            skillsList = comm.GetAllSkillType();
            SelectList slist = new SelectList(skillsList, "StId", "StName");
            ViewData["Skills"] = slist;

            return View();
        }

        public IActionResult _QuestionStory(int id)
        {
            //Get Question Data
            List<EasySystem.Models.QuestionVM> qList = new List<EasySystem.Models.QuestionVM>();
            qList = comm.GetQuestions(id);
            if (qList.Count > 0)
            {
                TempData["Question"] = qList;
            }
            return PartialView();
        }

        [HttpPost]
        public IActionResult Create(Question_Story data)
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                var Id = (int)HttpContext.Session.GetInt32("ID");
                if (RoleId == 1)
                {
                    data.qStatus = true;
                    data.qCreatedBy = Id;
                }
                else
                {
                    data.qStatus = false;
                    data.qCreatedBy = Id;
                }
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("Questionnaire/AddQuestion", data);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    List<EasySystem.Models.QuestionVM> qList = new List<EasySystem.Models.QuestionVM>();
                    qList = comm.GetQuestions(data.StId);
                    if (qList.Count > 0)
                    {
                        TempData["Question"] = qList;
                    }
                    return PartialView("_QuestionStory");


                    //return RedirectToAction("Create");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Create");
                }
            }

            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Create");
            }

        }

        public IActionResult DeleteQuestion(int id)
        {
            try
            {
                HttpClient clientt = _api.Initial();
                var data = clientt.DeleteAsync("Questionnaire/DelQuestion?id=" + id.ToString());
                data.Wait();
                var result = data.Result;
                if (result.IsSuccessStatusCode)
                {
                    return Content("");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    //TempData["Error"] = "" + errorMsg.message + "";
                    return Content("" + errorMsg.message + "");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return Content("An error occured during getting the request. Please try again later");
            }

        }

        public IActionResult _EditQuestion(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("Questionnaire/EditQuestion?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<Question_Story>(res);

                    List<SkillType> skillsList = new List<SkillType>();
                    skillsList = comm.GetAllSkillType();
                    SelectList slist = new SelectList(skillsList, "StId", "StName");
                    ViewData["Skills"] = slist;

                    return PartialView(data);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    //TempData["Error"] = "" + errorMsg.message + "";
                    //return RedirectToAction("Create");
                    return PartialView();
                }
            }
            catch (Exception)
            {
                //TempData["Error"] = "An error occured during getting the request. Please try again later";
                //return RedirectToAction("Create");
                return PartialView();
            }

        }

        [HttpPost]
        public IActionResult QuestionUpdate(int id, Question_Story data)
        {
            try
            {
                id = data.qId;
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("Questionnaire/UpdateQuestion?id=" + id.ToString(), data);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    return Content("");
                }
                else
                {
                    //var res = result.Content.ReadAsStringAsync().Result;
                    //var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    //TempData["Error"] = "" + errorMsg.message + "";
                    //return RedirectToAction("Create");
                    return Content("Data Not Updated");
                }
            }
            catch (Exception)
            {
                //TempData["Error"] = "An error occured during getting the request. Please try again later";
                //return RedirectToAction("Create");
                return Content("Data Not Updated");
            }
        }
    }
}
