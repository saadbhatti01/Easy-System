using EasySystem.EasyAPI;
using EasySystem.General;
using EasySystem.Models;
using EasySystemAPI.Models;
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

namespace EasySystem.Controllers
{
    [SessionCheckForAdmin]
    public class AdminController : Controller
    {
        EasySysAPI _api = new EasySysAPI();
        Common comm = new Common();

        private readonly IHostingEnvironment HostingEnvironment;

        public AdminController(IHostingEnvironment hostingEnvironment)
        {
            this.HostingEnvironment = hostingEnvironment;
        }

        public IActionResult SignUpUsers()
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    HttpClient client = _api.Initial();
                    var data = client.GetAsync("UserSignUpCodes");
                    data.Wait();
                    var result = data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var List = JsonConvert.DeserializeObject<List<EasySystem.Models.UserSignUpCode>>(res);
                        TempData["List"] = List;
                        return View();
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Info"] = "" + errorMsg.message + "";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        public IActionResult Users()
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    HttpClient client = _api.Initial();
                    var data = client.GetAsync("Users/RegisteredUsers");
                    data.Wait();
                    var result = data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var List = JsonConvert.DeserializeObject<List<RegisterViewModel>>(res);
                        TempData["List"] = List;
                        return View();
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Info"] = "" + errorMsg.message + "";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        public IActionResult UserBankInfo()
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    HttpClient client = _api.Initial();
                    var data = client.GetAsync("My/GetAllBankInfo");
                    data.Wait();
                    var result = data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var List = JsonConvert.DeserializeObject<List<EasySystem.Models.UserBankVM>>(res);
                        TempData["List"] = List;
                        return View();
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Info"] = "" + errorMsg.message + "";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        public IActionResult EditBankInfo(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("My/AdminGetBankInfo?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var bankInfoList = JsonConvert.DeserializeObject<EasySystem.Models.UserBankInfo>(res);
                    var GetBankList = PopulateBank();
                    SelectList list = new SelectList(GetBankList, "BankId", "BankName");
                    ViewData["Bank"] = list;
                    return View(bankInfoList);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("UserBankInfo");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserBankInfo");
            }
        }

        [HttpPost]
        public IActionResult EditBankInfo(int id, EasySystem.Models.UserBankInfo usrBankInfo)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("My/AdminUpdateBankInfo?id=" + id.ToString(), usrBankInfo);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("UserBankInfo");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("UserBankInfo");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserBankInfo");
            }

        }



        public IActionResult Bank()
        {
            try
            {
                var GetCountries = GetCountryList();
                SelectList list = new SelectList(GetCountries, "CountryId", "CountryName");
                ViewData["Country"] = list;
                HttpClient client = _api.Initial();
                var GetBank = client.GetAsync("My/GetBankData");
                GetBank.Wait();
                var result = GetBank.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var bankList = JsonConvert.DeserializeObject<List<EasySystem.Models.BankVm>>(res);
                    TempData["List"] = bankList;
                }

                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
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
        [HttpPost]
        public IActionResult Bank(EasySystem.Models.Bank bnk)
        {
            try
            {
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("My/AddBank", bnk);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record added successfuly";
                    return RedirectToAction("Bank");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Bank");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Bank");
            }
        }

        public IActionResult DeleteBankInfo(int id)
        {
            try
            {
                HttpClient clientt = _api.Initial();
                var DelbankInfo = clientt.DeleteAsync("My/DelBank?id=" + id.ToString());
                DelbankInfo.Wait();
                var result = DelbankInfo.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record Deleted successfully";
                    return RedirectToAction("Bank");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Bank");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Bank");
            }

        }

        public IActionResult EditBank(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("My/AdminGetBank?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var bankInfoList = JsonConvert.DeserializeObject<EasySystem.Models.Bank>(res);
                    var GetCountries = GetCountryList();
                    SelectList list = new SelectList(GetCountries, "CountryId", "CountryName");
                    ViewData["Country"] = list;
                    return View(bankInfoList);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Bank");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Bank");
            }

        }

        [HttpPost]
        public IActionResult EditBank(int id, EasySystem.Models.Bank bank)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("My/UpdateBank?id=" + id.ToString(), bank);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("Bank");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Bank");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Bank");
            }
        }

        public IActionResult Country()
        {

            try
            {
                HttpClient client = _api.Initial();
                var GetBank = client.GetAsync("My/GetCountryData");
                GetBank.Wait();
                var result = GetBank.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var List = JsonConvert.DeserializeObject<List<EasySystem.Models.Country>>(res);
                    TempData["List"] = List;
                }

                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
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

        [HttpPost]
        public IActionResult Country(EasySystem.Models.Country country)
        {
            try
            {
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("My/AddCountry", country);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record added successfuly";
                    return RedirectToAction("Country");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Country");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Country");
            }
        }

        public IActionResult DeleteCountry(int id)
        {
            try
            {
                HttpClient clientt = _api.Initial();
                var DelbankInfo = clientt.DeleteAsync("My/DelCountry?id=" + id.ToString());
                DelbankInfo.Wait();
                var result = DelbankInfo.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record Deleted successfully";
                    return RedirectToAction("Country");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Country");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Country");
            }

        }

        public IActionResult EditCountry(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("My/AdminGetCountry?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var Info = JsonConvert.DeserializeObject<EasySystem.Models.Country>(res);
                    return View(Info);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Country");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Country");
            }

        }

        [HttpPost]
        public IActionResult EditCountry(int id, EasySystem.Models.Country country)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("My/UpdateCountry?id=" + id.ToString(), country);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("Country");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Country");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Country");
            }
        }




        public IActionResult UserVerificationType()
        {
            try
            {
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("UserVerification/GetVerificationTypeData");
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var List = JsonConvert.DeserializeObject<List<UserVerificationType>>(res);
                    TempData["List"] = List;
                }

                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
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
        [HttpPost]
        public IActionResult UserVerificationType(UserVerificationType data)
        {
            try
            {
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("UserVerification/AddVerificationType", data);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record added successfuly";
                    return RedirectToAction("UserVerificationType");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("UserVerificationType");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserVerificationType");
            }
        }

        public IActionResult DeleteUserVerificationType(int id)
        {
            try
            {
                HttpClient clientt = _api.Initial();
                var DelbankInfo = clientt.DeleteAsync("UserVerification/DelVerificationType?id=" + id.ToString());
                DelbankInfo.Wait();
                var result = DelbankInfo.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record Deleted successfully";
                    return RedirectToAction("UserVerificationType");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("UserVerificationType");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserVerificationType");
            }

        }

        public IActionResult EditUserVerificationType(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("UserVerification/GetVerificationType?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<UserVerificationType>(res);
                    return View(data);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("UserVerificationType");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserVerificationType");
            }

        }

        [HttpPost]
        public IActionResult EditUserVerificationType(int id, UserVerificationType data)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("UserVerification/UpdateVerificationType?id=" + id.ToString(), data);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("UserVerificationType");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("UserVerificationType");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserVerificationType");
            }
        }



        public IActionResult ApproveTransaction()
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    HttpClient client = _api.Initial();
                    var data = client.GetAsync("Transection/GetTransaction");
                    data.Wait();
                    var result = data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var reqList = JsonConvert.DeserializeObject<List<AppTransaction>>(res);
                        if (reqList.Count > 0)
                        {
                            TempData["List"] = reqList;
                            return View();
                        }
                        else
                        {
                            TempData["Info"] = "No Record pending transaction record found";
                            return View();
                        }
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Info"] = "" + errorMsg.message + "";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        public IActionResult VerifyTransaction(int id)
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    HttpClient client = _api.Initial();
                    var data = client.GetAsync("Transection/ApprovedTransaction?id=" + id.ToString());
                    data.Wait();
                    var result = data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Transaction Verified successfully";
                        return RedirectToAction("ApproveTransaction");
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Info"] = "" + errorMsg.message + "";
                        return RedirectToAction("ApproveTransaction");
                    }
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("ApproveTransaction");
            }
        }

        public IActionResult UserPayment()
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    List<UserCode> cList = new List<UserCode>();
                    cList = GetCodes();
                    SelectList list = new SelectList(cList, "usrCode", "usrName");
                    ViewData["Codes"] = list;
                    return View();
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
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
        [HttpPost]
        public IActionResult UserPayment(string status, DateTime fromDate, DateTime toDate, string Code)
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    List<UserCode> cList = new List<UserCode>();
                    cList = GetCodes();
                    SelectList list = new SelectList(cList, "usrCode", "usrCode");
                    ViewData["Codes"] = list;

                    if (status != "" && status != null)
                    {
                        List<MoneyDrawVM> reqList = new List<MoneyDrawVM>();
                        GetMyWallet wallet = new GetMyWallet();
                        wallet.ToDate = toDate.AddHours(24);
                        wallet.FromDate = fromDate;
                        wallet.Status = status;
                        if (Code != null)
                        {
                            wallet.usrCode = Convert.ToInt32(Code);
                        }
                        //wallet.usrId = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                        HttpClient client = _api.Initial();
                        var data = client.PostAsJsonAsync("Transection/GetDrawRequest", wallet);
                        data.Wait();
                        var result = data.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var res = result.Content.ReadAsStringAsync().Result;
                            reqList = JsonConvert.DeserializeObject<List<MoneyDrawVM>>(res);
                            if (reqList.Count > 0)
                            {
                                TempData["List"] = reqList;
                                return View();
                            }
                            else
                            {
                                TempData["Info"] = "No Record found on selected parameters";
                                return View();
                            }
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
                        TempData["Error"] = "Please select Status";
                    }

                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "There is some error please try again later";
            }

            return View();
        }

        public IActionResult ApproveDrawRequest(int id, int usrId)
        {
            try
            {

                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    HttpClient client = _api.Initial();
                    var UpdateData = client.GetAsync("Transection/GetDrawInfo?id=" + id.ToString());
                    UpdateData.Wait();
                    var result = UpdateData.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        MoneyDrawVM moneyDrawVM = new MoneyDrawVM();
                        var res = result.Content.ReadAsStringAsync().Result;
                        moneyDrawVM = JsonConvert.DeserializeObject<MoneyDrawVM>(res);


                        var GetBankInfo = PopulateUBInfo(usrId);
                        SelectList list = new SelectList(GetBankInfo, "ubId", "ubDetail");
                        ViewData["BankInfo"] = list;
                        return View(moneyDrawVM);
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return RedirectToAction("UserPayment");
                    }

                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserPayment");
            }
        }

        [HttpPost]
        public IActionResult ApproveDrawRequest(int id, MoneyDrawVM drawVM, IFormFile file)
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    if (file != null)
                    {
                        string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                        string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadFolder, fileName);
                        file.CopyTo(new FileStream(filePath, FileMode.Create));
                        drawVM.udrImage = fileName;
                    }
                    HttpClient client = _api.Initial();
                    var UpdateData = client.PutAsJsonAsync("Transection/ApproveDrawRequest?id=" + id.ToString(), drawVM);
                    UpdateData.Wait();
                    var result = UpdateData.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Data Updated successfully";
                        return RedirectToAction("UserPayment");
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return RedirectToAction("UserPayment");
                    }
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
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

        public IActionResult AssignCoupanToUser()
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    List<UserCode> cList = new List<UserCode>();
                    cList = GetCodesForCoupan();
                    SelectList list = new SelectList(cList, "usrId", "usrName");
                    ViewData["Codes"] = list;

                    //Get Coupan Data
                    List<EasySystem.Models.CoupanVM> coupans = new List<EasySystem.Models.CoupanVM>();
                    HttpClient client = _api.Initial();
                    var data = client.GetAsync("Transection/GetCoupan");
                    data.Wait();
                    var result = data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        coupans = JsonConvert.DeserializeObject<List<EasySystem.Models.CoupanVM>>(res);
                        TempData["List"] = coupans;
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return View();
                    }



                    return View();
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
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

        [HttpPost]
        public IActionResult AssignCoupanToUser(Coupan cpn)
        {
            try
            {
                int ID = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                cpn.cCreatedBy = ID;
                HttpClient client = _api.Initial();
                var postData = client.PostAsJsonAsync("Transection/AddCoupan", cpn);
                postData.Wait();
                var result = postData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Coupans Created successfully";
                    return RedirectToAction("AssignCoupanToUser");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("AssignCoupanToUser");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occure while adding the Coupan";
                return RedirectToAction("AssignCoupanToUser");
            }
        }

        public IActionResult UserHelpingMaterial()
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    HttpClient client = _api.Initial();
                    var GetData = client.GetAsync("MyHelpingMaterial/GetAllHelpingMaterial");
                    GetData.Wait();
                    var result = GetData.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var list = JsonConvert.DeserializeObject<List<EasySystem.Models.UserHelpingMaterial>>(res);
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
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
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

        [HttpPost]
        public IActionResult UserHelpingMaterial(EasySystem.Models.UserHelpingMaterial hm, IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    if (hm.umsType == "video")
                    {

                    }
                    else
                    {
                        string uploadFolder = "";
                        if (hm.umsFor == "Male")
                        {
                            uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/UserMarketingMaterial/male");
                        }
                        else if (hm.umsFor == "Female")
                        {
                            uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/UserMarketingMaterial/female");
                        }
                        else if (hm.umsFor == "Company")
                        {
                            uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/UserMarketingMaterial/company");
                        }
                        else if (hm.umsFor == "Personal")
                        {
                            uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/UserMarketingMaterial/personal");
                        }
                        else if (hm.umsFor == "All")
                        {
                            uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/All");
                        }

                        //string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string fileName = file.FileName;
                        string filePath = Path.Combine(uploadFolder, fileName);
                        file.CopyTo(new FileStream(filePath, FileMode.Create));
                        hm.umsPath = fileName;
                    }

                }
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("MyHelpingMaterial/AddHelpingMaterial", hm);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record added successfuly";
                    return RedirectToAction("UserHelpingMaterial");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("UserHelpingMaterial");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("BankInfo");
            }
        }

        public IActionResult DeleteUserHelpingMaterial(int id)
        {
            try
            {
                HttpClient clientt = _api.Initial();
                var DelInfo = clientt.DeleteAsync("MyHelpingMaterial/DelHelpingMaterial?id=" + id.ToString());
                DelInfo.Wait();
                var result = DelInfo.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record Deleted successfully";
                    return RedirectToAction("UserHelpingMaterial");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("UserHelpingMaterial");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserHelpingMaterial");
            }

        }


        public IActionResult EditUserHelpingMaterial(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("MyHelpingMaterial/GetHelpingMaterial?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var Info = JsonConvert.DeserializeObject<EasySystem.Models.UserHelpingMaterial>(res);
                    return View(Info);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("UserHelpingMaterial");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserHelpingMaterial");
            }

        }

        [HttpPost]
        public IActionResult EditUserHelpingMaterial(int id, EasySystem.Models.UserHelpingMaterial hm, IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    string uploadFolder = "";
                    if (hm.umsFor == "Male")
                    {
                        uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/UserMarketingMaterial/male");
                    }
                    else if (hm.umsFor == "Female")
                    {
                        uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/UserMarketingMaterial/female");
                    }
                    else if (hm.umsFor == "Company")
                    {
                        uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/UserMarketingMaterial/company");
                    }
                    else if (hm.umsFor == "Personal")
                    {
                        uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/UserMarketingMaterial/personal");
                    }
                    else if (hm.umsFor == "All")
                    {
                        uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/All");
                    }

                    string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    file.CopyTo(new FileStream(filePath, FileMode.Create));
                    hm.umsPath = fileName;
                }
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("MyHelpingMaterial/UpdateHelpingMaterial?id=" + id.ToString(), hm);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("UserHelpingMaterial");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("UserHelpingMaterial");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserHelpingMaterial");
            }

        }


        public IActionResult UserVerificationRequests()
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    HttpClient client = _api.Initial();
                    var data = client.GetAsync("UserVerification/GetVerificationData");
                    data.Wait();
                    var result = data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var reqList = JsonConvert.DeserializeObject<List<AccountVerifyVM>>(res);
                        if (reqList.Count > 0)
                        {
                            TempData["List"] = reqList;
                            return View();
                        }
                        else
                        {
                            TempData["Info"] = "No Record found";
                            return View();
                        }
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Info"] = "" + errorMsg.message + "";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        public IActionResult VerifyUserDocument(int id)
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    //int ID = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                    //data.uvStatusBy = ID;
                    HttpClient client = _api.Initial();
                    var updatedata = client.GetAsync("UserVerification/GetUserVerificationData?id=" + id.ToString());
                    updatedata.Wait();
                    var result = updatedata.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var reqList = JsonConvert.DeserializeObject<AccountVerifyVM>(res);

                        //TempData["List"] = reqList;
                        return View(reqList);
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Info"] = "" + errorMsg.message + "";
                        return RedirectToAction("UserVerificationRequests");
                    }
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserVerificationRequests");
            }
        }

        [HttpPost]
        public IActionResult VerifyUserDocument(int id, AccountVerifyVM vm)
        {
            try
            {
                int RoleId = Convert.ToInt32(HttpContext.Session.GetInt32("RoleId"));
                if (RoleId == 1)
                {
                    int ID = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                    UserVerification data = new UserVerification();
                    data.uvId = vm.uvId;
                    data.uvStatusBy = ID;
                    data.uvStatus = vm.uvStatus;
                    data.uvStatusRemarks = vm.uvStatusRemarks;
                    HttpClient client = _api.Initial();
                    var Postdata = client.PostAsJsonAsync("UserVerification/UpdateVerificationData", data);
                    Postdata.Wait();
                    var result = Postdata.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "User verification Docements updated successfully";
                        return RedirectToAction("UserVerificationRequests");
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Info"] = "" + errorMsg.message + "";
                        return RedirectToAction("UserVerificationRequests");
                    }
                }
                else
                {
                    TempData["Error"] = "You are not allowed to Enter that Page";
                    return RedirectToAction("Logout", "Users");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("UserVerificationRequests");
            }
        }

        public IActionResult InfoSentData()
        {
            try
            {
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("Transection/GetInfoSend");
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    List<InfoSendVM> infoSendVMs = new List<InfoSendVM>();
                    var res = result.Content.ReadAsStringAsync().Result;
                    infoSendVMs = JsonConvert.DeserializeObject<List<InfoSendVM>>(res);
                    TempData["List"] = infoSendVMs;
                }

                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
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

        public IActionResult InfoSentDataForCompany()
        {
            try
            {
                HttpClient client = _api.Initial();
                var GetData = client.GetAsync("Transection/GetInfoSendForCompany");
                GetData.Wait();
                var result = GetData.Result;
                if (result.IsSuccessStatusCode)
                {
                    List<InfoSend_CompanyVM> infoSendVMs = new List<InfoSend_CompanyVM>();
                    var res = result.Content.ReadAsStringAsync().Result;
                    infoSendVMs = JsonConvert.DeserializeObject<List<InfoSend_CompanyVM>>(res);
                    TempData["List"] = infoSendVMs;
                }

                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
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


        public ActionResult SkillType()
        {
            try
            {
                List<SkillTypeVM> datalist = new List<SkillTypeVM>();
                datalist = GetSkillTypeList();


                SelectList slist = new SelectList(datalist, "StId", "StName");
                ViewData["Skills"] = slist;

                //get Data for Search
                var getData = datalist.Where(s => s.StId == s.SubType).ToList();
                SelectList list = new SelectList(getData, "StId", "StName");
                ViewData["SearchSkills"] = list;
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

        public ActionResult _SkillTypeData(int id)
        {
            try
            {
                List<SkillTypeVM> datalist = new List<SkillTypeVM>();
                datalist = GetSkillTypeListById(id);
                if (datalist.Count > 0)
                {
                    TempData["List"] = datalist;
                }
            }
            catch (Exception ex)
            {

            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult SkillType(SkillType data, IFormFile Image, IFormFile cImage)
        {
            try
            {
                if (Image != null)
                {
                    string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/SkillType/Thumbnail");
                    string fileName = Image.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    Image.CopyTo(new FileStream(filePath, FileMode.Create));
                    data.StImage = fileName;
                }
                if (cImage != null)
                {
                    string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/SkillType/Cover");
                    string fileName = cImage.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    cImage.CopyTo(new FileStream(filePath, FileMode.Create));
                    data.StCoverImage = fileName;
                }

                ////Code for Enter 10000 Entries for testing purpose
                //var Name = data.StName;
                //var Detail = data.StDetail;
                //List<SkillType> sList = new List<SkillType>();
                //for (int i = 1; i < 10000; i++)
                //{
                //    data.StName = Name + "-" + comm.RandomString(5);
                //    data.StDetail = Detail + "-" + comm.RandomString(15);
                //    sList.Add(data);
                //}
                var Id = (int)HttpContext.Session.GetInt32("ID");
                data.SubType = data.StId;
                data.StId = 0;
                data.CreatedBy = Id;
                HttpClient client = _api.Initial();
                var postData = client.PostAsJsonAsync("My/AddSkillType", data);
                postData.Wait();
                var result = postData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Skill Type added successfully";
                    return RedirectToAction("SkillType");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("SkillType");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occure while adding the Coupan";
                return RedirectToAction("SkillType");
            }
        }

        public IActionResult DeleteSkillType(int id)
        {
            try
            {
                HttpClient clientt = _api.Initial();
                var data = clientt.DeleteAsync("My/DelSkillType?id=" + id.ToString());
                data.Wait();
                var result = data.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record Deleted successfully";
                    return RedirectToAction("SkillType");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("SkillType");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("SkillType");
            }

        }

        public IActionResult EditSkillType(int id)
        {
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
                    var datalist = PopulateSkillType();
                    SelectList slist = new SelectList(datalist, "SubType", "StName");
                    ViewData["Skills"] = slist;
                    return View(data);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("SkillType");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("SkillType");
            }

        }

        [HttpPost]
        public IActionResult EditSkillType(int id, string SubType, SkillType data, IFormFile Image, IFormFile cImage)
        {
            try
            {
                if (Image != null)
                {
                    string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/SkillType/Thumbnail");
                    string fileName = Image.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    Image.CopyTo(new FileStream(filePath, FileMode.Create));
                    data.StImage = fileName;
                }
                if (cImage != null)
                {
                    string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/SkillType/Cover");
                    string fileName = cImage.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    cImage.CopyTo(new FileStream(filePath, FileMode.Create));
                    data.StCoverImage = fileName;
                }

                //data.SubType = data.StId;
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("My/UpdateSkillType?id=" + id.ToString(), data);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("SkillType");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("SkillType");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("SkillType");
            }
        }

        public IActionResult SkillName()
        {
            return View();
        }

        public IActionResult _SkillNameData()
        {
            var getNames = comm.SkillTypeName();
            TempData["sName"] = getNames;
            return PartialView();
        }

        public IActionResult _UpdateName(int id)
        {
            HttpClient client = _api.Initial();
            var UpdateData = client.GetAsync("Skills/EditSkillType?id=" + id.ToString());
            UpdateData.Wait();
            var result = UpdateData.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<SkillType>(res);
                return PartialView(data);
            }
            else
            {
                var res = result.Content.ReadAsStringAsync().Result;
                var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                //TempData["Error"] = "" + errorMsg.message + "";
                return PartialView();
            }
        }

        public IActionResult UpdateSkillName(SkillType data)
        {
            HttpClient client = _api.Initial();
            var UpdateData = client.PostAsJsonAsync("Skills/UpdateSkillTypeName", data);
            UpdateData.Wait();
            var result = UpdateData.Result;
            if (result.IsSuccessStatusCode)
            {
                return Content("");
            }
            else
            {
                var res = result.Content.ReadAsStringAsync().Result;
                var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                TempData["Error"] = "" + errorMsg.message + "";
                return Content("Not Updated");
            }
        }

        public IActionResult SkillMaterial()
        {
            try
            {
                List<SkillType> datalist = new List<SkillType>();
                datalist = GetSkillList();
                SelectList slist = new SelectList(datalist, "StId", "StName");
                ViewData["Skills"] = slist;
            }
            catch (Exception)
            {

            }
            return View();
        }

        [HttpPost]
        public IActionResult SkillMaterial(SkillMaterial data)
        {
            try
            {
                var Id = (int)HttpContext.Session.GetInt32("ID");
                data.CreatedBy = Id;
                HttpClient client = _api.Initial();
                var postData = client.PostAsJsonAsync("Skills/AddSkillMaterial", data);
                postData.Wait();
                var result = postData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var skill = JsonConvert.DeserializeObject<SkillMaterial>(res);



                    List<SkillMaterialDetail> sList = new List<SkillMaterialDetail>();
                    if (TempData["Url"] != null)
                    {
                        var Url = TempData["Url"].ToString();
                        List<string> Urls = Url.Split(',').ToList();
                        if (Urls.Count > 0)
                        {
                            foreach (var item in Urls)
                            {
                                if (item != "")
                                {
                                    SkillMaterialDetail detail = new SkillMaterialDetail();
                                    detail.SmdURL = item;
                                    detail.SmId = skill.SmId;
                                    detail.StId = data.StId;
                                    sList.Add(detail);
                                }
                            }

                            var pData = client.PostAsJsonAsync("Skills/AddSkillMaterialDetail", sList);
                            pData.Wait();
                            var resultt = pData.Result;
                            if (resultt.IsSuccessStatusCode)
                            {
                                return Content("");
                            }
                            else
                            {
                                var ress = resultt.Content.ReadAsStringAsync().Result;
                                var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                                //TempData["Error"] = "" + errorMsg.message + "";
                                return Content("" + errorMsg.message + "");
                            }
                        }
                    }

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
                return Content("Not Added");
            }
            return Content("Not Added");
        }

        public ActionResult _SkillMaterialData(int id)
        {
            try
            {
                List<SkillDetailVM> datalist = new List<SkillDetailVM>();
                datalist = GetSkillMaterialListById(id);
                if (datalist.Count > 0)
                {
                    TempData["List"] = datalist;
                }
            }
            catch (Exception ex)
            {

            }
            return PartialView();
        }

        public IActionResult DeleteSkillMaterialData(int id)
        {
            try
            {

                HttpClient clientt = _api.Initial();
                var data = clientt.DeleteAsync("Skills/DelSkillMaterial?id=" + id.ToString());
                data.Wait();
                var result = data.Result;
                if (result.IsSuccessStatusCode)
                {
                    //TempData["Success"] = "Record Deleted successfully";
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
                //TempData["Error"] = "An error occured during getting the request. Please try again later";
                return Content("Not Deleted");
            }

        }

        public ActionResult _UpdateSkillMaterial(int id)
        {
            try
            {
                try
                {
                    HttpClient client = _api.Initial();
                    var UpdateData = client.GetAsync("Skills/EditSkillMaterial?id=" + id.ToString());
                    UpdateData.Wait();
                    var result = UpdateData.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert.DeserializeObject<SkillMaterial>(res);
                        List<SkillType> datalist = new List<SkillType>();
                        datalist = GetSkillList();
                        SelectList slist = new SelectList(datalist, "StId", "StName");
                        ViewData["Skills"] = slist;
                        return PartialView(data);
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return RedirectToAction("SkillMaterial");
                    }
                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                {
                    TempData["Error"] = "An error occured during getting the request. Please try again later";
                    return RedirectToAction("SkillMaterial");
                }
            }
            catch (Exception)
            {

            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateSkillMaterial(SkillMaterial data)
        {
            try
            {
                HttpClient client = _api.Initial();
                int id = data.SmId;
                var UpdateData = client.PutAsJsonAsync("Skills/UpdateSkillMaterial?id=" + id.ToString(), data);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {

                    List<SkillMaterialDetail> sList = new List<SkillMaterialDetail>();
                    if (TempData["Url"] != null)
                    {
                        var Url = TempData["Url"].ToString();
                        List<string> Urls = Url.Split(',').ToList();
                        if (Urls.Count > 0)
                        {
                            foreach (var item in Urls)
                            {
                                if (item != "")
                                {
                                    SkillMaterialDetail detail = new SkillMaterialDetail();
                                    detail.SmdURL = item;
                                    detail.SmId = data.SmId;
                                    detail.StId = data.StId;
                                    sList.Add(detail);
                                }
                            }

                            var pData = client.PostAsJsonAsync("Skills/AddSkillMaterialDetail", sList);
                            pData.Wait();
                            var resultt = pData.Result;
                            if (resultt.IsSuccessStatusCode)
                            {
                                return Content("");
                            }
                            else
                            {
                                var ress = resultt.Content.ReadAsStringAsync().Result;
                                var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(ress);
                                return Content("" + errorMsg.message + "");
                            }
                        }
                    }


                    return Content("");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return Content("Data Not Updated");
                }
            }
            catch (Exception)
            {
                //TempData["Error"] = "An error occured during getting the request. Please try again later";
                return Content("Not Updated");
            }


        }

        public ActionResult SkillMaterialDetail(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var data = client.GetAsync("Skills/GetSkillMaterialAdmin?id=" + id.ToString());
                data.Wait();
                var result = data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var getdata = JsonConvert.DeserializeObject<SkillDetailVM>(res);

                    if (getdata != null)
                    {
                        TempData["Material"] = getdata;
                        List<SkillMaterialDetail> detail = new List<SkillMaterialDetail>();
                        detail = comm.GetSkillMaterialDetailAdmin(getdata.SmId);
                        if (detail.Count > 0)
                        {
                            TempData["MaterialDetail"] = detail;
                        }

                    }

                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("SkillMaterial");
                }
            }
            catch (Exception)
            {
            }
            return View();
        }

        public ActionResult DelMaterialVideo(int id, int SmId)
        {
            try
            {
                bool isDeleted = false;
                isDeleted = comm.DelMaterialVideo(id);
                if (isDeleted)
                {
                    TempData["Success"] = "Video deleted successfully";
                    return RedirectToAction("SkillMaterialDetail", new { id = SmId });
                }
                else
                {
                    TempData["Error"] = "Video not deleted";
                    return RedirectToAction("SkillMaterialDetail", new { id = SmId });
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Video not deleted";
                return RedirectToAction("SkillMaterialDetail", new { id = SmId });
            }
        }

        public ActionResult LoginLogs()
        {
            try
            {
                List<UserCode> cList = new List<UserCode>();
                cList = GetCodesForCoupan();
                SelectList list = new SelectList(cList, "usrId", "usrName");
                ViewData["Users"] = list;
            }
            catch (Exception)
            {

            }
            return View();
        }

        public ActionResult _LoginLogs(int? UsrId, string fromDate, string toDate)
        {
            try
            {
                if (UsrId != null)
                {

                }
                else if (fromDate != null && toDate != null)
                {

                }
                else
                {

                }
            }
            catch (Exception)
            {

            }
            return PartialView();
        }

        public List<UserCode> GetCodesForCoupan()
        {
            List<UserCode> cList = new List<UserCode>();
            List<UserCode> coupanList = new List<UserCode>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Transection/GetAllUserCode");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                cList = JsonConvert.DeserializeObject<List<UserCode>>(res);
            }
            return cList;
        }

        public List<UserCode> GetCodes()
        {
            List<UserCode> cList = new List<UserCode>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Transection/GetUserCode");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                cList = JsonConvert.DeserializeObject<List<UserCode>>(res);
            }
            return cList;
        }

        public List<EasySystem.Models.Bank> PopulateBank()
        {
            List<EasySystem.Models.Bank> bankList = new List<EasySystem.Models.Bank>();
            HttpClient client = _api.Initial();
            var GetBank = client.GetAsync("My/PopulateBank");
            GetBank.Wait();
            var result = GetBank.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                bankList = JsonConvert.DeserializeObject<List<EasySystem.Models.Bank>>(res);
            }

            return bankList;
        }

        public List<EasySystem.Models.UserBankInfo> PopulateUBInfo(int id)
        {
            List<EasySystem.Models.UserBankInfo> li = new List<Models.UserBankInfo>();
            //int id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Transection/PopulateUBInfo?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                li = JsonConvert.DeserializeObject<List<EasySystem.Models.UserBankInfo>>(res);

            }
            return li;
        }

        public List<EasySystem.Models.Country> GetCountryList()
        {
            List<EasySystem.Models.Country> li = new List<EasySystem.Models.Country>();
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

        public List<SkillTypeVM> GetSkillTypeList()
        {
            List<SkillTypeVM> li = new List<SkillTypeVM>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("My/GetSkillType");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                li = JsonConvert.DeserializeObject<List<SkillTypeVM>>(res);
            }
            return li;
        }

        public List<SkillType> GetSkillList()
        {
            List<SkillType> li = new List<SkillType>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Skills/GetSkillTypeList");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                li = JsonConvert.DeserializeObject<List<SkillType>>(res);
            }
            return li;
        }

        public List<SkillTypeVM> GetSkillTypeListById(int id)
        {
            List<SkillTypeVM> li = new List<SkillTypeVM>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("My/GetSkillTypeById?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                li = JsonConvert.DeserializeObject<List<SkillTypeVM>>(res);
            }
            return li;
        }

        public List<SkillDetailVM> GetSkillMaterialListById(int id)
        {
            List<SkillDetailVM> li = new List<SkillDetailVM>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Skills/GetMaterialTypeById?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                li = JsonConvert.DeserializeObject<List<SkillDetailVM>>(res);
            }
            return li;
        }

        public List<SkillType> PopulateSkillType()
        {
            List<SkillType> li = new List<SkillType>();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("My/PopulateSkillType");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                li = JsonConvert.DeserializeObject<List<SkillType>>(res);
            }
            return li;
        }

        public string VideoLink(string Link)
        {
            if (Link != null)
            {
                SkillMaterialDetail skill = new SkillMaterialDetail();
                skill.SmdURL = Link;
                TempData["Url"] = TempData["Url"] + "," + Link;
                return "";
            }

            if (Link == null)
            {
                var Url = TempData["Url"].ToString();
                List<string> Urls = Url.Split(',').ToList();
                foreach (var item in Urls)
                {
                    var u = item;
                }
            }

            else
            {
                return "Not Added";
            }
            return "Not Added";
        }

    }
}