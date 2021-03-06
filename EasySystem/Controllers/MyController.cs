using EasySystem.EasyAPI;
using EasySystem.General;
using EasySystem.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace EasySystem.Controllers
{

    public class MyController : Controller
    {
        EasySysAPI _api = new EasySysAPI();
        Common comm = new Common();
        private readonly IHostingEnvironment HostingEnvironment;
        private readonly IDataProtector protector;

        public MyController(IDataProtectionProvider dataProtectionProvider
            , DataProtection dataProtection, IHostingEnvironment hostingEnvironment)
        {
            this.HostingEnvironment = hostingEnvironment;
            protector = dataProtectionProvider.CreateProtector(dataProtection.UserId);

        }

        public IActionResult Profile(string id)
        {
            try
            {
                if (id == null)
                {
                    TempData["SearchProfile"] = "SearchProfile";
                    var Id = HttpContext.Session.GetInt32("ID");

                    HttpClient client = _api.Initial();
                    var GetBank = client.GetAsync("My/GetMyProfile?id=" + Id.ToString());
                    GetBank.Wait();
                    var result = GetBank.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var User = JsonConvert.DeserializeObject<Users>(res);
                        TempData["User"] = User;

                        //UserSkills
                        List<UserSkillVM> slist = new List<UserSkillVM>();
                        slist = GetUsrSkills(0);

                        if (slist.Count != 0)
                        {
                            TempData["UserSkills"] = slist;
                        }

                        int MentorId = Convert.ToInt32(HttpContext.Session.GetInt32("RefId"));
                        var getMentor = GetRefData(MentorId);
                        if (getMentor != null)
                        {
                            TempData["Mentor"] = getMentor;
                        }


                        //WHiteBaord
                        List<UserWhiteBoardVM> wlist = new List<UserWhiteBoardVM>();
                        wlist = userWhiteBoards(0);

                        if (wlist.Count != 0)
                        {
                            TempData["UserWhiteBoard"] = wlist;
                        }

                        //MyTeam
                        List<Users> tlist = new List<Users>();
                        tlist = GetMyTeam(0);

                        if (tlist.Count != 0)
                        {
                            TempData["MyTeam"] = tlist;
                        }


                        //Get Certification
                        var pdata = client.GetAsync("Questionnaire/GetMyPassedCertificateData?id=" + Id.ToString());
                        pdata.Wait();
                        var presult = pdata.Result;
                        if (presult.IsSuccessStatusCode)
                        {
                            var ress = presult.Content.ReadAsStringAsync().Result;
                            var pList = JsonConvert.DeserializeObject<List<CertificateVM>>(ress);
                            if (pList.Count > 0)
                            {
                                TempData["Passed"] = pList;
                            }
                        }
                        //MyBlog
                        //List<Blog> blist = new List<Blog>();
                        //blist = GetBlogs(0);

                        //if (blist.Count != 0)
                        //{
                        //    TempData["Blog"] = blist;
                        //}
                    }

                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        //return View();
                    }
                }
                else
                {
                    //id = protector.Unprotect(id.ToString());
                    HttpClient client = _api.Initial();
                    var GetBank = client.GetAsync("My/GetMyProfile?Id=" + id.ToString());
                    GetBank.Wait();
                    var result = GetBank.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var User = JsonConvert.DeserializeObject<Users>(res);
                        TempData["User"] = User;



                        //UserSkills
                        List<UserSkillVM> slist = new List<UserSkillVM>();
                        int ID = Convert.ToInt32(id);
                        slist = GetUsrSkills(ID);

                        if (slist.Count != 0)
                        {
                            TempData["UserSkills"] = slist;
                        }


                        //WHiteBaord
                        List<UserWhiteBoardVM> wlist = new List<UserWhiteBoardVM>();
                        wlist = userWhiteBoards(ID);

                        if (wlist.Count != 0)
                        {
                            TempData["UserWhiteBoard"] = wlist;
                        }

                        //MyTeam
                        List<Users> tlist = new List<Users>();
                        tlist = GetMyTeam(ID);

                        if (tlist.Count != 0)
                        {
                            TempData["MyTeam"] = tlist;
                        }

                        //Get Certification
                        var pdata = client.GetAsync("Questionnaire/GetMyPassedCertificateData?id=" + id.ToString());
                        pdata.Wait();
                        var presult = pdata.Result;
                        if (presult.IsSuccessStatusCode)
                        {
                            var ress = presult.Content.ReadAsStringAsync().Result;
                            var pList = JsonConvert.DeserializeObject<List<CertificateVM>>(ress);
                            if (pList.Count > 0)
                            {
                                TempData["Passed"] = pList;
                            }
                        }


                        //MyBlog
                        //List<Blog> blist = new List<Blog>();
                        //blist = GetBlogs(ID);

                        //if (blist.Count != 0)
                        //{
                        //    TempData["Blog"] = blist;
                        //}
                        id = null;
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
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return View();
            }
            return View();
        }

        public IActionResult AutocompleteProfile(string term)
        {
            List<string> msg = new List<string> { "No Record found" };
            try
            {
                //List<string> AutoCourse;
                HttpClient client = _api.Initial();
                var GetBank = client.GetAsync("My/SearchForProfile?term=" + term.ToString());
                GetBank.Wait();
                var result = GetBank.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var uList = JsonConvert.DeserializeObject<List<Users>>(res);
                    if (uList.Count == 0 || uList == null)
                    {
                        return Json(msg);
                    }
                    else
                    {
                        //var userList = uList.Select(s => s.usrName).ToList();
                        var userList = (from usr in uList.Where(x => x.usrName.StartsWith(term))
                                        select new
                                        { value = usr.usrId, label = usr.usrName }).ToList();

                        return Json(uList);
                        //return Json(uList);
                    }
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                }

                //List<string> AutoCourse;
                //HttpClient client = _api.Initial();
                //Task<HttpResponseMessage> Data;
                //Data = client.GetAsync("My/SearchForProfile?term=" + term.ToString());
                //Data.Wait();
                //var result = Data.Result;
                //if (result.IsSuccessStatusCode)
                //{
                //    var res = result.Content.ReadAsStringAsync().Result;
                //    AutoCourse = JsonConvert.DeserializeObject<List<string>>(res);
                //    if (AutoCourse.Count == 0 || AutoCourse == null)
                //    {
                //        return Json(msg);
                //    }
                //    else
                //    {
                //        return Json(AutoCourse);
                //    }
                //}
                //else
                //{
                //    return Json(msg);
                //}
            }
            catch (Exception)
            {
                return Json(msg);
            }
            return Json(msg);
        }

        [SessionCheck]
        public IActionResult Dashboard()
        {
            try
            {
                var id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var Data = client.GetAsync("My/GetDashboardData?id=" + id.ToString());
                Data.Wait();
                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var info = JsonConvert.DeserializeObject<EasySystemAPI.Models.Dashboard>(res);
                    TempData["Dashboard"] = info;

                    //For VerifyEmail
                    bool IsVerify = CheckVerifyEmail();
                    if (IsVerify == false)
                    {
                        TempData["EmailNotVerified"] = "Please Verify Your Email.";
                    }

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
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        [SessionCheckActiveUser]
        public IActionResult BankInfo()
        {
            try
            {
                List<UserBankVM> bankInfoList = new List<UserBankVM>();
                bankInfoList = GetUserBankInfos();

                if (bankInfoList.Count != 0)
                {
                    TempData["BankInfo"] = bankInfoList;
                }
                var CountryId = HttpContext.Session.GetInt32("CountryId");
                HttpClient client = _api.Initial();
                var GetBank = client.GetAsync("My/PopulateUserBank?Id=" + CountryId.ToString());
                GetBank.Wait();
                var result = GetBank.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var bankList = JsonConvert.DeserializeObject<List<Bank>>(res);
                    SelectList list = new SelectList(bankList, "BankId", "BankName");
                    ViewData["Bank"] = list;
                }

                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    //return View();
                }



            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                //return View();
            }
            return View();
        }
        [SessionCheck]
        [HttpPost]
        public IActionResult BankInfo(UserBankInfo usr)
        {
            try
            {
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("My/AddUserBankInfo", usr);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var bankInfoList = JsonConvert.DeserializeObject<List<UserBankInfo>>(res);
                    TempData["BankInfo"] = bankInfoList;
                    TempData["Success"] = "Record added successfuly";
                    return RedirectToAction("BankInfo");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("BankInfo");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("BankInfo");
            }
        }

        [SessionCheck]
        public IActionResult DeleteBankInfo(int id)
        {
            try
            {
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                UserBankInfo usrBankInfo = new UserBankInfo();
                usrBankInfo.ubId = id;
                usrBankInfo.usrId = Id;
                HttpClient clientt = _api.Initial();
                var DelbankInfo = clientt.PostAsJsonAsync("My/DelBankInfo", usrBankInfo);
                DelbankInfo.Wait();
                var result = DelbankInfo.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record Deleted successfully";
                    return RedirectToAction("BankInfo");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("BankInfo");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("BankInfo");
            }

        }

        [SessionCheck]
        public IActionResult EditBankInfo(int id)
        {
            try
            {
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                UserBankInfo usrBankInfo = new UserBankInfo();
                usrBankInfo.ubId = id;
                usrBankInfo.usrId = Id;
                HttpClient client = _api.Initial();
                var UpdateData = client.PostAsJsonAsync("My/GetBankInfo", usrBankInfo);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var bankInfoList = JsonConvert.DeserializeObject<UserBankInfo>(res);
                    var GetBankList = PopulateUserBank();
                    SelectList list = new SelectList(GetBankList, "BankId", "BankName");
                    ViewData["Bank"] = list;
                    return View(bankInfoList);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("BankInfo");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("BankInfo");
            }

        }

        [SessionCheck]
        [HttpPost]

        public IActionResult EditBankInfo(int id, UserBankInfo usrBankInfo)
        {
            try
            {
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                usrBankInfo.usrId = Id;
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("My/UpdateBankInfo?id=" + id.ToString(), usrBankInfo);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("BankInfo");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("BankInfo");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("BankInfo");
            }

        }


        [SessionCheck]
        public IActionResult Skills()
        {
            try
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetUserSkillInfo?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<UserSkillVM>>(res);
                    TempData["UserSkills"] = list;
                    //Populate SKills
                    var skillsList = PopulateAllSkills();
                    SelectList slist = new SelectList(skillsList, "StId", "StName");
                    ViewData["Skills"] = slist;
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
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                //return View();
            }
            return View();
        }
        [HttpPost]
        [SessionCheck]
        public IActionResult Skills(UserSkills usr)
        {
            try
            {
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("My/AddUserSkillInfo", usr);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    //var res = result.Content.ReadAsStringAsync().Result;
                    //var userSkills = JsonConvert.DeserializeObject<List<UserSkills>>(res);
                    //TempData["UserSkills"] = userSkills;
                    TempData["Success"] = "Record added successfuly";
                    return RedirectToAction("Skills");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Skills");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Skills");
            }
        }

        [SessionCheck]
        public IActionResult DelSkills(int id)
        {
            try
            {
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                UserSkills skillData = new UserSkills();
                skillData.usId = id;
                skillData.usrId = Id;
                HttpClient client = _api.Initial();
                var DelData = client.PostAsJsonAsync("My/DelSkillInfo", skillData);
                DelData.Wait();
                var result = DelData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record Deleted successfully";
                    return RedirectToAction("Skills");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Skills");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Skills");
            }

        }
        [SessionCheck]
        public IActionResult EditSkills(int id)
        {
            try
            {
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                UserSkills skillData = new UserSkills();
                skillData.usId = id;
                skillData.usrId = Id;
                HttpClient client = _api.Initial();
                var UpdateData = client.PostAsJsonAsync("My/GetSkill", skillData);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<UserSkills>(res);
                    //Populate SKills
                    var skillsList = PopulateAllSkills();
                    SelectList slist = new SelectList(skillsList, "StId", "StName");
                    ViewData["Skills"] = slist;
                    return View(data);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Skills");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Skills");
            }

        }

        [SessionCheck]
        [HttpPost]
        public IActionResult EditSkills(int id, UserSkills skillData)
        {
            try
            {
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                skillData.usrId = Id;
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("My/UpdateSkill?id=" + id.ToString(), skillData);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("Skills");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Skills");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Skills");
            }

        }



        public IActionResult SkillDetail(int id, string Page)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("My/GetUserSkillInfoDetail?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<UserSkillVM>(res);
                    TempData["Page"] = Page;
                    TempData["SkillDetail"] = data;
                    return View();
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Skills");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Skills");
            }
        }


        [SessionCheck]
        public IActionResult CopyMentorSkills()
        {
            var Id = HttpContext.Session.GetInt32("ID");
            HttpClient client = _api.Initial();
            var GetData = client.GetAsync("My/CopyMyMentorSkills?id=" + Id.ToString());
            GetData.Wait();
            var result = GetData.Result;
            if (result.IsSuccessStatusCode)
            {
                TempData["Success"] = "Skills Copied successfully";
                return RedirectToAction("Skills");
            }

            else
            {
                var res = result.Content.ReadAsStringAsync().Result;
                var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                TempData["Error"] = "" + errorMsg.message + "";
                return RedirectToAction("Skills");
            }

        }


        [SessionCheck]
        public IActionResult WhiteBoard()
        {
            try
            {
                List<UserWhiteBoardVM> list = new List<UserWhiteBoardVM>();
                list = userWhiteBoards(0);

                if (list.Count != 0)
                {
                    TempData["UserWhiteBoard"] = list;
                }
                int? Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var postVerify = client.GetAsync("My/GetUserSkill?id=" + Id.ToString());
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var skillkList = JsonConvert.DeserializeObject<List<UserSkillVM>>(res);
                    SelectList slist = new SelectList(skillkList, "usId", "StName");
                    ViewData["Skills"] = slist;
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
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        [SessionCheck]
        [HttpPost]
        public IActionResult WhiteBoard(UserWhiteBoard usr, string editor)
        {
            try
            {
                usr.uwbDetail = editor;
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("My/AddUserWhiteBoard", usr);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record added successfuly";
                    return RedirectToAction("WhiteBoard");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("WhiteBoard");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("WhiteBoard");
            }
        }


        [SessionCheck]
        public IActionResult DeleteWhiteBoard(int id)
        {
            try
            {
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                UserWhiteBoard usrdata = new UserWhiteBoard();
                usrdata.uwbId = id;
                usrdata.usrId = Id;
                HttpClient client = _api.Initial();
                var DelData = client.PostAsJsonAsync("My/DelWhiteBoard", usrdata);
                DelData.Wait();
                var result = DelData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record Deleted successfully";
                    return RedirectToAction("WhiteBoard");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("WhiteBoard");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("WhiteBoard");
            }

        }

        [SessionCheck]
        public IActionResult EditWhiteBoard(int id)
        {
            try
            {
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                UserWhiteBoard usrdata = new UserWhiteBoard();
                usrdata.uwbId = id;
                usrdata.usrId = Id;
                HttpClient client = _api.Initial();
                var UpdateData = client.PostAsJsonAsync("My/GetWhiteBoard", usrdata);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<UserWhiteBoard>(res);
                    int uId = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                    var GetSkillList = GetUsrSkills(uId);
                    SelectList slist = new SelectList(GetSkillList, "usId", "StName");
                    ViewData["Skills"] = slist;
                    return View(data);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("WhiteBoard");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("WhiteBoard");
            }

        }

        [SessionCheck]
        [HttpPost]
        public IActionResult EditWhiteBoard(int id, UserWhiteBoard usrdata, string editor)
        {
            try
            {
                usrdata.uwbDetail = editor;
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                usrdata.usrId = Id;
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("My/UpdateWhiteBoard?id=" + id.ToString(), usrdata);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("WhiteBoard");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("WhiteBoard");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("WhiteBoard");
            }

        }

        public IActionResult WhiteBoardDetail(int id, string Page)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("My/GetWhiteBoardDetail?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<UserWhiteBoardVM>(res);
                    TempData["Page"] = Page;
                    TempData["WhiteboardDetail"] = data;
                    return View(data);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("WhiteBoard");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("WhiteBoard");
            }

        }

        [SessionCheck]
        public IActionResult MentorWhiteBoard(int? id)
        {
            try
            {
                var Id = "";
                if (id == null)
                {
                    Id = HttpContext.Session.GetInt32("RefId").ToString();
                }
                else
                {
                    Id = id.ToString();
                }

                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetMyMentorWhiteBoard?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<UserWhiteBoardVM>>(res);
                    TempData["UserWhiteBoard"] = list;
                    return View();
                }

                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    if (errorMsg != null)
                    {
                        TempData["Error"] = "" + errorMsg.message + "";
                    }

                    return View();
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        [SessionCheck]
        public IActionResult URL()
        {
            var Code = HttpContext.Session.GetInt32("Code");
            //Live
            var Url = "https://universalskills.co/Signup/M?C=" + Code + "";

            //Local
            //var Url = "http://localhost:60337/Signup/M?C=" + Code + "";
            TempData["Code"] = Url;
            return View();
        }

        [SessionCheck]
        public IActionResult Mentor(string code)
        {
            TempData["Info"] = "Please register you PhoneNo first";
            return RedirectToAction("SignUp", "Users", new { code });
        }


        [SessionCheck]
        public IActionResult Team(string Page)
        {
            try
            {
                //Get Team 
                int? Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("Users/GetTeam?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<TeamVM>>(res);
                    if (Page != null)
                    {
                        TempData["Page"] = Page;
                    }
                    TempData["MyTeam"] = list;

                    ////Get Team logs
                    //var Logs = comm.GetLogs((int)Id);
                    //if (Logs.Count > 0)
                    //{
                    //    TempData["Logs"] = Logs;
                    //}
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return View();
                }

                //Other Trainees
                int id = Convert.ToInt32(Id);
                var OtherTrainees = comm.GetOtherTrainees(id);
                if (OtherTrainees.Count > 0)
                {
                    TempData["OtherTrainees"] = OtherTrainees;
                }


            }
            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
            }
            return View();
        }


        [SessionCheckActiveUser]
        public IActionResult Coupans()
        {
            try
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("Transection/GetMyCoupan?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<CoupanVM>>(res);
                    TempData["MyCoupan"] = list;
                    return View();
                }

                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    if (errorMsg != null)
                    {
                        TempData["Error"] = "" + errorMsg.message + "";
                    }
                    else
                    {
                        TempData["Info"] = "No Coupon found";
                    }

                    return View();
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                //return View();
            }
            return View();
        }

        [SessionCheckActiveUser]
        public IActionResult MyLostTrainee(string Page)
        {
            try
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetMyLostTrainee?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<LostUser>>(res);
                    TempData["LostTraniee"] = list;
                    if (Page != null)
                    {
                        TempData["Page"] = Page;
                    }
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
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        [SessionCheck]
        public IActionResult MentorSkills()
        {
            try
            {
                var Id = HttpContext.Session.GetInt32("RefId");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetMyMentorSkill?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<UserSkillVM>>(res);
                    if (list.Count > 0)
                    {
                        TempData["UserSkills"] = list;
                    }

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
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                //return View();
            }
            return View();
        }

        [SessionCheckActiveUser]
        public IActionResult AccountVerification()
        {
            try
            {
                var Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                List<EasySystemAPI.Models.UserVerification> userVerifications = new List<EasySystemAPI.Models.UserVerification>();
                userVerifications = GetUserVerification(Id);
                if (userVerifications.Count > 0)
                {
                    TempData["UserVerifications"] = userVerifications;
                }

                List<EasySystemAPI.Models.UserVerificationType> userVerificationTypes = new List<EasySystemAPI.Models.UserVerificationType>();
                userVerificationTypes = GetVerificationType();
                if (userVerificationTypes.Count > 0)
                {
                    TempData["UserVerificationTypes"] = userVerificationTypes;
                }
                return View();
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                //return View();
            }
            return View();
        }

        [SessionCheck]
        [HttpPost]
        public IActionResult AccountVerification(int uvtId, string uvText, IFormFile file, string Email, string btnName)
        {
            try
            {
                EasySystemAPI.Models.UserVerification data = new EasySystemAPI.Models.UserVerification();
                if (file != null)
                {
                    var allowedExtensions = new[] { ".Jpg", ".JPG", ".jpg", ".jpeg", ".JPEG", ".png", ".PNG" };
                    var ext = Path.GetExtension(file.FileName);
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/VerifiedPictures");
                        string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        //file.CopyTo(new FileStream(filePath, FileMode.Create));
                        data.uvImagePath = fileName;
                    }
                    else
                    {
                        TempData["Error"] = "Please Upload a valid image file. Valid formates are ('.Jpg','.JPG', '.jpg', .'jpeg', '.JPEG', '.png', '.PNG')";
                        return RedirectToAction("AccountVerification");
                    }
                }
                var Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));

                data.usrId = Id;
                data.uvtId = uvtId;
                if (btnName == "Verify")
                {
                    data.uvStatus = "Verify";
                }
                if (Email != null && Email != "")
                {

                    data.uvStatusRemarks = Email;
                }

                if (uvText != null && uvText != "")
                {
                    data.uvText = uvText;
                }
                HttpClient client = _api.Initial();
                var GetData = client.PostAsJsonAsync("UserVerification/AddUserVerificationData", data);
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    if (Email != null && Email != "")
                    {
                        HttpContext.Session.SetString("Email", Email);
                    }

                    var res = result.Content.ReadAsStringAsync().Result;
                    if (res != null)
                    {
                        var SuccessMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        if (SuccessMsg != null)
                        {
                            TempData["Info"] = "" + SuccessMsg.message + "";
                        }
                    }

                    else
                    {
                        TempData["Success"] = "Data Uploaded successfully";
                    }
                    return RedirectToAction("AccountVerification");
                }

                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Info"] = "" + errorMsg.message + "";
                    return RedirectToAction("AccountVerification");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                //return View();
            }
            return RedirectToAction("AccountVerification");
        }


        [SessionCheck]
        public ActionResult Certification()
        {
            try
            {
                var id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var data = client.GetAsync("Questionnaire/GetMyFailedCertificateData?id=" + id.ToString());
                data.Wait();
                var result = data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var fList = JsonConvert.DeserializeObject<List<CertificateVM>>(res);
                    if (fList.Count > 0)
                    {
                        TempData["Failed"] = fList;
                    }
                }

                var pdata = client.GetAsync("Questionnaire/GetMyPassedCertificateData?id=" + id.ToString());
                pdata.Wait();
                var presult = pdata.Result;
                if (presult.IsSuccessStatusCode)
                {
                    var res = presult.Content.ReadAsStringAsync().Result;
                    var pList = JsonConvert.DeserializeObject<List<CertificateVM>>(res);
                    if (pList.Count > 0)
                    {
                        TempData["Passed"] = pList;
                    }
                }
            }
            catch (Exception)
            {

            }
            return View();
        }

        [SessionCheck]
        public ActionResult GetCertificate(string Num)
        {
            TempData["No"] = Num;
            return View();
        }


        public ActionResult _Certificate(string Num)
        {
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Questionnaire/GetMyCertificate?Num=" + Num);
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                var cList = JsonConvert.DeserializeObject<CertificateVM>(res);
                if (cList != null)
                {
                    TempData["Certificate"] = cList;
                }
                else
                {
                    return Content("No record found");
                }
            }
            return PartialView();
        }

        public ActionResult DownloadPdf()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DownloadPdf(string GridHtml)
        {


            //var globalSettings = new GlobalSettings
            //{
            //    ColorMode = ColorMode.Color,
            //    Orientation = Orientation.Portrait,
            //    PaperSize = PaperKind.A4,
            //    Margins = new MarginSettings { Top = 10 },
            //    DocumentTitle = "PDF Report"
            //};
            //var objectSettings = new ObjectSettings
            //{
            //    PagesCount = true,
            //    HtmlContent = "https://code-maze.com/create-pdf-dotnetcore/",
            //    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            //    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
            //    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            //};
            //var pdf = new HtmlToPdfDocument()
            //{
            //    GlobalSettings = globalSettings,
            //    Objects = { objectSettings }
            //};
            //var file = _converter.Convert(pdf);
            //return File(file, "application/pdf");


            //// read parameters from the webpage
            //string url = "~/Views/My/_Certificate";
            ////string url = Path.Combine(HostingEnvironment.ContentRootPath, "Views\\My\\_Certificate.cshtml");

            //string pdf_page_size = "1";
            //PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), pdf_page_size, true);

            //string pdf_orientation = "landscape";
            //PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(
            //    typeof(PdfPageOrientation), pdf_orientation, true);

            //int webPageWidth = 1024;


            //int webPageHeight = 0;

            //// instantiate a html to pdf converter object
            //HtmlToPdf converter = new HtmlToPdf();

            //// set converter options
            //converter.Options.PdfPageSize = pageSize;
            //converter.Options.PdfPageOrientation = pdfOrientation;
            //converter.Options.WebPageWidth = webPageWidth;
            //converter.Options.WebPageHeight = webPageHeight;

            //// create a new pdf document converting an url
            ////PdfDocument doc = converter.ConvertUrl(url);
            //PdfDocument doc = converter.ConvertHtmlString(url);


            //// save pdf document
            //byte[] pdf = doc.Save();

            //// close pdf document
            //doc.Close();

            //// return resulted pdf document
            //return File(pdf, "application/pdf", "Grid.pdf");

            ////FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            ////fileResult.FileDownloadName = "Document.pdf";
            ////return fileResult;
            ////Byte[] res = null;
            ////using (MemoryStream ms = new MemoryStream())
            ////{
            ////    var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(GridHtml, PdfSharp.PageSize.A4);
            ////    pdf.Save(ms);
            ////    res = ms.ToArray();
            ////}
            //////return File(res, "application/pdf", "Grid.pdf");
            return View();
        }

        [SessionCheck]
        public ActionResult _SkillData(int Value)
        {
            try
            {
                HttpClient client = _api.Initial();
                Task<HttpResponseMessage> Data;
                Data = client.GetAsync("My/GetSkillTypeData?value=" + Value.ToString());
                Data.Wait();
                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var List = JsonConvert.DeserializeObject<List<EasySystemAPI.Models.SkillType>>(res);
                    if (List.Count == 0)
                    {
                        return Content("");
                    }
                    TempData["Skills"] = List;
                }
                else
                {
                    return Content("");
                    //var res = result.Content.ReadAsStringAsync().Result;
                    //var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    //TempData["Error"] = "" + errorMsg.message + "";
                }

                //var GetSkills = comm.GetSkills();
                //if (GetSkills.Count > 0)
                //{
                //    TempData["Skills"] = GetSkills;
                //    return PartialView();
                //}
            }
            catch (Exception)
            {

            }
            return PartialView();
        }

        [SessionCheck]
        public ActionResult _TestSkillData(int Value, int Id)
        {
            try
            {
                EasySystemAPI.Models.PageModel data = new EasySystemAPI.Models.PageModel();
                data.Count = Value;
                data.id = Id;
                HttpClient client = _api.Initial();
                Task<HttpResponseMessage> Data;
                Data = client.PostAsJsonAsync("My/GeSkillsData", data);
                Data.Wait();
                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var List = JsonConvert.DeserializeObject<List<EasySystemAPI.Models.SkillType>>(res);
                    if (List.Count == 0)
                    {
                        return Content("");
                    }
                    TempData["SkillId"] = Id;
                    TempData["TestSkills"] = List;
                }
                else
                {
                    return Content("");
                }


                //var GetSkills = comm.GetTestSkills(id);
                //if (GetSkills.Count > 0)
                //{
                //    TempData["TestSkills"] = GetSkills;
                //    return PartialView();
                //}
            }
            catch (Exception)
            {

            }
            return PartialView();
        }


        [SessionCheck]
        public ActionResult CertificationTest(int id, string Name)
        {
            try
            {
                TempData["Test"] = Name;
                TempData["StId"] = id;
            }
            catch (Exception)
            {

            }
            return View();
        }

        [SessionCheck]
        public ActionResult _QuestionData(int id)
        {
            try
            {
                UserSkills user = new UserSkills();
                user.StId = id;
                user.usrId = (int)HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var data = client.PostAsJsonAsync("Questionnaire/GetTestQuestions", user);
                data.Wait();
                var result = data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var sList = JsonConvert.DeserializeObject<List<EasySystemAPI.Models.Question_Story>>(res);
                    if (sList.Count > 0)
                    {
                        TempData["TestQuestion"] = sList;
                        return PartialView();
                    }
                    else
                    {
                        TempData["Info"] = "No Record found";
                        ViewBag.No = "No";
                        return PartialView();
                    }
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    if (errorMsg.message != null)
                    {
                        if (errorMsg.message == "You already passed this test. Please choose another category")
                        {
                            TempData["Info"] = errorMsg.message;
                            return PartialView();
                        }
                        else
                        {
                            TempData["Error"] = "" + errorMsg.message + "";
                            return PartialView();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return PartialView();
        }


        public ActionResult AddTestResult(int StId, int cId, string AccDetails)
        {
            try
            {
                if (StId != 0)
                {
                    if (AccDetails != null)
                    {
                        List<TestVM> qList = new List<TestVM>();

                        var ObjDetail = JsonConvert.DeserializeObject<List<TestVM>>(AccDetails);
                        EasySystemAPI.Models.Question_Story q = new EasySystemAPI.Models.Question_Story();
                        q.StId = StId;
                        q.qCategory = cId;
                        var GetQuest = comm.TestVerification(q);
                        if (GetQuest.Count > 0)
                        {

                            int Passing = 80;
                            int Total = GetQuest.Count();
                            int RightAns = 0;
                            foreach (var i in GetQuest)
                            {
                                var ChkAns = ObjDetail.Where(o => o.qId == i.qId).FirstOrDefault();
                                if (ChkAns != null)
                                {
                                    if (i.qAnswer == ChkAns.Opt1 || i.qAnswer == ChkAns.Opt2 || i.qAnswer == ChkAns.Opt3 || i.qAnswer == ChkAns.Opt4)
                                    {
                                        RightAns = RightAns + 1;
                                    }
                                }
                            }



                            EasySystemAPI.Models.TestVM qResult = new EasySystemAPI.Models.TestVM();
                            int result = (int)Math.Round(((double)RightAns / (double)Total) * 100);

                            if (result >= Passing)
                            {
                                qResult.uqrStatus = "Passed";
                                //User Certification
                                EasySystemAPI.Models.UserCertificate qCertificate = new EasySystemAPI.Models.UserCertificate();


                            }
                            else
                            {
                                qResult.uqrStatus = "Failed";
                            }
                            qResult.qCategory = cId;
                            qResult.StId = StId;
                            qResult.usrId = (int)HttpContext.Session.GetInt32("ID");
                            qResult.Date = DateTime.Now;
                            //CheckNum
                            foreach (var i in comm.RandomString(6))
                            {
                                var Num = comm.RandomString(6);
                                var check = comm.CheckNumber(Num);
                                if (check == true)
                                {
                                    qResult.ucNumber = Num;
                                    break;
                                }
                            }


                            HttpClient client = _api.Initial();
                            var data = client.PostAsJsonAsync("Questionnaire/AddTestResult", qResult);
                            data.Wait();
                            var resultt = data.Result;
                            if (resultt.IsSuccessStatusCode)
                            {
                                if (qResult.uqrStatus == "Passed")
                                {
                                    return Json(new { success = true, Result = "Passed", responseText = "Congratulations. You have pass the Test successfully.You have secured: " + RightAns + "/" + Total + ".(" + result + "%)" });
                                }
                                else
                                {
                                    return Json(new { success = true, Result = "Failed", responseText = "oops! Better luck next time! Test Result: " + RightAns + "/" + Total + ".(" + result + "%)" });
                                }

                            }

                        }
                    }
                    else
                    {
                        return Json(new { success = false, responseText = "Please Check all the questions." });

                    }
                }
                else
                {
                    return Json(new { success = false, responseText = "No record found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "An error occured." });
            }
            return Json(new { success = false, responseText = "No record found." });
        }

        [SessionCheck]
        public ActionResult AllMentors()
        {
            try
            {
                int? MentorId = HttpContext.Session.GetInt32("RefId");

                var Mentor = GetRefData(Convert.ToInt32(MentorId));
                if (Mentor != null)
                {
                    TempData["MentorName"] = Mentor.usrName + "-" + Mentor.usrCode;
                    TempData["MentorCode"] = Mentor.usrCode;
                }

                var Code = HttpContext.Session.GetInt32("Code");
                TempData["Code"] = Code;


                var usrStatus = HttpContext.Session.GetString("Status");
                TempData["Status"] = usrStatus;

                int? CountryId = HttpContext.Session.GetInt32("CountryId");
                TempData["CId"] = CountryId;

                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("Users/GetMyAllMentor?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<UserMentorVM>>(res);
                    if (list.Count > 0)
                    {
                        TempData["UserMentor"] = list;
                    }

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
            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }

        }

        [SessionCheck]
        public ActionResult MakeMentor(int id)
        {
            try
            {
                var Id = HttpContext.Session.GetInt32("ID");
                if (Id != null)
                {
                    EasySystemAPI.Models.UserMentors data = new EasySystemAPI.Models.UserMentors();
                    data.usrId = Convert.ToInt32(Id);
                    data.mentId = id;
                    HttpClient client = _api.Initial();
                    var GetData = client.PostAsJsonAsync("Users/MakeMentor", data);
                    GetData.Wait();
                    var result = GetData.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("AllMentors");
                    }

                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return RedirectToAction("AllMentors");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Users");
                }

            }
            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }


        [SessionCheck]
        public ActionResult ChangeCoreMentor(int id)
        {
            try
            {
                var Id = HttpContext.Session.GetInt32("ID");
                if (Id != null)
                {
                    EasySystemAPI.Models.UserMentors data = new EasySystemAPI.Models.UserMentors();
                    data.usrId = Convert.ToInt32(Id);
                    data.mentId = id;
                    HttpClient client = _api.Initial();
                    var GetData = client.PostAsJsonAsync("Users/ChangeCoreMentor", data);
                    GetData.Wait();
                    var result = GetData.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        HttpContext.Session.SetInt32("RefId", id);
                        return RedirectToAction("AllMentors");
                    }

                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return RedirectToAction("AllMentors");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Users");
                }

            }
            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }


        [SessionCheck]
        public ActionResult LeaveMentor(int id)
        {
            try
            {
                var Id = HttpContext.Session.GetInt32("ID");
                if (Id != null)
                {
                    EasySystemAPI.Models.UserMentors data = new EasySystemAPI.Models.UserMentors();
                    data.usrId = Convert.ToInt32(Id);
                    data.mentId = id;
                    HttpClient client = _api.Initial();
                    var GetData = client.PostAsJsonAsync("Users/LeaveMentor", data);
                    GetData.Wait();
                    var result = GetData.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("AllMentors");
                    }

                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return RedirectToAction("AllMentors");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Users");
                }

            }
            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        [SessionCheckActiveUser]
        public ActionResult CatchTheTrainee()
        {
            try
            {
                int cId = (int)HttpContext.Session.GetInt32("CountryId");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("Users/CatchTheTrainee?id=" + cId.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var usrData = JsonConvert.DeserializeObject<List<Users>>(res);
                    if (usrData.Count > 0)
                    {
                        TempData["Trainees"] = usrData;
                    }
                    else
                    {
                        TempData["Info"] = "No new trainees for today";
                    }
                    return View();
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("AllMentors");
                }
            }
            catch (Exception)
            {

            }
            return View();
        }

        public ActionResult SetTrainingFee()
        {
            try
            {
                int Id = (int)HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/TrainingFee?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var usrData = JsonConvert.DeserializeObject<EasySystemAPI.Models.MentorFee>(res);
                    if (usrData != null)
                    {
                        TempData["FeeData"] = usrData;
                        return View(usrData);
                    }

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
            catch (Exception)
            {

            }
            return View();
        }

        [HttpPost]
        public ActionResult SetTrainingFee(string Amount)
        {
            try
            {
                if (Amount != null)
                {
                    int Id = (int)HttpContext.Session.GetInt32("ID");
                    EasySystemAPI.Models.MentorFee Fee = new EasySystemAPI.Models.MentorFee();
                    Fee.usrId = Id;
                    Fee.usrFeeAmount = Convert.ToDouble(Amount);

                    HttpClient client = _api.Initial();
                    var GetData = client.PostAsJsonAsync("My/SetTrainingAmount", Fee);
                    GetData.Wait();
                    var result = GetData.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Training Fee amount has been updated successfully";
                        return Json(new { success = true, responseText = "Training Fee updated." });
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return Json(new { success = false, responseText = "Training Fee not Updated." });
                    }

                }
            }
            catch (Exception)
            {
                return Json(new { success = false, responseText = "Training Fee not Updated." });
            }
            return Json(new { success = false, responseText = "Training Fee not Updated." });

        }

        public IActionResult Chat()
        {
            return View();
        }

        public List<UserBankVM> GetUserBankInfos()
        {
            List<UserBankVM> bankInfoList = new List<UserBankVM>();
            var Id = HttpContext.Session.GetInt32("ID");
            HttpClient clientt = _api.Initial();
            var GetbankInfo = clientt.GetAsync("My/GetUserBankInfo?id=" + Id.ToString());
            GetbankInfo.Wait();
            var reslt = GetbankInfo.Result;
            if (reslt.IsSuccessStatusCode)
            {
                var res = reslt.Content.ReadAsStringAsync().Result;
                bankInfoList = JsonConvert.DeserializeObject<List<UserBankVM>>(res);
            }
            return bankInfoList;
            //else
            //{
            //    var res = reslt.Content.ReadAsStringAsync().Result;
            //    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
            //    TempData["Error"] = "" + errorMsg.message + "";
            //    //return View();
            //}

        }

        public List<UserWhiteBoardVM> userWhiteBoards(int? id)
        {
            List<UserWhiteBoardVM> list = new List<UserWhiteBoardVM>();
            if (id != 0)
            {
                //var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetUserWhiteBoard?id=" + id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<UserWhiteBoardVM>>(res);
                }
            }
            else
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetUserWhiteBoard?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<UserWhiteBoardVM>>(res);
                }
            }

            return list;
        }

        public List<UserWhiteBoardVM> GetMentorWhiteBoards()
        {
            List<UserWhiteBoardVM> list = new List<UserWhiteBoardVM>();
            var Id = HttpContext.Session.GetInt32("ID");
            HttpClient client = _api.Initial();
            var GetData = client.GetAsync("My/GetMyMentorWhiteBoard?id=" + Id.ToString());
            GetData.Wait();
            var result = GetData.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                list = JsonConvert.DeserializeObject<List<UserWhiteBoardVM>>(res);
            }
            return list;
        }

        public List<Bank> PopulateBank()
        {
            List<Bank> bankList = new List<Bank>();
            HttpClient client = _api.Initial();
            var GetBank = client.GetAsync("My/PopulateBank");
            GetBank.Wait();
            var result = GetBank.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                bankList = JsonConvert.DeserializeObject<List<Bank>>(res);
            }

            return bankList;
        }

        public List<Bank> PopulateUserBank()
        {
            var CountryId = HttpContext.Session.GetInt32("CountryId");
            List<Bank> bankList = new List<Bank>();
            HttpClient client = _api.Initial();
            var GetBank = client.GetAsync("My/PopulateUserBank?Id=" + CountryId.ToString());
            GetBank.Wait();
            var result = GetBank.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                bankList = JsonConvert.DeserializeObject<List<Bank>>(res);
            }

            return bankList;
            //else
            //{
            //    var res = result.Content.ReadAsStringAsync().Result;
            //    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
            //    TempData["Error"] = "" + errorMsg.message + "";

            //}

        }

        public List<UserSkills> PopulateSkills()
        {
            List<UserSkills> List = new List<UserSkills>();
            HttpClient client = _api.Initial();
            var GetSkills = client.GetAsync("My/PopulateSkills");
            GetSkills.Wait();
            var result = GetSkills.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                List = JsonConvert.DeserializeObject<List<UserSkills>>(res);
            }

            return List;
        }

        public List<UserSkillVM> GetUsrSkills(int? id)
        {
            List<UserSkillVM> List = new List<UserSkillVM>();
            if (id != 0)
            {
                //var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetSkills = client.GetAsync("My/GetUserSkill?id=" + id.ToString());
                GetSkills.Wait();
                var result = GetSkills.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    List = JsonConvert.DeserializeObject<List<UserSkillVM>>(res);
                }
            }
            else
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetSkills = client.GetAsync("My/GetUserSkill?id=" + Id.ToString());
                GetSkills.Wait();
                var result = GetSkills.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    List = JsonConvert.DeserializeObject<List<UserSkillVM>>(res);
                }
            }


            return List;
        }

        public List<Users> GetMyTeam(int? id)
        {
            List<Users> List = new List<Users>();
            if (id != 0)
            {
                //var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetMyTeam?id=" + id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    List = JsonConvert.DeserializeObject<List<Users>>(res);

                    TempData["MyTeam"] = List;

                }
            }
            else
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetMyTeam?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    List = JsonConvert.DeserializeObject<List<Users>>(res);
                    TempData["MyTeam"] = List;

                }
            }


            return List;
        }

        public List<Blog> GetBlogs(int? id)
        {
            List<Blog> List = new List<Blog>();
            if (id != 0)
            {
                //var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetBlogData?id=" + id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    List = JsonConvert.DeserializeObject<List<Blog>>(res);

                    TempData["Blog"] = List;

                }
            }
            else
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetBlogData?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    List = JsonConvert.DeserializeObject<List<Blog>>(res);
                    TempData["Blog"] = List;

                }
            }


            return List;
        }

        public List<UserSkills> PopulateUserSkills()
        {
            List<UserSkills> List = new List<UserSkills>();
            int? Id = HttpContext.Session.GetInt32("ID");
            HttpClient client = _api.Initial();
            var GetSkills = client.GetAsync("My/GetUserSkill?id=" + Id.ToString());
            GetSkills.Wait();
            var result = GetSkills.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                List = JsonConvert.DeserializeObject<List<UserSkills>>(res);
            }

            return List;
        }

        public List<EasySystemAPI.Models.SkillType> PopulateAllSkills()
        {
            List<EasySystemAPI.Models.SkillType> List = new List<EasySystemAPI.Models.SkillType>();
            HttpClient client = _api.Initial();
            var GetSkills = client.GetAsync("My/GetAllSkill");
            GetSkills.Wait();
            var result = GetSkills.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                List = JsonConvert.DeserializeObject<List<EasySystemAPI.Models.SkillType>>(res);
            }

            return List;
        }

        public List<EasySystemAPI.Models.UserVerificationType> GetVerificationType()
        {
            List<EasySystemAPI.Models.UserVerificationType> List = new List<EasySystemAPI.Models.UserVerificationType>();
            HttpClient client = _api.Initial();
            var GetSkills = client.GetAsync("UserVerification/GetVerificationTypeForUser");
            GetSkills.Wait();
            var result = GetSkills.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                List = JsonConvert.DeserializeObject<List<EasySystemAPI.Models.UserVerificationType>>(res);
            }

            return List;
        }

        public List<EasySystemAPI.Models.UserVerification> GetUserVerification(int id)
        {
            List<EasySystemAPI.Models.UserVerification> List = new List<EasySystemAPI.Models.UserVerification>();
            HttpClient client = _api.Initial();
            var GetSkills = client.GetAsync("UserVerification/GetUserVerificationUser?id=" + id.ToString());
            GetSkills.Wait();
            var result = GetSkills.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                List = JsonConvert.DeserializeObject<List<EasySystemAPI.Models.UserVerification>>(res);
            }

            return List;
        }

        public Users GetRefData(int id)
        {
            Users users = new Users();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("My/GetRefData?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<Users>(res);
            }
            return users;
        }

        public bool CheckVerifyEmail()
        {
            var Id = HttpContext.Session.GetInt32("ID");
            HttpClient client = _api.Initial();
            var GetData = client.GetAsync("My/CheckEmailVerify?id=" + Id.ToString());
            GetData.Wait();
            var result = GetData.Result;
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult RunQuery()
        {
            try
            {
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("Users/RecentLoginLogs");
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Operation Performed successfully";
                    return View();
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "Operation Failed";
                    return View();
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Operation Failed";
            }
            return View();
        }
    }
}