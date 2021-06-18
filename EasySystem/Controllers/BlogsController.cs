using EasySystem.EasyAPI;
using EasySystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


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

        public IActionResult Index(string Public)
        {
            if (Public != null)
            {
                TempData["Public"] = "true";
            }
            else
            {
                HttpContext.Session.Remove("Public");
            }
            return View();
        }

        public IActionResult _BlogData(int Value, string Public)
        {
            try
            {
                if (Public != null)
                {
                    HttpContext.Session.SetString("Public", "Public");
                    HttpClient clientt = _api.Initial();
                    Task<HttpResponseMessage> Dataa;
                    Dataa = clientt.GetAsync("My/GetMoreBlogs?Value=" + Value.ToString());
                    TempData["BlogWithOutLogin"] = "BlogWithOutLogin";
                    Dataa.Wait();
                    var resultt = Dataa.Result;
                    if (resultt.IsSuccessStatusCode)
                    {
                        var res = resultt.Content.ReadAsStringAsync().Result;
                        var List = JsonConvert.DeserializeObject<List<Blog>>(res);
                        if (List.Count == 0)
                        {
                            return PartialView("_BlogData", "");
                        }
                        TempData["List"] = List;
                    }

                    return PartialView("_BlogData");
                }
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
        public IActionResult AddBlog(Blog data, IFormFile Image, IFormFile cImage)
        {
            try
            {
                var allowedExtensions = new[] { ".Jpg", ".JPG", ".jpg", ".jpeg", ".JPEG", ".png", ".PNG" };

                if (Image != null)
                {
                    var ext = Path.GetExtension(Image.FileName);
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/Blogs/Thumbnail");
                        string fileName = Image.FileName;
                        string filePath = Path.Combine(uploadFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            Image.CopyTo(fileStream);
                        }
                        //Image.CopyTo(new FileStream(filePath, FileMode.Create));
                        data.blogTitleImage = fileName;
                    }
                    else
                    {
                        TempData["Error"] = "Please Upload a valid image file. Valid formates are ('.Jpg','.JPG', '.jpg', .'jpeg', '.JPEG', '.png', '.PNG')";
                        return RedirectToAction("Index");
                    }

                }
                if (cImage != null)
                {
                    var ext = Path.GetExtension(cImage.FileName);
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/Blogs/CoverPage");
                        string fileName = cImage.FileName;
                        string filePath = Path.Combine(uploadFolder, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            cImage.CopyTo(fileStream);
                        }
                        //cImage.CopyTo(new FileStream(filePath, FileMode.Create));
                        data.blogCoverPageImage = fileName;
                    }
                    else
                    {
                        TempData["Error"] = "Please Upload a valid image file. Valid formates are ('.Jpg','.JPG', '.jpg', .'jpeg', '.JPEG', '.png', '.PNG')";
                        return RedirectToAction("Index");
                    }

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
        public IActionResult EditBlog(int id, Blog data, IFormFile Image, IFormFile cImage)
        {
            try
            {
                var allowedExtensions = new[] { ".Jpg", ".JPG", ".jpg", ".jpeg", ".JPEG", ".png", ".PNG" };

                if (Image != null)
                {
                    var ext = Path.GetExtension(Image.FileName);
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/Blogs/Thumbnail");
                        string fileName = Image.FileName;
                        string filePath = Path.Combine(uploadFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            Image.CopyTo(fileStream);
                        }
                        //Image.CopyTo(new FileStream(filePath, FileMode.Create));
                        data.blogTitleImage = fileName;
                    }
                    else
                    {
                        TempData["Error"] = "Please Upload a valid image file. Valid formates are ('.Jpg','.JPG', '.jpg', .'jpeg', '.JPEG', '.png', '.PNG')";
                        return RedirectToAction("EditBlog");
                    }

                }
                if (cImage != null)
                {
                    var ext = Path.GetExtension(cImage.FileName);
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images/Blogs/CoverPage");
                        string fileName = cImage.FileName;
                        string filePath = Path.Combine(uploadFolder, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            cImage.CopyTo(fileStream);
                        }
                        //cImage.CopyTo(new FileStream(filePath, FileMode.Create));
                        data.blogCoverPageImage = fileName;
                    }
                    else
                    {
                        TempData["Error"] = "Please Upload a valid image file. Valid formates are ('.Jpg','.JPG', '.jpg', .'jpeg', '.JPEG', '.png', '.PNG')";
                        return RedirectToAction("EditBlog");
                    }

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