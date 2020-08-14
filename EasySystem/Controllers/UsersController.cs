using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySystem.Models;
using EasySystem.EasyAPI;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using EasySystemAPI.Models;
using Country = EasySystem.Models.Country;
using Microsoft.AspNetCore.Mvc.Rendering;
using EasySystem.General;

namespace EasySystem.Controllers
{
    public class UsersController : Controller
    {
        EasySysAPI _api = new EasySysAPI();
        Common comm = new Common();
        private readonly IHostingEnvironment HostingEnvironment;
        public UsersController(IHostingEnvironment hostingEnvironment)
        {
            this.HostingEnvironment = hostingEnvironment;
        }

        public IActionResult Profile(string id)
        {
            try
            {
                if (id == null)
                {
                    var Id = HttpContext.Session.GetInt32("ID");

                    HttpClient client = _api.Initial();
                    var GetBank = client.GetAsync("My/GetMyProfile?id=" + Id.ToString());
                    GetBank.Wait();
                    var result = GetBank.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var User = JsonConvert.DeserializeObject<EasySystem.Models.Users>(res);
                        TempData["User"] = User;

                        //UserSkills
                        List<EasySystem.Models.UserSkillVM> slist = new List<EasySystem.Models.UserSkillVM>();
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
                        List<EasySystem.Models.UserWhiteBoardVM> wlist = new List<EasySystem.Models.UserWhiteBoardVM>();
                        wlist = userWhiteBoards(0);

                        if (wlist.Count != 0)
                        {
                            TempData["UserWhiteBoard"] = wlist;
                        }

                        //MyTeam
                        List<EasySystem.Models.Users> tlist = new List<EasySystem.Models.Users>();
                        tlist = GetMyTeam(0);

                        if (tlist.Count != 0)
                        {
                            TempData["MyTeam"] = tlist;
                        }

                        //MyBlog
                        //List<EasySystem.Models.Blog> blist = new List<EasySystem.Models.Blog>();
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
                        var User = JsonConvert.DeserializeObject<EasySystem.Models.Users>(res);
                        TempData["User"] = User;

                        var UsrID = HttpContext.Session.GetInt32("ID");
                        if (UsrID == null)
                        {
                            HttpContext.Session.SetString("UsrName", User.usrName);
                            HttpContext.Session.SetInt32("UsrID", User.usrId);
                            TempData["Profile"] = User;
                        }


                        //UserSkills
                        List<EasySystem.Models.UserSkillVM> slist = new List<EasySystem.Models.UserSkillVM>();
                        int ID = Convert.ToInt32(id);
                        slist = GetUsrSkills(ID);

                        if (slist.Count != 0)
                        {
                            TempData["UserSkills"] = slist;
                        }


                        //WHiteBaord
                        List<EasySystem.Models.UserWhiteBoardVM> wlist = new List<EasySystem.Models.UserWhiteBoardVM>();
                        wlist = userWhiteBoards(ID);

                        if (wlist.Count != 0)
                        {
                            TempData["UserWhiteBoard"] = wlist;
                        }

                        //MyTeam
                        List<EasySystem.Models.Users> tlist = new List<EasySystem.Models.Users>();
                        tlist = GetMyTeam(ID);

                        if (tlist.Count != 0)
                        {
                            TempData["MyTeam"] = tlist;
                        }

                        //MyBlog
                        //List<EasySystem.Models.Blog> blist = new List<EasySystem.Models.Blog>();
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
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return View();
            }
            return View();
        }

        public IActionResult SignUp(string code)
        {
            if (code != null)
            {
                TempData["MentorCode"] = code;
            }
            var GetCountries = GetCountryList();
            SelectList list = new SelectList(GetCountries, "CountryCode", "CountryName");
            ViewData["Country"] = list;

            return View();
        }
        [HttpPost]
        public IActionResult SignUp(EasySystem.Models.UserSignUpCode usr, string code, string cCode)
        {
            try
            {
                if (cCode != null && cCode != "")
                {
                    EasySystem.Models.PhoneNumber no = new EasySystem.Models.PhoneNumber();
                    no.Num = usr.usPhone;
                    no.Code = cCode;

                    HttpClient client = _api.Initial();
                    var postSignUp = client.PostAsJsonAsync<EasySystem.Models.PhoneNumber>("UserSignUpCodes/RegisterPhoneNo", no);
                    postSignUp.Wait();
                    var result = postSignUp.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var PhoneNo = JsonConvert.DeserializeObject<EasySystem.Models.UserSignUpCode>(res);
                        TempData["Number"] = PhoneNo.usPhone;
                        TempData["Info"] = "An OTP send to your Mobile number for verification. This code will be expired in 10 minutes";
                        if (code != null)
                        {
                            TempData["MentorCode"] = code;
                        }
                        TempData["cCode"] = cCode;
                        return RedirectToAction("Code_Verification");
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        if (errorMsg.message == "This Phone Number is verified. Please go to Register Page.")
                        {
                            var Number = comm.PhoneNoWithCountryCode(no);
                            TempData["Number"] = Number.usPhone;
                            TempData["Info"] = "This phone number is verified. Please register yourself.";
                            return RedirectToAction("Registration");
                        }
                        else if (errorMsg.message == "This Phone Number is already exist please choose a different one.")
                        {
                            TempData["Warning"] = "This Phone Number is already exist please choose a different one.";
                            return RedirectToAction("SignUp");
                        }
                        else
                        {
                            TempData["Error"] = "" + errorMsg.message + "";
                            return RedirectToAction("SignUp");
                        }

                    }


                }
                else
                {
                    TempData["Error"] = "Country Code not found. please try again later";
                    return RedirectToAction("SignUp");
                }



            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured to proceed with your request please try again later";
                return RedirectToAction("SignUp");
            }

        }

