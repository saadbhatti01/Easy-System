using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySystem.EasyAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using EasySystem.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;


namespace EasySystem.Controllers
{
    public class BlogsController : Controller
    {
        EasySysAPI _api = new EasySysAPI();
        private readonly IHostingEnvironment HostingEnvironment;
        public BlogsController(IHostingEnvironment hostingEnvironment)
        {
            this.HostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            //            try
            //            {
            //                var Id = HttpContext.Session.GetInt32("ID");
            //                HttpClient client = _api.Initial();
            //                Task<HttpResponseMessage> Data;
            //                if (Id != null)
            //                {
            //                    Data = client.GetAsync("My/GetBlogData?Id=" + Id.ToString());
            //                }
            //                else
            //                {
            //                    Data = client.GetAsync("My/GetBlogs");
            //                    TempData["BlogWithOutLogin"] = "BlogWithOutLogin";
            //                }

            //                Data.Wait();
            //                var result = Data.Result;
            //                if (result.IsSuccessStatusCode)
            //                {
            //                    var res = result.Content.ReadAsStringAsync().Result;
            //                    var List = JsonConvert.DeserializeObject<List<Blog>>(res);
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

        public IActionResult _BlogData(int Value)
        {
            try
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                Task<HttpResponseMessage> Data;
                if (Id != null)
                {
                    EasySystemAPI.Models.PageModel page = new EasySystemAPI.Models.PageModel();
                    page.id = Convert.ToInt32(Id);
                    page.Count = Value;
                    //Data = client.GetAsync("My/GetBlogData?Id=" + Id.ToString());
                    Data = client.PostAsJsonAsync("My/GetMoreBlogData", page);
                }
                else
                {
                    Data = client.GetAsync("My/GetMoreBlogs?Value=" + Value.ToString());
                    TempData["BlogWithOutLogin"] = "BlogWithOutLogin";
                }
                Data.Wait();
                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var List = JsonConvert.DeserializeObject<List<Blog>>(res);
                    if (List.Count == 0)
                    {
                        return PartialView("_BlogData", "");
                    }
                    TempData["List"] = List;
                }
                else
                {
                    return PartialView("_BlogData", "");
                    //var res = result.Content.ReadAsStringAsync().Result;
                    //var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    //TempData["Error"] = "" + errorMsg.message + "";
                }
            }

            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
            }
            return PartialView("_BlogData");
        }

        [HttpPost]
        public IActionResult AddBlog(Blog data, IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                    string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    file.CopyTo(new FileStream(filePath, FileMode.Create));
                    data.blogTitleImage = fileName;
                }
                HttpClient client = _api.Initial();
                var postVerify = client.PostAsJsonAsync("My/AddBlog", data);
                postVerify.Wait();
                var result = postVerify.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record added successfuly";
                    return RedirectToAction("Index");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Index");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Index");
            }
        }

        public IActionResult DeleteBlog(int id)
        {
            try
            {
                HttpClient clientt = _api.Initial();
                var DelbankInfo = clientt.DeleteAsync("My/DelBlog?id=" + id.ToString());
                DelbankInfo.Wait();
                var result = DelbankInfo.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Record Deleted successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Index");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Index");
            }

        }

        public IActionResult BlogDetail(int id, string title)
        {
            try
            {
                var Id = HttpContext.Session.GetInt32("ID");
                HttpClient client = _api.Initial();
                //var UpdateData = client.GetAsync("My/GetBlog?id=" + id.ToString());
                var UpdateData = client.GetAsync("My/GetBlog?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<Blog>(res);

                    string realTitle = EasySysAPI.URLFriendly(data.blogTitle);
                    string urlTitle = (title ?? "").Trim().ToLower();

                    if (realTitle != urlTitle)
                    {
                        string url = "/Blogs/BlogDetail/" + data.blogId + "/" + realTitle;
                        return new LocalRedirectResult(url);
                    }


                    TempData["BlogDetail"] = data;

                    //GetUserBlog
                    if (Id == null)
                    {
                        var getBlogs = GetUserBlog(data.UsrId);
                        if (getBlogs.Count > 0)
                        {
                            TempData["OtherBlogs"] = getBlogs;
                            foreach (var i in getBlogs)
                            {
                                TempData["UserName"] = i.usrName;
                            }
                        }
                        TempData["BlogWithOutLogin"] = "BlogWithOutLogin";
                    }
                    return View(data);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Index");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Index");
            }
        }

        public IActionResult EditBlog(int id)
        {
            try
            {
                HttpClient client = _api.Initial();
                var UpdateData = client.GetAsync("My/GetBlog?id=" + id.ToString());
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<Blog>(res);
                    return View(data);
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Index");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult EditBlog(int id, Blog data, IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                    string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    file.CopyTo(new FileStream(filePath, FileMode.Create));
                    data.blogTitleImage = fileName;
                }
                HttpClient client = _api.Initial();
                var UpdateData = client.PutAsJsonAsync("My/UpdateBlog?id=" + id.ToString(), data);
                UpdateData.Wait();
                var result = UpdateData.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Data Updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    TempData["Error"] = "" + errorMsg.message + "";
                    return RedirectToAction("Index");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
                return RedirectToAction("Index");
            }

        }


        public IActionResult _ProfileBlogData(int Value, int usId)
        {
            try
            {
                HttpClient client = _api.Initial();
                Task<HttpResponseMessage> Data;
                EasySystemAPI.Models.PageModel page = new EasySystemAPI.Models.PageModel();
                page.id = Convert.ToInt32(usId);
                page.Count = Value;
                //Data = client.GetAsync("My/GetBlogData?Id=" + Id.ToString());
                Data = client.PostAsJsonAsync("My/GetMoreBlogData", page);
                Data.Wait();
                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var List = JsonConvert.DeserializeObject<List<Blog>>(res);
                    if (List.Count == 0)
                    {
                        return PartialView("_BlogData", "");
                    }
                    TempData["List"] = List;
                }

                else
                {
                    return PartialView("_BlogData", "");
                    //var res = result.Content.ReadAsStringAsync().Result;
                    //var errorMsg = JsonConvert.DeserializeObject<ErrorMessage>(res);
                    //TempData["Error"] = "" + errorMsg.message + "";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occured during getting the request. Please try again later";
            }
            return PartialView("_BlogData");
        }

        public List<Blog> GetUserBlog(int Id)
        {
            List<Blog> Data = new List<Blog>();
            HttpClient client = _api.Initial();
            var GetBank = client.GetAsync("My/GetUsersBlogs?Id=" + Id.ToString());
            GetBank.Wait();
            var result = GetBank.Result;
            if (result.IsSuccessStatusCode)
            {
                var res = result.Content.ReadAsStringAsync().Result;
                Data = JsonConvert.DeserializeObject<List<Blog>>(res);
            }

            return Data;
        }
    }
}