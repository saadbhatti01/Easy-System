using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasySystem.Models;
using EasySystemAPI.Models;
using EasySystem.General;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http;
using EasySystem.EasyAPI;

namespace EasySystem.Controllers
{
    [SessionCheckForAdmin]
    public class QuestionsController : Controller
    {
        EasySysAPI _api = new EasySysAPI();
        Common comm = new Common();
        public IActionResult Create()
        {
            List<SkillType> skillsList = new List<SkillType>();
            skillsList = comm.GetSkillTypes();
            SelectList slist = new SelectList(skillsList, "StId", "StName");
            ViewData["Skills"] = slist;

            //Get Question Data
            List<EasySystem.Models.QuestionVM> qList = new List<EasySystem.Models.QuestionVM>();
            qList = comm.GetQuestions();
            if(qList.Count > 0)
            {
                TempData["Question"] = qList;
            }

            return View();
        }
        [HttpPost]
        public IActionResult Create(Question_Story data)
        {
            try
            {
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("Questionnaire/AddQuestion", data);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record added successfuly";
                    return RedirectToAction("Create");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Create");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
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
                    TempData["Success"] = "Record Deleted successfully";
                    return RedirectToAction("Create");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Create");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Create");
            }

        }

        public IActionResult EditQuestion(int id)
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
                    skillsList = comm.GetSkillTypes();
                    SelectList slist = new SelectList(skillsList, "StId", "StName");
                    ViewData["Skills"] = slist;

                    return View(data);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Create");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Create");
            }

        }

        [HttpPost]
        public IActionResult EditQuestion(int id, Question_Story data)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("Questionnaire/UpdateQuestion?id=" + id.ToString(), data);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("Create");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Create");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Create");
            }
        }
    }
}
