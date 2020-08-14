using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using EasySystem.EasyAPI;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using EasySystemAPI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Users = EasySystem.Models.Users;
using EasySystem.General;

namespace EasySystem.Controllers
{
    [SessionCheck]
    public class MyTransactionController : Controller
    {
        EasySysAPI _api = new EasySysAPI();
        Common comm = new Common();
        readonly IConfiguration _configuration;
        string apiKey = "";
        public MyTransactionController(IConfiguration configuration)
        {
            _configuration = configuration;
            apiKey = _configuration.GetValue<string>("GetAPI:MyAPI");
        }

        public IActionResult PayMyFee()
        {
            int UsrId = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            int? MentorId = HttpContext.Session.GetInt32("RefId");
            int? MentorCode = HttpContext.Session.GetInt32("Code");
            var Mentor = GetRefData(Convert.ToInt32(MentorId));
            if (Mentor != null)
            {
                TempData["MentorName"] = Mentor.usrName + "-" + Mentor.usrCode;
            }

            //var Expiry = HttpContext.Session.GetString("Expiry");
            var Expiry = GetUser(UsrId);
            if (Expiry != null)
            {
                DateTime exp = Convert.ToDateTime(Expiry.ExpiryDate);
                if (exp.Year > 2000)
                {
                    if (exp < DateTime.Now)
                    {
                        var getLeftDays = exp - DateTime.Now;
                        TempData["Expired"] = "Fee Validity Expired on: " + exp.ToString("dd-MMM-yyyy") + ".";

                    }
                    else
                    {
                        var getLeftDays = exp - DateTime.Now;
                        TempData["Validity"] = "Fee Validity: " + exp.ToString("dd-MMM-yyyy") + ". " + getLeftDays.Days + " Days left.";

                    }
                }
            }


            //Getting Fee TotalAmount
            SkillsFee fee = new SkillsFee();
            fee = GetSkillsFee();
            TempData["Total"] = fee.Total;


            //JazzCash Code
            JazzCash jazz = new JazzCash();
            double Amount = fee.Total * 100;
            jazz.pp_Amount = "" + Amount + "";
            jazz.pp_TxnDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            jazz.pp_TxnExpiryDateTime = DateTime.Now.AddDays(+8).ToString("yyyyMMddHHmmss");
            jazz.pp_TxnRefNo = "T" + DateTime.Now.ToString("yyyyMMddHHmmss");
            jazz.pp_Version = "1.1";
            jazz.pp_TxnType = "";
            jazz.pp_Language = "EN";
            jazz.pp_SubMerchantID = "";
            jazz.pp_BankID = "TBANK";
            jazz.pp_ProductID = "RETL";
            jazz.pp_TxnCurrency = "PKR";
            jazz.pp_BillReference = "" + UsrId + "";
            jazz.pp_Description = "Thank you for using our services";
            jazz.ppmpf_1 = "1";
            jazz.ppmpf_2 = "2";
            jazz.ppmpf_3 = "3";
            jazz.ppmpf_4 = "4";
            jazz.ppmpf_5 = "5";

            //Local
            //jazz.salt = "2d8w96dwtv";
            //jazz.pp_MerchantID = "MC0793";
            //jazz.pp_Password = "5z1yu80v4w";
            //jazz.pp_ReturnURL = "http://localhost:60337/Home/Thankyou";


            //Live
            jazz.salt = "e39yxwc120";
            jazz.pp_MerchantID = "00168609";
            jazz.pp_Password = "g1f4e1h595";
            jazz.pp_ReturnURL = "https://universalskills.co/Home/Thankyou";


            //jazz.pp_SecureHash = sHash;
            TempData["JazzCash"] = jazz;

            //Check Country
            int cId = Convert.ToInt32(HttpContext.Session.GetInt32("CountryId"));
            var CountryName = comm.CountryName(cId);
            if(CountryName == "PAK")
            {
                TempData["IsPak"] = CountryName;
            }
            return View();
        }
        [HttpPost]
        public IActionResult PayNow()
        {
            try
            {
                var SerialNo = GetSerialNo();
                SkillsFee fee = new SkillsFee();
                fee = GetSkillsFee();
                EasySystemAPI.Models.Transections trn = new EasySystemAPI.Models.Transections();
                int? usrCode = HttpContext.Session.GetInt32("Code");
                string usrName = HttpContext.Session.GetString("Name");
                trn.tDate = DateTime.Now.Date;
                trn.tDateTime = DateTime.Now;
                trn.tStatus = "Pending";
                trn.tNarration = "" + usrName + "(" + usrCode + ")";
                trn.tPaying = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                trn.tReceiving = Convert.ToInt32(HttpContext.Session.GetInt32("RefId"));
                trn.TuitionAmount = fee.TuitionFee;
                trn.SoftServiceCharges = fee.SoftwdareFee;
                trn.ThirdPartyCharges = fee.ThirdPartyFee;
                trn.tNumber = "" + usrCode + "-" + DateTime.Now.ToString("ddMMyy") + "-" + SerialNo + "";

                HttpClient client = _api.Initial();
                var PostData = client.PostAsJsonAsync("Transection/AddTransection", trn);
                PostData.Wait();
                var result = PostData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Transaction Completed successfully";
                    return RedirectToAction("PayMyFee");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("PayMyFee");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured while adding the Transection Data.Please try again later";
                return RedirectToAction("PayMyFee");
            }

        }