        public IActionResult Code_Verification()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Code_Verification(EasySystem.Models.UserSignUpCode usr, string code, string cCode)
        {
            try
            {
                if (usr.usPhone != "" && usr.usPhone != null)
                {
                    if (usr.uscCode != 0)
                    {
                        HttpClient client = _api.Initial();
                        var postVerify = client.PostAsJsonAsync<EasySystem.Models.UserSignUpCode>("UserSignUpCodes/RegisterCodeVerification", usr);
                        postVerify.Wait();
                        var result = postVerify.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Number"] = usr.usPhone;
                            TempData["Success"] = "Phone Number verified Successfully";
                            if (code != null)
                            {
                                TempData["MentorCode"] = code;
                            }
                            TempData["cCode"] = cCode;
                            return View("Registration");
                        }
                        else
                        {
                            var res = result.Content.ReadAsStringAsync().Result;
                            var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                            TempData["Error"] = "" + errorMsg.message + "";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Code must not be empty";
                        return View();
                    }

                }
                else
                {
                    TempData["Error"] = "Your Phone Number not Found Please try again later";
                    return View();
                }

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "there is some error. Please try again later";
            }

            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(EasySystem.Models.Users usr, string rePass, IFormFile perImage, string cCode)
        {
            try
            {
                if (usr.usrPassword == rePass)
                {
                    if (perImage != null)
                    {
                        string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                        string fileName = Guid.NewGuid().ToString() + "_" + perImage.FileName;
                        string filePath = Path.Combine(uploadFolder, fileName);
                        perImage.CopyTo(new FileStream(filePath, FileMode.Create));
                        usr.usrImage = fileName;
                    }
                    if (cCode != null)
                    {
                        Country country = GetCountryId(cCode);
                        if (country != null)
                        {
                            usr.CountryId = country.CountryId;
                        }
                    }
                    HttpClient client = _api.Initial();
                    var postSignUp = client.PostAsJsonAsync<EasySystem.Models.Users>("Users/RegisterUser", usr);
                    postSignUp.Wait();
                    var result = postSignUp.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var usrData = JsonConvert.DeserializeObject<EasySystem.Models.Users>(res);

                        TempData["Number"] = null;
                        TempData["MentorCode"] = null;
                        TempData["cCode"] = null;
                        HttpContext.Session.SetString("Name", usrData.usrName);
                        HttpContext.Session.SetInt32("ID", usrData.usrId);
                        HttpContext.Session.SetInt32("Code", usrData.usrCode);
                        HttpContext.Session.SetString("Email", usrData.usrEmail);
                        HttpContext.Session.SetString("Status", usrData.usrStatus);
                        HttpContext.Session.SetInt32("Image", usrData.usrId);
                        HttpContext.Session.SetInt32("RefId", usrData.refId);
                        HttpContext.Session.SetInt32("RoleId", usrData.roleId);
                        HttpContext.Session.SetInt32("MemId", usrData.memId);
                        HttpContext.Session.SetInt32("CountryId", usrData.CountryId);
                        HttpContext.Session.SetString("Gender", usrData.usrGender);
                        HttpContext.Session.SetString("Expiry", usrData.ExpiryDate.ToString());
                        TempData["Name"] = HttpContext.Session.GetString("Name");

                        //Add SignUp Congratulations Amount
                        //var SerialNo = comm.GetSerialNo();
                        //EasySystem.Models.Transections trn = new EasySystem.Models.Transections();
                        //trn.tNarration = "" + usrData.usrName + "(" + usrData.usrCode + ")";
                        //trn.tReceiving = usrData.usrId;
                        //trn.tNumber = "Received on signup";
                        //string isOk = SignUpTransaction(trn);
                        //if (isOk != null)
                        //{
                        //    var SignUpAmount = comm.GetSkillsFee();
                        //    TempData["SignUp"] = "Congratulations " + TempData["Name"] + " you have received " + SignUpAmount.SignUpAmount + "/- pkr on Sign Up with us.";
                        //}
                        return RedirectToAction("Dashboard", "My");
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "Re-Type Password must be matched";
                    return View();
                }

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured to proceed with your request please try again later";
                return View();
            }
        }

        public IActionResult ResendCode(string PhoneNo)
        {
            try
            {
                var trimPhonNo = PhoneNo.Trim();
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("UserSignUpCodes/ResendCode?PhoneNo=" + PhoneNo);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Code Resent Successfully.This Code will be expire in 10 minutes";
                    return RedirectToAction("Code_Verification");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Code_Verification");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return RedirectToAction("Code_Verification");
            }

        }

        public IActionResult Login()
        {
            var GetCountries = GetCountryList();
            SelectList list = new SelectList(GetCountries, "CountryCode", "CountryName");
            ViewData["Country"] = list;
            return View();
        }
        [HttpPost]
        public ActionResult Login(EasySystem.Models.Users usr, string cCode)
        {
            try
            {
                var GetCountries = GetCountryList();
                SelectList list = new SelectList(GetCountries, "CountryCode", "CountryName");
                ViewData["Country"] = list;
                if (cCode != null && cCode != "")
                {
                    if (usr.usrPhone != "" && usr.usrPhone != null)
                    {
                        if (usr.usrPassword != "" && usr.usrPassword != null)
                        {
                            EasySystem.Models.PhoneNumber no = new EasySystem.Models.PhoneNumber();
                            no.Num = usr.usrPhone.Trim();
                            no.Code = cCode.Trim();
                            no.Password = usr.usrPassword.Trim();
                            //usr.usrPhone = usr.usrPhone.Trim();
                            //usr.usrPassword = usr.usrPassword.Trim();
                            HttpClient client = _api.Initial();
                            var postVerify = client.PostAsJsonAsync("Users/UserLogin", no);
                            postVerify.Wait();
                            var result = postVerify.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var res = result.Content.ReadAsStringAsync().Result;
                                var usrData = JsonConvert.DeserializeObject<EasySystem.Models.Users>(res);
                                HttpContext.Session.SetString("Name", usrData.usrName);
                                HttpContext.Session.SetInt32("ID", usrData.usrId);
                                HttpContext.Session.SetInt32("Code", usrData.usrCode);
                                HttpContext.Session.SetString("Email", usrData.usrEmail);
                                HttpContext.Session.SetString("Status", usrData.usrStatus);
                                HttpContext.Session.SetInt32("Image", usrData.usrId);
                                HttpContext.Session.SetInt32("RefId", usrData.refId);
                                HttpContext.Session.SetInt32("RoleId", usrData.roleId);
                                HttpContext.Session.SetInt32("MemId", usrData.memId);
                                HttpContext.Session.SetInt32("CountryId", usrData.CountryId);
                                HttpContext.Session.SetString("Gender", usrData.usrGender);
                                HttpContext.Session.SetString("Expiry", usrData.ExpiryDate.ToString());
                                TempData["Name"] = HttpContext.Session.GetString("Name");
                                return RedirectToAction("Dashboard", "My");
                            }
                            else
                            {
                                var res = result.Content.ReadAsStringAsync().Result;
                                var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                                TempData["Error"] = "" + errorMsg.message + "";
                                return View();
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Please enter password to login";
                            return View();
                        }

                    }
                    else
                    {
                        TempData["Error"] = "Please enter phone number to login";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "Country Code not found. please try again later";
                    return RedirectToAction("SignUp");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        public IActionResult Forget_Password()
        {
            var GetCountries = GetCountryList();
            SelectList list = new SelectList(GetCountries, "CountryCode", "CountryName");
            ViewData["Country"] = list;
            return View();
        }
        [HttpPost]
        public IActionResult Forget_Password(EasySystem.Models.Users usr, string cCode)
        {
            try
            {
                if (cCode != null && cCode != "")
                {
                    EasySystem.Models.PhoneNumber no = new EasySystem.Models.PhoneNumber();
                    no.Num = usr.usrPhone;
                    no.Code = cCode;
                    HttpClient client = _api.Initial();
                    var postSignUp = client.PostAsJsonAsync<EasySystem.Models.PhoneNumber>("Users/ForgetPassword", no);
                    postSignUp.Wait();
                    var result = postSignUp.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var PhoneNo = JsonConvert.DeserializeObject<EasySystem.Models.UserSignUpCode>(res);
                        TempData["Number"] = PhoneNo.usPhone;
                        TempData["Info"] = "An OTP send to your Mobile number for verification. This Code will be expired in 10 Minutes";
                        return RedirectToAction("Forget_Password_Code");
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return RedirectToAction("Forget_Password");
                    }
                }
                else
                {
                    TempData["Error"] = "Country Code not found. please try again later";
                    return RedirectToAction("Forget_Password");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured to proceed with your request please try again later";
                return RedirectToAction("Forget_Password");
            }
        }


        public IActionResult Forget_Password_Code()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Forget_Password_Code(EasySystem.Models.UserSignUpCode usr)
        {
            try
            {
                if (usr.usPhone != "" && usr.usPhone != null)
                {
                    if (usr.uscCode != 0)
                    {
                        HttpClient client = _api.Initial();
                        var postVerify = client.PostAsJsonAsync<EasySystem.Models.UserSignUpCode>("Users/VerifyForgetPasswordCode", usr);
                        postVerify.Wait();
                        var result = postVerify.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Number"] = usr.usPhone;
                            TempData["Info"] = "Enter New Password to proceed";
                            return RedirectToAction("Reset_Password");
                        }
                        else
                        {

                            var res = result.Content.ReadAsStringAsync().Result;
                            var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                            TempData["Error"] = "" + errorMsg.message + "";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Code must not be empty";
                        return View();
                    }

                }
                else
                {
                    TempData["Error"] = "Your Phone Number not Found Please try again later";
                    return View();

                }

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An Error occured to proceed. Please try again later";
                return View();
            }
        }

        public IActionResult ForgetResendCode(string PhoneNo)
        {
            try
            {
                var trimPhonNo = PhoneNo.Trim();
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("UserSignUpCodes/ResendCode?PhoneNo=" + PhoneNo);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Code Resent Successfully.This Code will be expire in 10 minutes";
                    return RedirectToAction("Forget_Password_Code");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Forget_Password_Code");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return RedirectToAction("Forget_Password_Code");
            }

        }

        public IActionResult Reset_Password()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Reset_Password(EasySystem.Models.Users usr, string rePass)
        {
            try
            {
                if (usr.usrPassword == rePass)
                {
                    if (usr.usrPhone != "" && usr.usrPhone != null)
                    {
                        HttpClient client = _api.Initial();
                        var postVerify = client.PostAsJsonAsync<EasySystem.Models.Users>("Users/ResetPassword", usr);
                        postVerify.Wait();
                        var result = postVerify.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Success"] = "Password Updated successfully. Please Login to proceed";
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            var res = result.Content.ReadAsStringAsync().Result;
                            var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                            TempData["Error"] = "" + errorMsg.message + "";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Your Phone Number not Found. Please try again later";
                        return View();

                    }
                }
                else
                {
                    TempData["Error"] = "Re-Type Password must be matched";
                    return View();
                }

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during updating password. Please try again later";
                return View();
            }
        }

        [SessionCheck]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [SessionCheck]
        [HttpPost]
        public IActionResult ChangePassword(ChangePassword password)
        {
            try
            {
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                password.Id = Id;
                HttpClient client = _api.Initial();
                var postData = client.PostAsJsonAsync("Users/ChangeUserPassword", password);
                postData.Wait();
                var result = postData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Password Updated successfully. Please Relogin to use the Application again";
                    return RedirectToAction("Logout");
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
                TempData["Error"] = "There is some error.please try again later.";
                return View();
            }
        }

        [SessionCheck]
        public IActionResult PersonalInfo()
        {
            try
            {

                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                HttpClient client = _api.Initial();
                var postData = client.GetAsync("Users/GetUser?id=" + Id.ToString());
                postData.Wait();
                var result = postData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var User = JsonConvert.DeserializeObject<EasySystem.Models.Users>(res);
                    bool IsVerified = CheckVerification();
                    if (IsVerified == true)
                    {
                        TempData["Verified"] = "Verified";
                    }
                    return View(User);
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
                TempData["Error"] = "There is some error.please try again later.";
                return View();
            }
        }

        [SessionCheck]
        [HttpPost]
        public IActionResult PersonalInfo(EasySystem.Models.Users usr, IFormFile perImage, string status)
        {
            try
            {
                if (perImage != null)
                {
                    string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                    string fileName = Guid.NewGuid().ToString() + "_" + perImage.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    perImage.CopyTo(new FileStream(filePath, FileMode.Create));
                    usr.usrImage = fileName;
                }
                int Id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                usr.usrId = Id;
                HttpClient client = _api.Initial();
                var postData = client.PostAsJsonAsync("Users/ChangeUserInfo", usr);
                postData.Wait();
                var result = postData.Result;
                if (result.IsSuccessStatusCode)
                {
                    HttpContext.Session.SetString("Name", usr.usrName);
                    HttpContext.Session.SetString("Gender", status);
                    TempData["Name"] = HttpContext.Session.GetString("Name");
                    TempData["Success"] = "Info Updated successfully";
                    return RedirectToAction("PersonalInfo");
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
                TempData["Error"] = "There is some error.please try again later.";
                return View();
            }
        }

        [SessionCheck]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult SessionOut()
        {
            TempData["Info"] = "Your Session period is out. Please SignIn to use services";
            return RedirectToAction("Login");
        }

        public EasySystem.Models.Users GetRefData(int id)
        {
            EasySystem.Models.Users users = new EasySystem.Models.Users();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("My/GetRefData?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<EasySystem.Models.Users>(res);
            }
            return users;
        }

        public List<Country> GetCountryList()
        {
            List<Country> li = new List<Country>();
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            HttpClient client = _api.Initial();
            var data = client.GetAsync("My/GetCountryList");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                li = JsonConvert.DeserializeObject<List<EasySystem.Models.Country>>(res);
            }
            return li;
        }

        public Country GetCountry()
        {
            Country li = new Country();
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            HttpClient client = _api.Initial();
            var data = client.GetAsync("My/GetCountry");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                li = JsonConvert.DeserializeObject<EasySystem.Models.Country>(res);
            }
            return li;
        }

        public Country GetCountryId(string Code)
        {
            Country country = new Country();
            string[] split = Code.Split('+');
            string splitCode = split[1];
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Users/GetCountryId?code=" + splitCode);
            data.Wait();
            var result = data.Result;
            var res = result.Content.ReadAsStringAsync().Result;
            country = JsonConvert.DeserializeObject<Country>(res);
            return country;
        }


        public IActionResult Skills(int Id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetMyMentorSkill?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<EasySystem.Models.UserSkills>>(res);
                    TempData["UserSkills"] = list;
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

        public IActionResult Whiteboard(int Id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetMyMentorWhiteBoard?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<EasySystem.Models.UserWhiteBoardVM>>(res);
                    TempData["UserWhiteBoard"] = list;
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
                return View();
            }
        }

        public IActionResult Blogs(int Id)
        {
            
            TempData["UsrId"] = Id;
//            try
//            {
//                HttpClient client = _api.Initial();
//                var GetBank = client.GetAsync("My/GetUsersBlogs?Id=" + Id.ToString());
//                GetBank.Wait();
//                var result = GetBank.Result;
//                if (result.IsSuccessStatusCode)
//                {
//                    var res = result.Content.ReadAsStringAsync().Result;
//                    var List = JsonConvert.DeserializeObject<List<EasySystem.Models.Blog>>(res);
//                    TempData["List"] = List;
//                }

//                else
//                {
//                    var res = result.Content.ReadAsStringAsync().Result;
//                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
//                    TempData["Error"] = "" + errorMsg.message + "";
//                }
//            }
//#pragma warning disable CS0168 // The variable 'ex' is declared but never used
//            catch (Exception ex)
//#pragma warning restore CS0168 // The variable 'ex' is declared but never used
//            {
//                TempData["Error"] = "An error occured during getting the request. Please try again later";
//                //return View();
//            }
            return View();
        }

        public List<EasySystem.Models.UserSkillVM> GetUsrSkills(int? id)
        {
            List<EasySystem.Models.UserSkillVM> List = new List<EasySystem.Models.UserSkillVM>();
            if (id != 0)
            {
                //var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetSkills = client.GetAsync("My/GetUserSkillInfo?id=" + id.ToString());
                GetSkills.Wait();
                var result = GetSkills.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    List = JsonConvert.DeserializeObject<List<EasySystem.Models.UserSkillVM>>(res);
                }
            }
            else
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetSkills = client.GetAsync("My/GetUserSkillInfo?id=" + Id.ToString());
                GetSkills.Wait();
                var result = GetSkills.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    List = JsonConvert.DeserializeObject<List<EasySystem.Models.UserSkillVM>>(res);
                }
            }


            return List;
        }
        public List<EasySystem.Models.UserWhiteBoardVM> userWhiteBoards(int? id)
        {
            List<EasySystem.Models.UserWhiteBoardVM> list = new List<EasySystem.Models.UserWhiteBoardVM>();
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
                    list = JsonConvert.DeserializeObject<List<EasySystem.Models.UserWhiteBoardVM>>(res);
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
                    list = JsonConvert.DeserializeObject<List<EasySystem.Models.UserWhiteBoardVM>>(res);
                }
            }

