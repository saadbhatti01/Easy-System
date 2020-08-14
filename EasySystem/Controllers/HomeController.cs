using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasySystem.Models;
using EasySystem.EasyAPI;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using EasySystemAPI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using EasySystem.General;
using Microsoft.AspNetCore.Hosting;

using System.IO;
using Microsoft.AspNetCore.Hosting.Internal;

namespace EasySystem.Controllers
{
    public class HomeController : Controller
    {
        EasySysAPI _api = new EasySysAPI();
        Common com = new Common();
        private readonly IHostingEnvironment HostingEnvironment;
        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            this.HostingEnvironment = hostingEnvironment;
        }

        //[SessionCheck]
        public IActionResult Index()
        {
            List<TrainingInfo> DataList = new List<TrainingInfo>();
            DataList = GetTraining();
            if (DataList.Count > 0)
            {
                if(DataList.Count > 6)
                {
                    IEnumerable<TrainingInfo> DataList6 = new List<TrainingInfo>();
                    DataList6 = DataList.Take(6);
                    TempData["List"] = DataList6;
                }
                else
                {
                    TempData["List"] = DataList;
                }
                
            }
            return View();
        }

        public ActionResult About()
        {

            return View();
        }
        //public ActionResult Contact()
        //{
        //    return View();
        //}
        public ActionResult Helping_Material()
        {
            List<EasySystem.Models.UserHelpingMaterial> DataList = new List<EasySystem.Models.UserHelpingMaterial>();
            DataList = GetHelpingMaterial();
            if (DataList.Count > 0)
            {
                TempData["List"] = DataList;
            }
            return View();
        }