        public IActionResult MyWallet(string Page)
        {
            if (Page != null)
            {
                TempData["Page"] = Page;
            }
            return View();
        }
        [HttpPost]
        public IActionResult MyWallet(string status, DateTime fromDate, DateTime toDate)
        {
            try
            {
                if (status != "" && status != null)
                {
                    List<EasySystem.Models.Transections> transectionsList = new List<Models.Transections>();
                    GetMyWallet wallet = new GetMyWallet();
                    wallet.ToDate = toDate;
                    wallet.FromDate = fromDate;
                    wallet.Status = status;
                    wallet.usrId = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                    HttpClient client = _api.Initial();
                    var data = client.PostAsJsonAsync("Transection/MyWallet", wallet);
                    data.Wait();
                    var result = data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        transectionsList = JsonConvert.DeserializeObject<List<EasySystem.Models.Transections>>(res);
                        if (transectionsList.Count > 0)
                        {
                            TempData["List"] = transectionsList;
                            TempData["Status"] = status;
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
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "There is some error please try again later";
            }


            return View();
        }

        public IActionResult MyMoneyDrawRequest()
        {
            try
            {
                double Balance = GetBalance();
                TempData["Balance"] = Balance;
                var GetBankInfo = PopulateUBInfo();
                SelectList list = new SelectList(GetBankInfo, "ubId", "ubDetail");
                ViewData["BankInfo"] = list;

                int id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                HttpClient client = _api.Initial();
                var data = client.GetAsync("Transection/CheckDrawRequest?id=" + id.ToString());
                data.Wait();
                var result = data.Result;
                if (result.IsSuccessStatusCode)
                {
                    return View();
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var info = JsonConvert.DeserializeObject<UserDrawRequest>(res);
                    TempData["IsRequest"] = "Yes";
                    TempData["Info"] = "You already requested for withdraw and it's status is " + info.udrStatus + ". Once this request is completed then you'll be able to with draw a new request.";
                }


                return View();
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "There is some error please try again later";
            }
            return View();
        }

        [HttpPost]
        public IActionResult MyMoneyDrawRequest(UserDrawRequest udr)
        {
            try
            {
                SkillsFee fee = new SkillsFee();
                fee = GetSkillsFee();
                if (udr.udrAmount >= fee.DrawAmount)
                {
                    int? usrCode = HttpContext.Session.GetInt32("Code");
                    udr.usrId = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                    if (udr.udrDetail == "")
                    {
                        udr.udrDetail = "No Description";
                    }
                    udr.udrCreatedDate = DateTime.Now;
                    udr.udrActionDate = DateTime.Now.AddYears(-100);
                    udr.udrStatus = "Pending";
                    var SerialNo = GetUDRSerialNo();
                    udr.udrCode = "" + usrCode + "-" + DateTime.Now.ToString("ddMMyy") + "-" + SerialNo + "";
                    //Calling API
                    HttpClient client = _api.Initial();
                    var PostData = client.PostAsJsonAsync("Transection/AddUDRequest", udr);
                    PostData.Wait();
                    var result = PostData.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Money Draw Request sent successfully";
                        double Balance = GetBalance();
                        TempData["Balance"] = Balance;
                        var GetBankInfo = PopulateUBInfo();
                        SelectList list = new SelectList(GetBankInfo, "ubId", "ubDetail");
                        ViewData["BankInfo"] = list;
                        bool IsRequest = CheckDrawRequest();
                        if (IsRequest == true)
                        {
                            TempData["IsRequest"] = "Yes";
                        }
                        return View();
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                        return RedirectToAction("MyMoneyDrawRequest");
                    }
                }
                else
                {
                    TempData["Error"] = "You can not width draw Amount because your Amount is less then width draw limit ("+ fee.DrawAmount + "). Please see terms and conditions section";
                    return RedirectToAction("MyMoneyDrawRequest");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "There is some error please try again later";
            }
            return RedirectToAction("MyMoneyDrawRequest");
        }

        public IActionResult GetMoneyDrawRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetMoneyDrawRequest(string status, DateTime fromDate, DateTime toDate)
        {
            try
            {
                
                if (status != "" && status != null)
                {
                    List<MoneyDrawVM> reqList = new List<MoneyDrawVM>();
                    GetMyWallet wallet = new GetMyWallet();
                    wallet.ToDate = toDate.AddHours(24);
                    wallet.FromDate = fromDate;
                    wallet.Status = status;
                    wallet.usrId = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
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
                            ViewData["List"] = reqList;
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
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "There is some error please try again later";
            }

            return View();
        }

        public IActionResult Coupan(Coupan cpn)
        {
            try
            {
                if (cpn.cCode != 0)
                {
                    //cpn.cAssignedUserId = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                    cpn.cUsedUserId = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
                    cpn.cUsedDate = DateTime.Now;
                    HttpClient client = _api.Initial();
                    var data = client.PostAsJsonAsync("Transection/UsedCoupan", cpn);
                    data.Wait();
                    var result = data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Coupon Value Used successfully";
                    }
                    else
                    {
                        var res = result.Content.ReadAsStringAsync().Result;
                        var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                        TempData["Error"] = "" + errorMsg.message + "";
                    }
                }
                else
                {
                    TempData["Error"] = "Please Enter Coupon Value";
                }

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "There is some error please try again later";
            }
            return RedirectToAction("PayMyFee");
        }


        public string GetSerialNo()
        {
            var SerialNo = "";
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Transection/GetSerialNo");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                SerialNo = JsonConvert.DeserializeObject<string>(res);
            }
            return SerialNo;
        }

        public string GetUDRSerialNo()
        {
            var SerialNo = "";
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Transection/GetUDRSerialNo");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                SerialNo = JsonConvert.DeserializeObject<string>(res);
            }
            return SerialNo;
        }

