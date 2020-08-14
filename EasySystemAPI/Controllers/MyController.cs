using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySystemAPI.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasySystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyController : ControllerBase
    {
        private readonly EasyContext con;
        private readonly IDataProtector protector;

        public MyController(EasyContext context, IDataProtectionProvider dataProtectionProvider
            , DataProtection dataProtection)
        {
            con = context;
            protector = dataProtectionProvider.CreateProtector(dataProtection.UserId);
        }

        [HttpGet("GetMyProfile")]
        public async Task<ActionResult<Users>> GetMyProfile(string id)
        {
            try
            {
                //string dId = PasswordEncryption.Decrypt(id);
                //int usrId = Convert.ToInt32(protector.Unprotect(id));
                int usrId = Convert.ToInt32(id);
                var users = await con.users.FindAsync(usrId);

                if (users == null)
                {
                    return BadRequest(new { message = "No Profile record found" });
                }

                return users;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occured while getting the user profile data. Please try again later." });
            }

        }

        [HttpGet("GetDashboardData")]
        public async Task<ActionResult<Dashboard>> GetDashboard(int id)
        {
            try
            {
                Dashboard dash = new Dashboard();
                var getUserData = await con.users.Where(u => u.refId == id).ToListAsync();
                if (getUserData.Count > 0)
                {
                    var Active = getUserData.Where(u => u.usrStatus == "Active").Count();
                    var Learner = getUserData.Where(u => u.usrStatus == "Learning").Count();
                    dash.Active = Active;
                    dash.Learner = Learner;
                }
                var getLost = await con.userLostTrainees.Where(b => b.usrIdM == id).CountAsync();
                dash.Lost = getLost;

                var getBalance = await con.transections.Where(t => t.tReceiving == id).ToListAsync();
                if (getBalance.Count > 0)
                {
                    dash.Current = getBalance.Where(b => b.tStatus == "Verified").Sum(t => t.TuitionAmount);
                    dash.Withdraw = getBalance.Where(b => b.tStatus == "Completed").Sum(t => t.TuitionAmount);
                    dash.Total = dash.Current + dash.Withdraw;
                }

                return Ok(dash);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the dashbaord data. Please try again later." });
            }

        }


        [HttpGet("GetBlogs")]
        public async Task<ActionResult> GetBlogs()
        {
            try
            {
                var Count = con.blogs.Where(b => b.blogStatus == "Active").Count();
                if (Count > 9)
                {
                    var getData = await (from b in con.blogs
                                         join c in con.users on b.UsrId equals c.usrId
                                         where b.blogStatus == "Active"
                                         select new
                                         {
                                             b.blogId,
                                             b.blogTitle,
                                             b.blogTitleImage,
                                             b.createdDate,
                                             b.UsrId,
                                             b.blogCoverPageImage,
                                             b.blogDescription,
                                             b.blogStatus,
                                             b.blogType,
                                             c.usrName,

                                         }).Take(9).OrderByDescending(o => o.blogId).ToListAsync();
                    return Ok(getData);
                }
                else
                {
                    var getData = await (from b in con.blogs
                                         join c in con.users on b.UsrId equals c.usrId
                                         where b.blogStatus == "Active"
                                         select new
                                         {
                                             b.blogId,
                                             b.blogTitle,
                                             b.blogTitleImage,
                                             b.createdDate,
                                             b.UsrId,
                                             b.blogCoverPageImage,
                                             b.blogDescription,
                                             b.blogStatus,
                                             b.blogType,
                                             c.usrName,

                                         }).OrderByDescending(o => o.blogId).ToListAsync();
                    return Ok(getData);
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting Info. Please try again later." });
            }
        }

        [HttpGet("GetMoreBlogs")]
        public async Task<ActionResult> GetMoreBlogs(int Value)
        {
            try
            {
                var Count = con.blogs.Where(b => b.blogStatus == "Active").Count();

                if (Count >= Value)
                {
                    var getData = await (from b in con.blogs
                                         join c in con.users on b.UsrId equals c.usrId
                                         where b.blogStatus == "Active"
                                         select new
                                         {
                                             b.blogId,
                                             b.blogTitle,
                                             b.blogTitleImage,
                                             b.createdDate,
                                             b.UsrId,
                                             b.blogCoverPageImage,
                                             b.blogDescription,
                                             b.blogStatus,
                                             b.blogType,
                                             c.usrName,

                                         }).Skip(Value).Take(9).OrderByDescending(o => o.blogId).ToListAsync();
                    return Ok(getData);
                }
                else
                {
                    var getData = await (from b in con.blogs
                                         join c in con.users on b.UsrId equals c.usrId
                                         where b.blogStatus == "Active"
                                         select new
                                         {
                                             b.blogId,
                                             b.blogTitle,
                                             b.blogTitleImage,
                                             b.createdDate,
                                             b.UsrId,
                                             b.blogCoverPageImage,
                                             b.blogDescription,
                                             b.blogStatus,
                                             b.blogType,
                                             c.usrName,

                                         }).Skip(Value).OrderByDescending(o => o.blogId).ToListAsync();
                    if (getData.Count > 0)
                    {
                        return Ok(getData);
                    }
                    else
                    {
                        return BadRequest(new { message = "No record found." });
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting Info. Please try again later." });
            }
        }

        [HttpPost("GetMoreBlogData")]
        public async Task<ActionResult> GetMoreBlogData(PageModel Data)
        {
            try
            {
                var Count = con.blogs.Where(b => b.blogStatus == "Active").Count();
                if (Count >= Data.Count)
                {
                    var getData = await (from b in con.blogs
                                         join c in con.users on b.UsrId equals c.usrId
                                         where b.UsrId == Data.id
                                         select new
                                         {
                                             b.blogId,
                                             b.blogTitle,
                                             b.blogTitleImage,
                                             b.createdDate,
                                             b.UsrId,
                                             b.blogCoverPageImage,
                                             b.blogDescription,
                                             b.blogStatus,
                                             b.blogType,
                                             c.usrId,
                                             c.usrName,

                                         }).Skip(Data.Count).Take(9).OrderByDescending(o => o.blogId).ToListAsync();
                    return Ok(getData);
                }
                else
                {
                    var getData = await (from b in con.blogs
                                         join c in con.users on b.UsrId equals c.usrId
                                         where b.UsrId == Data.id
                                         select new
                                         {
                                             b.blogId,
                                             b.blogTitle,
                                             b.blogTitleImage,
                                             b.createdDate,
                                             b.UsrId,
                                             b.blogCoverPageImage,
                                             b.blogDescription,
                                             b.blogStatus,
                                             b.blogType,
                                             c.usrId,
                                             c.usrName,

                                         }).Skip(Data.Count).OrderByDescending(o => o.blogId).ToListAsync();
                    if (getData.Count > 0)
                    {
                        return Ok(getData);
                    }
                    else
                    {
                        return BadRequest(new { message = "No record found." });
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting Info. Please try again later." });
            }
        }

        [HttpGet("GetBlogData")]
        public async Task<ActionResult> GetBlogData(int id)
        {
            try
            {
                var getData = await (from b in con.blogs
                                     join c in con.users on b.UsrId equals c.usrId
                                     where b.UsrId == id
                                     select new
                                     {
                                         b.blogId,
                                         b.blogTitle,
                                         b.blogTitleImage,
                                         b.createdDate,
                                         b.UsrId,
                                         b.blogCoverPageImage,
                                         b.blogDescription,
                                         b.blogStatus,
                                         b.blogType,
                                         c.usrId,
                                         c.usrName,

                                     }).OrderByDescending(o => o.blogId).ToListAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting Info. Please try again later." });
            }
        }


        [HttpGet("GetUsersBlogs")]
        public async Task<ActionResult> GetUsersBlogs(int id)
        {
            try
            {
                var getData = await (from b in con.blogs
                                     join c in con.users on b.UsrId equals c.usrId
                                     where b.UsrId == id && b.blogStatus == "Active"
                                     select new
                                     {
                                         b.blogId,
                                         b.blogTitle,
                                         b.blogTitleImage,
                                         b.createdDate,
                                         b.UsrId,
                                         b.blogCoverPageImage,
                                         b.blogDescription,
                                         b.blogStatus,
                                         b.blogType,
                                         c.usrId,
                                         c.usrName,
                                     }).OrderByDescending(o => o.blogId).ToListAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting Info. Please try again later." });
            }
        }


        [HttpPost("AddBlog")]
        public async Task<ActionResult> AddBlog(Blog blog)
        {
            try
            {
                blog.blogType = 1;
                blog.createdDate = DateTime.Now;
                con.blogs.Add(blog);
                await con.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "an error occured. Data not added" });
            }
        }

        [HttpDelete("DelBlog")]
        public async Task<ActionResult> DelBlog(int id)
        {

            var getData = await con.blogs.FindAsync(id);
            if (getData == null)
            {
                return NotFound();
            }

            con.blogs.Remove(getData);
            await con.SaveChangesAsync();
            return Ok();



        }

        [HttpGet("GetBlog")]
        public async Task<ActionResult> GetBlog(int id)
        {
            try
            {
                //var GetData = await con.blogs.FindAsync(id);
                var getData = await (from b in con.blogs
                                     join c in con.users on b.UsrId equals c.usrId
                                     where b.blogId == id
                                     select new
                                     {
                                         b.blogId,
                                         b.blogTitle,
                                         b.blogTitleImage,
                                         b.createdDate,
                                         b.UsrId,
                                         b.blogCoverPageImage,
                                         b.blogDescription,
                                         b.blogStatus,
                                         b.blogType,
                                         c.usrId,
                                         c.usrName,

                                     }).FirstOrDefaultAsync();
                if (getData == null)
                {
                    return NotFound();
                }

                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }

        [HttpGet("GetBlogWithTitle")]
        public async Task<ActionResult<Blog>> GetBlogWithTitle(string title)
        {
            try
            {
                var GetData = await con.blogs.Where(b => b.blogTitle == title).FirstOrDefaultAsync();

                if (GetData == null)
                {
                    return NotFound();
                }

                return GetData;

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }

        [HttpPut("UpdateBlog")]
        public async Task<IActionResult> UpdateBlog(int id, Blog blog)
        {
            var getData = await con.blogs.FindAsync(id);
            if (getData != null)
            {
                getData.blogDescription = blog.blogDescription;
                getData.blogStatus = blog.blogStatus;
                getData.blogTitle = blog.blogTitle;
                if (blog.blogTitleImage != null)
                {
                    getData.blogTitleImage = blog.blogTitleImage;
                }


                con.Entry(getData).State = EntityState.Modified;
            }
            try
            {
                await con.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "An error occured while adding the user skill Info. Please try again later" });
            }

            return NoContent();
        }


        [HttpGet("AdminGetBlog")]
        public async Task<ActionResult<Blog>> AdminGetBlog(int id)
        {
            try
            {
                var GetData = await con.blogs.FindAsync(id);

                if (GetData == null)
                {
                    return NotFound();
                }

                return GetData;

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the data. Please try again later." });
            }
        }




        [HttpGet("GetBankData")]
        public async Task<ActionResult> GetBankData()
        {
            try
            {
                var getData = await (from b in con.banks
                                     join c in con.countries on b.CountryId equals c.CountryId
                                     select new
                                     {
                                         b.BankId,
                                         b.BankName,
                                         b.Status,
                                         c.CountryId,
                                         c.CountryName
                                     }).OrderByDescending(o => o.BankId).ToListAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }

        [HttpPost("AddBank")]
        public async Task<ActionResult> AddBank(Bank bank)
        {
            try
            {
                var chkData = await con.banks.Where(b => b.BankName == bank.BankName && b.CountryId == bank.CountryId).AnyAsync();
                if (chkData == true)
                {
                    return BadRequest(new { message = "This Bank with selected country is already exist" });
                }
                else
                {
                    con.banks.Add(bank);
                    await con.SaveChangesAsync();
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "an error occured. Data not added" });
            }
        }

        [HttpDelete("DelBank")]
        public async Task<ActionResult> DelBank(int id)
        {
            var ChkData = await con.userBankInfos.Where(b => b.BankId == id).AnyAsync();
            if (ChkData == true)
            {
                return BadRequest(new { message = "This bank connt be deleted because this bank is associated with User BankInfo" });
            }
            else
            {
                var getData = await con.banks.FindAsync(id);
                if (getData == null)
                {
                    return NotFound();
                }

                con.banks.Remove(getData);
                await con.SaveChangesAsync();
                return Ok();
            }


        }

        [HttpGet("AdminGetBank")]
        public async Task<ActionResult<Bank>> AdminGetBank(int id)
        {
            try
            {
                var GetData = await con.banks.FindAsync(id);

                if (GetData == null)
                {
                    return NotFound();
                }

                return GetData;

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }

        [HttpPut("UpdateBank")]
        public async Task<IActionResult> UpdateBank(int id, Bank bank)
        {
            var getData = await con.banks.FindAsync(id);
            if (getData != null)
            {
                getData.CountryId = bank.CountryId;
                getData.BankName = bank.BankName;
                getData.Status = bank.Status;
                con.Entry(getData).State = EntityState.Modified;
            }
            try
            {
                await con.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "An error occured while adding the user skill Info. Please try again later" });
            }

            return NoContent();
        }


        [HttpGet("GetCountryData")]
        public async Task<ActionResult> GetCountryData()
        {
            try
            {
                var getData = await con.countries.ToListAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the data. Please try again later." });
            }
        }

        [HttpPost("AddCountry")]
        public async Task<ActionResult> AddCountry(Country country)
        {
            try
            {
                var chkData = await con.countries.Where(b => b.CountryName == country.CountryName || b.CountryCode == country.CountryCode).AnyAsync();
                if (chkData == true)
                {
                    return BadRequest(new { message = "The entered Data is already exist" });
                }
                else
                {
                    con.countries.Add(country);
                    await con.SaveChangesAsync();
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "an error occured. Data not added" });
            }
        }

        [HttpDelete("DelCountry")]
        public async Task<ActionResult> DelCountry(int id)
        {
            var ChkData = await con.users.Where(b => b.CountryId == id).AnyAsync();
            if (ChkData == true)
            {
                return BadRequest(new { message = "This country cannot be deleted because it is associated with User Info" });
            }
            else
            {
                var getData = await con.countries.FindAsync(id);
                if (getData == null)
                {
                    return NotFound();
                }

                con.countries.Remove(getData);
                await con.SaveChangesAsync();
                return Ok();
            }


        }

        [HttpGet("AdminGetCountry")]
        public async Task<ActionResult<Country>> AdminGetCountry(int id)
        {
            try
            {
                var GetData = await con.countries.FindAsync(id);

                if (GetData == null)
                {
                    return NotFound();
                }

                return GetData;

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }

        [HttpPut("UpdateCountry")]
        public async Task<IActionResult> UpdateCountry(int id, Country country)
        {
            var getData = await con.countries.FindAsync(id);
            if (getData != null)
            {
                getData.CountryName = country.CountryName;
                getData.CountryCode = country.CountryCode;
                getData.NumStart = country.NumStart;
                getData.NumLength = country.NumLength;
                getData.Status = country.Status;
                con.Entry(getData).State = EntityState.Modified;
            }
            try
            {
                await con.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "An error occured while updating the Info. Please try again later" });
            }

            return NoContent();
        }


        [HttpGet("GetUserBankInfo")]
        public async Task<ActionResult> GetUserBankInfo(int id)
        {
            try
            {
                //var getData = await con.userBankInfos.Where(u => u.usrId == id).ToListAsync();
                var getData = await (from u in con.userBankInfos
                                     join b in con.banks on u.BankId equals b.BankId
                                     where u.usrId == id
                                     select new
                                     {
                                         u.ubId,
                                         u.ubNumber,
                                         u.ubTitle,
                                         u.ubDetail,
                                         u.Status,
                                         b.BankName
                                     }).OrderByDescending(o => o.ubId).ToListAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }

        [HttpPost("AddUserBankInfo")]
        public async Task<ActionResult<UserBankInfo>> PostUserBankInfo(UserBankInfo usr)
        {
            try
            {
                var Count = con.userBankInfos.Where(a => a.usrId == usr.usrId).Count();
                if (Count >= 5)
                {
                    return BadRequest(new { message = "You Can add upto 5 Bank Accounts." });
                }
                else
                {
                    var checkData = await con.userBankInfos.Where(b => b.BankId == usr.BankId && b.ubNumber == usr.ubNumber).AnyAsync();
                    if (checkData == false)
                    {
                        con.userBankInfos.Add(usr);
                        try
                        {
                            await con.SaveChangesAsync();
                            return Ok();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return BadRequest(new { message = "An error occured while adding the user bank Info. Please try again later" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Bank with Account # " + usr.ubNumber + " is already exist" });
                    }

                }

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "Data not added." });
            }
        }

        [HttpPost("DelBankInfo")]
        public async Task<ActionResult> DelBankInfo(UserBankInfo data)
        {
            var getData = await con.userBankInfos.Where(u => u.ubId == data.ubId && u.usrId == data.usrId).FirstOrDefaultAsync();
            if (getData == null)
            {
                return BadRequest(new { message = "No record found" });
            }
            if (getData.Status == "Verified" || getData.Status == "Used")
            {
                return BadRequest(new { message = "This Bank Info cannot be deleted because it is " + getData.Status + "." });
            }
            else
            {
                con.userBankInfos.Remove(getData);
                await con.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpGet("GetAllBankInfo")]
        public async Task<ActionResult<IEnumerable<UserBankInfo>>> GetUserBankInfo()
        {
            try
            {
                //var getData = await con.userBankInfos.Where(u => u.usrId == id).ToListAsync();
                var getData = await (from u in con.userBankInfos
                                     join b in con.banks on u.BankId equals b.BankId
                                     join s in con.users on u.usrId equals s.usrId
                                     select new
                                     {
                                         u.ubId,
                                         u.ubNumber,
                                         u.ubTitle,
                                         u.ubDetail,
                                         u.Status,
                                         b.BankName,
                                         s.usrName,
                                         s.usrCode
                                     }).OrderByDescending(o => o.ubId).ToListAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }


        [HttpPost("GetBankInfo")]
        public async Task<ActionResult<UserBankInfo>> GetBankInfo(UserBankInfo data)
        {
            try
            {
                var GetData = await con.userBankInfos.Where(u => u.ubId == data.ubId && u.usrId == data.usrId).FirstOrDefaultAsync();

                if (GetData == null)
                {
                    return BadRequest(new { message = "No record found" });
                }
                if (GetData.Status == "Verified" || GetData.Status == "Used")
                {
                    return BadRequest(new { message = "This Bank Info cannot be edited because it is " + GetData.Status + "." });
                }
                else
                {
                    return GetData;
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }

        [HttpGet("AdminGetBankInfo")]
        public async Task<ActionResult<UserBankInfo>> AdminGetBankInfo(int id)
        {
            try
            {
                var GetData = await con.userBankInfos.FindAsync(id);

                if (GetData == null)
                {
                    return NotFound();
                }

                return GetData;

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }
        [HttpPut("UpdateBankInfo")]
        public async Task<IActionResult> UpdateBankInfo(int id, UserBankInfo bankInfo)
        {
            if (id != bankInfo.ubId)
            {
                return BadRequest();
            }
            var getData = await con.userBankInfos.Where(u => u.ubId == id && u.usrId == bankInfo.usrId).FirstOrDefaultAsync();
            if (getData != null)
            {
                if (getData.Status == "Verified" || getData.Status == "Used")
                {
                    return BadRequest(new { message = "This Bank Info cannot be Updated." });
                }
                else
                {
                    getData.BankId = bankInfo.BankId;
                    getData.ubDetail = bankInfo.ubDetail;
                    getData.ubTitle = bankInfo.ubTitle;
                    getData.ubNumber = bankInfo.ubNumber;
                    getData.Status = bankInfo.Status;
                    con.Entry(getData).State = EntityState.Modified;
                }
            }
            else
            {
                return BadRequest(new { message = "No Record found." });
            }


            try
            {
                await con.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "An error occured while adding the user skill Info. Please try again later" });
            }

            return NoContent();
        }

        [HttpPut("AdminUpdateBankInfo")]
        public async Task<IActionResult> AdminUpdateBankInfo(int id, UserBankInfo bankInfo)
        {
            if (id != bankInfo.ubId)
            {
                return BadRequest();
            }
            var getData = await con.userBankInfos.FindAsync(id);
            if (getData != null)
            {
                getData.Status = bankInfo.Status;
                con.Entry(getData).State = EntityState.Modified;
            }

            try
            {
                await con.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "An error occured while adding the user skill Info. Please try again later" });
            }

            return NoContent();
        }


        [HttpGet("GetUserSkillInfo")]
        public async Task<ActionResult<UserSkills>> GetUserSkillInfo(int id)
        {
            try
            {
                var getData = await (from sk in con.userSkills
                                     join st in con.skillTypes on sk.StId equals st.StId
                                     where sk.usrId == id
                                     select new
                                     {
                                         sk.usId,
                                         sk.StId,
                                         st.StName,
                                         sk.usDetail,
                                         sk.usLevel,
                                         sk.usrId,
                                         sk.Status,
                                         st.StCoverImage,
                                         st.StImage
                                     }).OrderByDescending(o => o.usId).ToListAsync();
                return Ok(getData);
                //var getData = await con.userSkills.Where(u => u.usrId == id).OrderByDescending(o => o.usId).ToListAsync();
                //if (getData.Count > 0)
                //{
                //    List<UserSkills> list = new List<UserSkills>();
                //    foreach (var i in getData)
                //    {
                //        UserSkills skills = new UserSkills();
                //        skills.usName = i.usName;
                //        skills.usLevel = i.usLevel;
                //        skills.Status = i.Status;
                //        skills.usId = i.usId;
                //        skills.usrId = i.usrId;
                //        string detail = i.usDetail;
                //        string twenty = "";
                //        if (!String.IsNullOrWhiteSpace(detail) && detail.Length >= 5)
                //        {
                //            if (!String.IsNullOrWhiteSpace(detail) && detail.Length <= 30)
                //            {
                //                skills.usDetail = i.usDetail;
                //            }
                //            else
                //            {
                //                twenty = detail.Substring(0, 30);
                //                skills.usDetail = twenty;
                //            }
                //        }
                //        else
                //        {
                //            skills.usDetail = i.usDetail;
                //        }
                //        list.Add(skills);

                //    }

                //    return Ok(list);
                //}
                //else
                //{
                //    return Ok(getData);
                //}

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user skill Info data. Please try again later." });
            }
        }

        [HttpGet("GetUserSkillInfoDetail")]
        public async Task<ActionResult<UserSkills>> GetUserSkillInfoDetail(int id)
        {
            try
            {
                var getData = await (from sk in con.userSkills
                                     join st in con.skillTypes on sk.StId equals st.StId
                                     where sk.usId == id
                                     select new
                                     {
                                         sk.usId,
                                         sk.StId,
                                         st.StName,
                                         sk.usDetail,
                                         sk.usLevel,
                                         sk.usrId,
                                         sk.Status,
                                         st.StCoverImage,
                                         st.StImage
                                     }).FirstOrDefaultAsync();
                return Ok(getData);
                //var getData = await con.userSkills.Where(u => u.usId == id).FirstOrDefaultAsync();
                //return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user skill Info data. Please try again later." });
            }
        }


        [HttpPost("AddUserSkillInfo")]
        public async Task<ActionResult<UserSkills>> PostUserSkillInfo(UserSkills usr)
        {
            try
            {
                var Count = con.userSkills.Where(a => a.usrId == usr.usrId).Count();
                if (Count >= 15)
                {
                    return BadRequest(new { message = "You Can add upto 15 Skills." });
                }
                else
                {
                    var CheckData = await con.userSkills.Where(u => u.StId == usr.StId && u.usrId == usr.usrId).AnyAsync();
                    if (CheckData == false)
                    {
                        usr.Status = true;
                        con.userSkills.Add(usr);
                        try
                        {
                            await con.SaveChangesAsync();
                            return Ok();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return BadRequest(new { message = "An error occured while adding the user skill Info. Please try again later" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "This Skill Name is already exist." });
                    }

                }

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "Data not added." });
            }
        }

        [HttpPost("DelSkillInfo")]
        public async Task<ActionResult> DelSkillInfo(UserSkills skillData)
        {
            var chkData = await con.userWhiteBoards.Where(s => s.usId == skillData.usId).AnyAsync();
            if (chkData == false)
            {
                var getData = await con.userSkills.Where(u => u.usId == skillData.usId && u.usrId == skillData.usrId).FirstOrDefaultAsync();
                if (getData == null)
                {
                    return BadRequest(new { message = "No record found" });
                }

                con.userSkills.Remove(getData);
                await con.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return BadRequest(new { message = "This entry cannot be deleted because it is associated with some information" });
            }

        }

        [HttpPost("GetSkill")]
        public async Task<ActionResult<UserSkills>> GetSkill(UserSkills skillData)
        {
            try
            {
                //var getData = await (from sk in con.userSkills
                //                     join st in con.skillTypes on sk.StId equals st.StId
                //                     where sk.usrId == skillData.usrId && sk.usId == skillData.usId
                //                     select new
                //                     {
                //                         sk.usId,
                //                         sk.StId,
                //                         st.StName,
                //                         sk.usDetail,
                //                         sk.usLevel,
                //                         sk.usrId,
                //                         sk.Status,
                //                         st.StCoverImage,
                //                         st.StImage
                //                     }).OrderByDescending(o => o.usId).ToListAsync();
                //return Ok(getData);
                var GetData = await con.userSkills.Where(u => u.usId == skillData.usId && u.usrId == skillData.usrId).FirstOrDefaultAsync();

                if (GetData == null)
                {
                    return BadRequest(new { message = "No record found" });
                }

                return GetData;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the Info. Please try again later." });
            }
        }

        [HttpPut("UpdateSkill")]
        public async Task<IActionResult> UpdateSkill(int id, UserSkills data)
        {
            if (id != data.usId)
            {
                return BadRequest();
            }
            var getData = await con.userSkills.Where(s => s.usId == id && s.usrId == data.usrId).FirstOrDefaultAsync();
            if (getData != null)
            {
                getData.StId = data.StId;
                getData.usLevel = data.usLevel;
                getData.usDetail = data.usDetail;
                getData.Status = true;
                con.Entry(getData).State = EntityState.Modified;
            }
            else
            {
                return BadRequest(new { message = "No record found" });
            }
            try
            {
                await con.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "An error occured while Updating Info. Please try again later" });
            }

            return NoContent();
        }

        [HttpGet("CopyMyMentorSkills")]
        public async Task<ActionResult> CopyMyMentorSkills(int id)
        {
            try
            {
                var getUser = await con.users.Where(s => s.usrId == id).FirstOrDefaultAsync();
                if (getUser != null)
                {
                    var getMentorSkills = await con.userSkills.Where(u => u.usrId == getUser.refId).ToListAsync();
                    if (getMentorSkills.Count > 0)
                    {
                        foreach (var i in getMentorSkills)
                        {
                            var Check = await con.userSkills.Where(a => a.StId == i.StId && a.usrId == id).AnyAsync();
                            if (Check == false)
                            {
                                UserSkills skill = new UserSkills();
                                skill.StId = i.StId;
                                skill.usDetail = i.usDetail;
                                skill.usLevel = i.usLevel;
                                skill.Status = true;
                                skill.usrId = id;

                                var Count = con.userSkills.Where(a => a.usrId == id).Count();
                                if (Count >= 15)
                                {
                                    return BadRequest(new { message = "Your Skills Limit completed. You Can add upto 15 Skills." });
                                }
                                else
                                {

                                    con.userSkills.Add(skill);
                                    try
                                    {
                                        await con.SaveChangesAsync();
                                        //return Ok();
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                                        return BadRequest(new { message = "An error occured while adding the user skill Info. Please try again later" });
                                    }
                                }
                            }

                        }
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(new { message = "No Mentor Skill found" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "User not found" });
                }


            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while Updating Info. Please try again later" });
            }
        }


        [HttpGet("GetUserWhiteBoard")]
        public async Task<ActionResult<UserWhiteBoard>> GetUserWhiteBoard(int id)
        {
            try
            {
                var getData = await (from u in con.userWhiteBoards
                                     join b in con.userSkills on u.usId equals b.usId
                                     join s in con.skillTypes on b.StId equals s.StId
                                     where u.usrId == id
                                     select new
                                     {
                                         u.uwbId,
                                         u.uwbName,
                                         u.uwbDetail,
                                         u.uwbDay,
                                         u.uwbTime,
                                         u.Status,
                                         b.usId,
                                         usName = s.StName
                                     }).OrderByDescending(o => o.uwbId).ToListAsync();
                List<UserWhiteBoardVM> whiteList = new List<UserWhiteBoardVM>();
                if (getData.Count > 0)
                {

                    foreach (var i in getData)
                    {
                        UserWhiteBoardVM white = new UserWhiteBoardVM();
                        white.uwbId = i.uwbId;
                        white.uwbName = i.uwbName;
                        white.uwbDay = i.uwbDay;
                        white.uwbTime = i.uwbTime;
                        white.Status = i.Status;
                        white.usrId = i.usId;
                        white.usName = i.usName;
                        string detail = i.uwbDetail;
                        string twenty = "";
                        if (!String.IsNullOrWhiteSpace(detail) && detail.Length >= 5)
                        {
                            if (!String.IsNullOrWhiteSpace(detail) && detail.Length <= 30)
                            {
                                white.uwbDetail = i.uwbDetail;
                            }
                            else
                            {
                                twenty = detail.Substring(0, 30);
                                white.uwbDetail = twenty;
                            }
                        }
                        else
                        {
                            white.uwbDetail = i.uwbDetail;
                        }

                        whiteList.Add(white);
                    }
                    return Ok(whiteList);
                }
                else
                {
                    return Ok(getData);
                }


            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occured while getting the user whaite board Info data. Please try again later. " + ex + "" });
            }
        }

        [HttpGet("GetWhiteBoardDetail")]
        public async Task<ActionResult<UserWhiteBoard>> GetWhiteBoardDetail(int id)
        {
            try
            {
                //var getData = await con.userWhiteBoards.Where(u => u.usrId == id).ToListAsync();
                var getData = await (from u in con.userWhiteBoards
                                     join b in con.userSkills on u.usId equals b.usId
                                     join s in con.skillTypes on b.StId equals s.StId
                                     where u.uwbId == id
                                     select new
                                     {
                                         u.usrId,
                                         u.uwbId,
                                         u.uwbName,
                                         u.uwbDetail,
                                         u.uwbDay,
                                         u.uwbTime,
                                         u.Status,
                                         usName = s.StName
                                     }).FirstOrDefaultAsync();
                return Ok(getData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occured while getting the user whaite board Info data. Please try again later. " + ex + "" });
            }
        }


        [HttpPost("AddUserWhiteBoard")]
        public async Task<ActionResult<UserWhiteBoard>> PostUserWhiteBoard(UserWhiteBoard usr)
        {
            try
            {
                var Count = con.userWhiteBoards.Where(a => a.usrId == usr.usrId).Count();
                if (Count >= 15)
                {
                    return BadRequest(new { message = "You Can add upto 15 Lectures." });
                }
                else
                {
                    usr.Status = true;
                    con.userWhiteBoards.Add(usr);
                    try
                    {
                        await con.SaveChangesAsync();
                        return Ok();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return BadRequest(new { message = "An error occured while adding the user white board Info. Please try again later" });
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "Data not added." });
            }
        }

        [HttpPost("DelWhiteBoard")]
        public async Task<ActionResult> DelWhiteBoard(UserWhiteBoard data)
        {
            var getData = await con.userWhiteBoards.Where(u => u.uwbId == data.uwbId && u.usrId == data.usrId).FirstOrDefaultAsync();
            if (getData == null)
            {
                return BadRequest(new { message = "No record found" });
            }

            con.userWhiteBoards.Remove(getData);
            await con.SaveChangesAsync();

            return Ok();
        }
        [HttpPost("GetWhiteBoard")]
        public async Task<ActionResult<UserWhiteBoard>> GetWhiteBoard(UserWhiteBoard data)
        {
            try
            {
                var GetData = await con.userWhiteBoards.Where(u => u.uwbId == data.uwbId && u.usrId == data.usrId).FirstOrDefaultAsync();

                if (GetData == null)
                {
                    return BadRequest(new { message = "No record found" });
                }

                return GetData;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the Info. Please try again later." });
            }
        }

        [HttpPut("UpdateWhiteBoard")]
        public async Task<IActionResult> UpdateWhiteBoard(int id, UserWhiteBoard data)
        {
            if (id != data.uwbId)
            {
                return BadRequest();
            }
            var getData = await con.userWhiteBoards.Where(u => u.uwbId == id && u.usrId == data.usrId).FirstOrDefaultAsync();
            if (getData != null)
            {
                getData.uwbName = data.uwbName;
                getData.usId = data.usId;
                getData.uwbDetail = data.uwbDetail;
                getData.uwbTime = data.uwbTime;
                getData.uwbDay = data.uwbDay;
                getData.Status = true;
                con.Entry(getData).State = EntityState.Modified;
            }
            else
            {
                return BadRequest(new { message = "No record found" });
            }
            try
            {
                await con.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "An error occured while Updating Info. Please try again later" });
            }

            return NoContent();
        }


        [HttpGet("PopulateBank")]
        public async Task<ActionResult<IEnumerable<Bank>>> GetBank()
        {
            return await con.banks.Where(b => b.Status == true).ToListAsync();
        }
        [HttpGet("PopulateUserBank")]
        public async Task<ActionResult<IEnumerable<Bank>>> PopulateUserBank(int Id)
        {
            return await con.banks.Where(b => b.Status == true && b.CountryId == Id).ToListAsync();
        }

        [HttpGet("GetUserBank")]
        public async Task<ActionResult<Bank>> GetUserBank(int id)
        {
            return await con.banks.Where(b => b.BankId == id && b.Status == true).FirstOrDefaultAsync();
        }

        [HttpGet("PopulateSkills")]
        public async Task<ActionResult<IEnumerable<UserSkills>>> GetSkills()
        {
            return await con.userSkills.Where(b => b.Status == true).ToListAsync();
        }


        [HttpGet("GetUserSkill")]
        public async Task<ActionResult> GetUserSkill(int id)
        {
            var getData = await (from sk in con.userSkills
                                 join st in con.skillTypes on sk.StId equals st.StId
                                 where sk.usrId == id
                                 select new
                                 {
                                     sk.usId,
                                     st.StName,
                                 }).OrderByDescending(o => o.usId).ToListAsync();
            return Ok(getData);
            //var getSkills = await con.userSkills.Where(b => b.usrId == id && b.Status == true).ToListAsync();
            //var getData = from sk in 
            //if(getSkills.Count > 0)
            //{
            //    List<UserSkills> sList = new List<UserSkills>();
            //    foreach(var i in getSkills)
            //    {
            //        UserSkills skills = new UserSkills();

            //    }
            //}
            //return await con.userSkills.Where(b => b.usrId == id && b.Status == true).ToListAsync();
        }

        [HttpGet("GetAllSkill")]
        public async Task<ActionResult<IEnumerable<SkillType>>> GetAllSkill()
        {
            var getSkills = await (from st in con.skillTypes
                                   select new
                                   {
                                       st.StId,
                                       st.StName
                                   }).ToListAsync();
            return Ok(getSkills);
        }

        [HttpGet("GeSkillDetail")]
        public JsonResult GeSkillDetail(int val)
        {
            if (val != 0)
            {
                var GetDetail = con.skillTypes.Where(a => a.StId == val).FirstOrDefault();
                var GenName = GetDetail.StDetail;
                return new JsonResult(new { Success = "true", Data = new { GenName } });
            }
            return new JsonResult(new { Success = "false" });
        }

        [HttpGet("GetUserName")]
        public async Task<ActionResult<Users>> GetUserName(int id)
        {
            return await con.users.Where(b => b.usrId == id).FirstOrDefaultAsync();
        }


        [HttpGet("GetMyTeam")]
        public async Task<ActionResult<IEnumerable<Users>>> GetMyTeam(int id)
        {
            return await con.users.Where(b => b.refId == id).ToListAsync();
        }
        [HttpGet("GetMyMentorSkill")]
        public async Task<ActionResult> GetMyMentorSkill(int id)
        {
            var getData = await (from sk in con.userSkills
                                 join st in con.skillTypes on sk.StId equals st.StId
                                 where sk.usrId == id
                                 select new
                                 {
                                     sk.usId,
                                     sk.StId,
                                     st.StName,
                                     sk.usDetail,
                                     sk.usLevel,
                                     sk.usrId,
                                     sk.Status,
                                     st.StCoverImage,
                                     st.StImage
                                 }).ToListAsync();
            return Ok(getData);
            //return await con.userSkills.Where(b => b.usrId == id).ToListAsync();
        }

        [HttpGet("GetMyMentorWhiteBoard")]
        public async Task<ActionResult<IEnumerable<UserWhiteBoard>>> GetMyMentorWhiteBoard(int id)
        {
            var getData = await (from u in con.userWhiteBoards
                                 join b in con.userSkills on u.usId equals b.usId
                                 join s in con.skillTypes on b.StId equals s.StId
                                 where u.usrId == id
                                 select new
                                 {
                                     u.uwbId,
                                     u.uwbName,
                                     u.uwbDetail,
                                     u.Status,
                                     usName = s.StName
                                 }).OrderByDescending(o => o.uwbId).ToListAsync();
            return Ok(getData);
            //return await con.userWhiteBoards.Where(b => b.usrId == id && b.Status == true).ToListAsync();
        }


        [HttpGet("GetMyHelpingMaterial")]
        public async Task<ActionResult<List<UserHelpingMaterial>>> GetMyHelpingMaterial()
        {
            var getData = await con.userHelpingMaterial.Where(b => b.umsStatus == true).ToListAsync();
            return Ok(getData);
        }

        [HttpGet("CheckEmailVerify")]
        public async Task<ActionResult> CheckEmailVerify(int id)
        {
            var getEmailId = await con.userVerificationTypes.Where(u => u.uvtName == "Email").FirstOrDefaultAsync();
            if (getEmailId != null)
            {
                var checkData = await con.userVerifications.Where(us => us.usrId == id && us.uvtId == getEmailId.uvtId && us.uvStatus == "Verified").AnyAsync();
                if (checkData == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = "Email not verified" });
                }
            }
            return BadRequest(new { message = "Email not verified" });
        }

        [HttpGet("GetRefData")]
        public async Task<ActionResult<Users>> GetRefData(int id)
        {
            return await con.users.Where(b => b.usrId == id).FirstOrDefaultAsync();
        }

        [HttpGet("GetMyLostTrainee")]
        public async Task<ActionResult> GetMyLostTrainee(int id)
        {
            try
            {
                var GetData = await (from u in con.userLostTrainees
                                     join us in con.users on u.usrIdC equals us.usrId
                                     where u.usrIdM == id
                                     select new
                                     {
                                         u.ultReason,
                                         us.usrName,
                                         us.usrGender,
                                         us.usrPhone

                                     }).ToListAsync();
                //var getLostTraine = await con.userLostTrainees.Where(b => b.usrIdM == id).ToListAsync();
                //if (getLostTraine.Count != 0)
                //{
                //    foreach (var i in getLostTraine)
                //    {
                //        var getUser = await con.users.Where(b => b.usrId == i.usrIdC).FirstOrDefaultAsync();

                //        listusers.Add(getUser);
                //    }
                //}
                return Ok(GetData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the info. Please try again later" });
            }



        }

        [HttpGet("GetCountryList")]
        public async Task<ActionResult<List<Country>>> GetCountryList()
        {
            try
            {
                List<Country> list = new List<Country>();
                var GetData = await con.countries.Where(t => t.Status == true).OrderBy(c => c.CountryName).ToListAsync();
                if (GetData.Count > 0)
                {
                    foreach (var i in GetData)
                    {
                        Country c = new Country();
                        c.CountryId = i.CountryId;
                        c.CountryName = i.CountryName + "(" + i.CountryCode + ")";
                        c.CountryCode = i.CountryCode;
                        list.Add(c);
                    }
                }
                return list;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the info. Please try again later" });
            }
        }

        [HttpGet("GetCountry")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            try
            {

                var GetData = await con.countries.Where(c => c.CountryId == id).FirstOrDefaultAsync();
                return GetData;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the info. Please try again later" });
            }
        }


        [HttpGet("GetSkillType")]
        public async Task<ActionResult> GetSkillType()
        {
            var getData = await (from sk in con.skillTypes
                                 join st in con.skillTypes on sk.SubType equals st.StId
                                 select new
                                 {
                                     sk.StId,
                                     sk.StName,
                                     sk.StDetail,
                                     sk.StStatus,
                                     sk.StCategory,
                                     sk.SubType,
                                     sk.StCoverImage,
                                     sk.StImage,
                                     SubTypeName = st.StName
                                 }).ToListAsync();
            return Ok(getData);
            //return await con.userSkills.Where(b => b.usrId == id).ToListAsync();
        }

        [HttpPost("AddSkillType")]
        public async Task<ActionResult> AddSkillType(SkillType data)
        {
            try
            {
                if(data.SubType == 0)
                {
                    con.skillTypes.Add(data);
                    await con.SaveChangesAsync();
                    

                    data.SubType = data.StId;
                    con.Entry(data).State = EntityState.Modified;
                    await con.SaveChangesAsync();
                    return Ok();

                }
                else
                {
                    con.skillTypes.Add(data);
                    await con.SaveChangesAsync();
                    return Ok();
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "an error occured. Data not added" });
            }
        }

        [HttpDelete("DelSkillType")]
        public async Task<ActionResult> DelSkillType(int id)
        {
            var ChkData = await con.userSkills.Where(b => b.StId == id).AnyAsync();
            if (ChkData == true)
            {
                return BadRequest(new { message = "This Skill type cannot be deleted because it is associated with some info" });
            }
            else
            {
                var getData = await con.skillTypes.FindAsync(id);
                if (getData == null)
                {
                    return NotFound();
                }

                con.skillTypes.Remove(getData);
                await con.SaveChangesAsync();
                return Ok();
            }


        }

        [HttpGet("EditSkillType")]
        public async Task<ActionResult<SkillType>> EditSkillType(int id)
        {
            var getData = await con.skillTypes.FindAsync(id);
            if (getData != null)
            {
                return getData;
            }
            else
            {
                return BadRequest(new { message = "No Record found" });
            }

        }

        [HttpPut("UpdateSkillType")]
        public async Task<IActionResult> UpdateSkillType(int id, SkillType data)
        {
            var getData = await con.skillTypes.FindAsync(id);
            if (getData != null)
            {
                getData.SubType = data.SubType;
                getData.StDetail = data.StDetail;
                getData.StName = data.StName;
                getData.StStatus = data.StStatus;
                getData.StCategory = data.StCategory;
                if (data.StImage != null)
                {
                    getData.StImage = data.StImage;
                }
                if (data.StCoverImage != null)
                {
                    getData.StCoverImage = data.StCoverImage;
                }



                con.Entry(getData).State = EntityState.Modified;
            }
            else
            {
                return BadRequest(new { message = "No record found" });
            }
            try
            {
                await con.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "An error occured while Updating Info. Please try again later" });
            }

            
        }
    }
}