        public ActionResult DownloadDocument(string Docfile)
        {
            string file = Path.Combine(HostingEnvironment.WebRootPath, "images/UserMarketingMaterial/company/" + Docfile);
            //string contentType = "application/rar";
            var memory = new MemoryStream();
            using (var stream = new FileStream(file, FileMode.Open))
            {
                stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", Path.GetFileName(file));
        }


        public ActionResult Trainings()
        {
            List<TrainingInfo> DataList = new List<TrainingInfo>();
            DataList = GetTraining();
            if (DataList.Count > 0)
            {
                TempData["List"] = DataList;
            }
            return View();
        }

        public ActionResult TrainingDetail(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("Public/GetTrainingDetail?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<TrainingInfo>(res);
                    TempData["Detail"] = data;
                    return View();
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Trainings");
                }
            }

            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Trainings");
            }
        }

        public ActionResult PayFee()
        {
            try
            {
                PopulateFeeType();
                JazzCash jazz = new JazzCash();
                jazz.pp_Amount = "1000";
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
                jazz.pp_BillReference = "00000";
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



                //var hash = "" + jazz.salt + "&" + jazz.pp_Amount + " &" + jazz.pp_BillReference + "&"  + jazz.pp_DiscountBank + "&" + jazz.pp_Language + "&" + jazz.pp_MerchantID + "&" + jazz.pp_Password + "&" + jazz.pp_ReturnURL + "&" + jazz.pp_TxnCurrency + "&" +
                //    "" + jazz.pp_TxnDateTime + "&" + jazz.pp_TxnExpiryDateTime + "&" + jazz.pp_TxnRefNo +  "&" + jazz.pp_Version + "&" + jazz.ppmpf_1 + "&" + jazz.ppmpf_2 + "&" + jazz.ppmpf_3 + "&" + jazz.ppmpf_4 + "&" + jazz.ppmpf_5 + "";

                //var hash = "" + jazz.salt + "&" + jazz.pp_Amount + "&" + jazz.pp_BillReference + "&" + jazz.pp_Description + "&" + jazz.pp_DiscountedAmount + "&" + jazz.pp_DiscountBank + "&" + jazz.pp_Language + "&" + jazz.pp_MerchantID + "&" + jazz.pp_Password + "&" + jazz.pp_ReturnURL + "&" + jazz.pp_TxnCurrency + "&" +
                //    "" + jazz.pp_TxnDateTime + "&" + jazz.pp_TxnExpiryDateTime + "&" + jazz.pp_TxnRefNo + "&" + jazz.pp_TxnType + "&" + jazz.pp_Version + "&" + jazz.ppmpf_1 + "&" + jazz.ppmpf_2 + "&" + jazz.ppmpf_3 + "&" + jazz.ppmpf_4 + "&" + jazz.ppmpf_5 + "";

                //var hash = "2d8w96dwtv&300000&TBANK&UniversalSkillsFee&Thank you for using our services&EN&MC0793&5z1yu80v4w&RETL&http://localhost:56790/Home/Thankyou&PKR&20200612180248&20200620180248&T20200612180248&1.1&1&2&3&4&5";

                //var SecureHash = CreateToken(hash, jazz.salt);

                //var sHash = GetHash(hash, jazz.salt);

                //var sHash = getHash(hash, jazz.salt);
                //var sHash = HMACSHA256Encode(hash, jazz.salt);

                //jazz.pp_SecureHash = sHash;
                TempData["JazzCash"] = jazz;
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        [HttpPost]
        public ActionResult PayFee(string json)
        {
            try
            {
                var fee = JsonConvert.DeserializeObject<FeeList>(json); // This produces an object with valid data!
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("Public/PayFee", fee);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record added successfuly";
                    return View("Index");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return View("Index");
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("PayFee");
        }


        //[SessionCheck]
        public IActionResult Thankyou(JazzCash jazz)
        {

            if (jazz != null)
            {
                if (jazz.pp_BillReference == "00000")
                {
                    TempData["JazzResponse"] = jazz;
                }
                else
                {
                    if (jazz.pp_ResponseCode == "000")
                    {
                        try
                        {
                            var SerialNo = com.GetSerialNo();
                            SkillsFee fee = new SkillsFee();
                            fee = com.GetSkillsFee();
                            EasySystemAPI.Models.Users usr = new EasySystemAPI.Models.Users();
                            int UsrId = Convert.ToInt32(jazz.pp_BillReference);
                            usr = com.GetUser(UsrId);
                            EasySystemAPI.Models.Transections trn = new EasySystemAPI.Models.Transections();
                            int? usrCode = usr.usrCode;
                            string usrName = usr.usrName;
                            trn.tDate = DateTime.Now.Date;
                            trn.tDateTime = DateTime.Now;
                            trn.tStatus = "Pending";
                            trn.tNarration = "" + usrName + "(" + usrCode + ")";
                            trn.tPaying = usr.usrId;
                            trn.tReceiving = usr.refId;
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
                                double totalAmount = trn.ThirdPartyCharges + trn.SoftServiceCharges + trn.TuitionAmount;
                                EasySystemAPI.Models.Users Mentor = new EasySystemAPI.Models.Users();
                                usr = com.GetUser(UsrId);
                                Mentor = com.GetMentor(usr.refId);
                                TempData["Transaction"] = "Transaction completed successfully." + usr.usrName + " you have paid " + totalAmount + " /- pkr' on dated " + DateTime.Now + ". " + trn.TuitionAmount + " /- pkr has been reflected in your Mentor's (" + Mentor.usrName + "-" + Mentor.usrCode + ") account, " + trn.ThirdPartyCharges + " /- pkr is money transferred fee and " + trn.SoftServiceCharges + " /- pkr is system fee and the transaction # is " + trn.tNumber + ".";
                                return RedirectToAction("PayMyFee", "MyTransaction");
                            }
                            else
                            {
                                var res = result.Content.ReadAsStringAsync().Result;
                                var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                                TempData["Error"] = "" + errorMsg.message + "";
                                return RedirectToAction("PayMyFee", "MyTransaction");
                            }
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = "An error occured while adding the Transection Data.Please try again later";
                            return RedirectToAction("PayMyFee", "MyTransaction");
                        }
                    }
                    else
                    {
                        //TempData["JazzResponse"] = jazz;
                        TempData["Error"] = "" + jazz.pp_ResponseMessage + "";
                        return RedirectToAction("PayMyFee", "MyTransaction");
                    }
                }
            }
            return View();
        }



        public List<TrainingInfo> GetTraining()
        {
            List<TrainingInfo> DataList = new List<TrainingInfo>();
            HttpClient client = _api.Initial();
            var Data = client.GetAsync("Public/GetTrainingInfo");
            Data.Wait();
            var result = Data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                DataList = JsonConvert.DeserializeObject<List<TrainingInfo>>(res);
            }
            return DataList;
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

        public void PopulateFeeType()
        {
            List<TrainingInfo> infos = new List<TrainingInfo>();

            List<TrainingInfo> DataList = new List<TrainingInfo>();
            DataList = GetTraining();
            if (DataList.Count != 0)
            {
                foreach (var i in DataList)
                {
                    TrainingInfo training = new TrainingInfo();
                    training.tiId = i.tiId;
                    training.tiName = i.tiName + " (Rs. " + i.tiPrice + "/" + i.tiPriceDuration + ")";
                    infos.Add(training);
                }
                SelectList sl = new SelectList(infos, "tiId", "tiName");
                ViewData["TrainingInfo"] = sl;
            }
        }



        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Privacy_Policy()
        {
            return View();
        }
        public IActionResult TermsAndConditions()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult EarningDisclaimer()
        {
            return View();
        }
        public IActionResult FAQs()
        {
            return View();
        }
        public IActionResult Facts()
        {
            return View();
        }
        public IActionResult HowThingsWork()
        {
            return View();
        }
        //PublicLayout
        public ActionResult RefundPolicy()
        {
            return View();
        }
        public ActionResult Terms()
        {
            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            return View();
        }



        // !-------------Admin Panel -----------!
        public ActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(Models.Users users)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.PostAsJsonAsync("Public/AdminLogin", users);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<Models.Users>(res);
                    if (data.roleId == 1)
                    {
                        HttpContext.Session.SetInt32("AdminID", data.usrId);
                        HttpContext.Session.SetInt32("Role", data.roleId);
                        return RedirectToAction("AdminDashboard");
                    }
                    else
                    {
                        TempData["Error"] = "You do not belongs from here.";
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

            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return View();
            }
        }

        [SessionCheckPublic]
        public ActionResult AdminDashboard()
        {
            var gettCount = com.GetTrainingTypeCount();
            TempData["Type"] = gettCount;

            var getICount = com.GetTrainingInfoCount();
            TempData["Info"] = getICount;

            List<FeeListVM> getFee = new List<FeeListVM>();
            getFee = GetFeeList();
            if (getFee.Count != 0)
            {
                var Ongoing = getFee.Where(o => o.flStatus == "Started").Count();
                TempData["Ongoing"] = Ongoing;

                var Completed = getFee.Where(o => o.flStatus == "Ended").Count();
                TempData["Completed"] = Completed;

                var New = getFee.Where(o => o.flStatus == "Pending").Sum(p => p.tiPrice);
                TempData["New"] = New;

                var Accepted = getFee.Where(o => o.flStatus == "Accepted").Sum(p => p.tiPrice);
                TempData["Accepted"] = Accepted;

                var Refunded = getFee.Where(o => o.flStatus == "Refunded").Sum(p => p.tiPrice);
                TempData["Refunded"] = Refunded;

                var Ended = getFee.Where(o => o.flStatus == "Ended").Sum(p => p.tiPrice);
                TempData["Ended"] = Ended;
            }


            return View();
        }
        [SessionCheckPublic]
        public ActionResult AdminFeeList()
        {
            return View();
        }
        [SessionCheckPublic]
        public ActionResult AdminTraining()
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("Public/AdminGetTrainingInfo");
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<List<TrainingInfoVM>>(res);
                    TempData["Detail"] = data;
                    return View();
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Trainings");
                }
            }

            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Trainings");
            }
        }

        [SessionCheckPublic]
        public ActionResult AdminAddTraining()
        {
            PopulateTrainingType();
            return View();
        }

        [SessionCheckPublic]
        [HttpPost]
        public ActionResult AdminAddTraining(TrainingInfo data)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.PostAsJsonAsync("Public/AdminAddTrainingInfo", data);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("AdminTraining");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return View();
                }
            }

            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("AdminTraining");
            }
        }

        [SessionCheckPublic]
        public ActionResult AdminEditTraining(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("Public/GetTrainingDetail?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    PopulateTrainingType();
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<TrainingInfo>(res);
                    return View(data);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("AdminTraining");
                }
            }

            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("AdminTraining");
            }


        }

        [SessionCheckPublic]
        [HttpPost]
        public ActionResult AdminEditTraining(int id, TrainingInfo trainingInfo)
        {
            try
            {
                trainingInfo.tiId = id;
                HttpClient client = _api.Initial();
                var UpdateData = client.PostAsJsonAsync("Public/AdminUpdateTrainingInfo", trainingInfo);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("AdminTraining");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("AdminTraining");
                }
            }

            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("AdminTraining");
            }


        }

        [SessionCheckPublic]
        public ActionResult DeleteTraining(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("Public/AdminDeleteTrainingInfo?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data deleted successfully";
                    return RedirectToAction("AdminTraining");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("AdminTraining");
                }
            }

            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("AdminTraining");
            }


        }

        public void PopulateTrainingType()
        {
            List<TrainingType> DataList = new List<TrainingType>();
            DataList = GetTrainingType();
            SelectList sl = new SelectList(DataList, "ttId", "ttName");
            ViewData["TrainingType"] = sl;
        }

        public List<TrainingType> GetTrainingType()
        {
            List<TrainingType> DataList = new List<TrainingType>();
            HttpClient client = _api.Initial();
            var Data = client.GetAsync("Public/GetTrainingType");
            Data.Wait();
            var result = Data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                DataList = JsonConvert.DeserializeObject<List<TrainingType>>(res);
            }
            return DataList;
        }

        public List<FeeListVM> GetFeeList()
        {
            List<FeeListVM> DataList = new List<FeeListVM>();
            HttpClient client = _api.Initial();
            var Data = client.GetAsync("Public/AdminDashboardFeeList");
            Data.Wait();
            var result = Data.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                DataList = JsonConvert.DeserializeObject<List<FeeListVM>>(res);
            }
            return DataList;
        }



        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("AdminLogin");
        }

    }
}