        public SkillsFee GetSkillsFee()
        {
            SkillsFee fee = new SkillsFee();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Transection/GetFeeAmount");
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                fee = JsonConvert.DeserializeObject<SkillsFee>(res);
            }
            return fee;
        }

        public double GetBalance()
        {
            int? Id = HttpContext.Session.GetInt32("ID");
            double Balance = 0;
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Transection/GetUserBalancec?=" + Id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                Balance = JsonConvert.DeserializeObject<double>(res);
            }
            return Balance;
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

        public Users GetUser(int id)
        {
            Users users = new Users();
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Users/GetUser?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<Users>(res);
            }
            return users;
        }

        public List<EasySystem.Models.UserBankInfo> PopulateUBInfo()
        {
            List<EasySystem.Models.UserBankInfo> li = new List<Models.UserBankInfo>();
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
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

        public bool CheckDrawRequest()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("ID"));
            HttpClient client = _api.Initial();
            var data = client.GetAsync("Transection/CheckDrawRequest?id=" + id.ToString());
            data.Wait();
            var result = data.Result;
            if (result.IsSuccessStatusCode)
            {
                return false;
            }
            else
            {
                var res = result.Content.ReadAsStringAsync().Result;
                var info = JsonConvert.DeserializeObject<UserDrawRequest>(res);
                return true;
            }
        }
    }
}