            return list;
        }

        public List<EasySystem.Models.Users> GetMyTeam(int? id)
        {
            List<EasySystem.Models.Users> List = new List<EasySystem.Models.Users>();
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
                    List = JsonConvert.DeserializeObject<List<EasySystem.Models.Users>>(res);

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
                    List = JsonConvert.DeserializeObject<List<EasySystem.Models.Users>>(res);
                    TempData["MyTeam"] = List;

                }
            }


            return List;
        }



        public List<EasySystem.Models.Blog> GetBlogs(int? id)
        {
            List<EasySystem.Models.Blog> List = new List<EasySystem.Models.Blog>();
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
                    List = JsonConvert.DeserializeObject<List<EasySystem.Models.Blog>>(res);

                    TempData["Blog"] = List;

                }
            }
            else
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("My/GetUsersBlogs?id=" + Id.ToString());
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    List = JsonConvert.DeserializeObject<List<EasySystem.Models.Blog>>(res);
                    TempData["Blog"] = List;

                }
            }


            return List;
        }

        public bool CheckVerification()
        {
            var Id = HttpContext.Session.GetInt32("ID");
            HttpClient client = _api.Initial();
            var GetData = client.GetAsync("Users/CheckVerification?id=" + Id.ToString());
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

        public string SignUpTransaction(EasySystem.Models.Transections trn)
        {
            var message = "";
            HttpClient client = _api.Initial();
            var PostData = client.PostAsJsonAsync("Transection/SignUpTransaction", trn);
            PostData.Wait();
            var result = PostData.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                var Msg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                message = "" + Msg.message + "";
                return message;
            }
            else
            {
                return null;
            }
        }

    }
}