using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasySystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserVerificationController : ControllerBase
    {
        private readonly EasyContext con;
        public UserVerificationController(EasyContext context)
        {
            con = context;
        }


        [HttpGet("GetVerificationTypeData")]
        public async Task<ActionResult> GetVerificationTypeData()
        {
            try
            {
                var getData = await con.userVerificationTypes.ToListAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the data. Please try again later." });
            }
        }

        [HttpPost("AddVerificationType")]
        public async Task<ActionResult> AddVerificationType(UserVerificationType data)
        {
            try
            {
                con.userVerificationTypes.Add(data);
                await con.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "an error occured. Data not added" });
            }
        }

        [HttpDelete("DelVerificationType")]
        public async Task<ActionResult> DelVerificationType(int id)
        {

            var getData = await con.userVerificationTypes.FindAsync(id);
            if (getData == null)
            {
                return NotFound();
            }

            con.userVerificationTypes.Remove(getData);
            await con.SaveChangesAsync();
            return Ok();



        }

        [HttpGet("GetVerificationType")]
        public async Task<ActionResult<UserVerificationType>> GetVerificationType(int id)
        {
            try
            {
                var GetData = await con.userVerificationTypes.FindAsync(id);

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

        [HttpPut("UpdateVerificationType")]
        public async Task<IActionResult> UpdateVerificationType(int id, UserVerificationType data)
        {
            var getData = await con.userVerificationTypes.FindAsync(id);
            if (getData != null)
            {
                getData.uvtName = data.uvtName;
                getData.uvtText = data.uvtText;
                getData.uvtStatus = data.uvtStatus;

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


        [HttpGet("GetVerificationTypeForUser")]
        public async Task<ActionResult> GetVerificationTypeForUser()
        {
            try
            {
                var getData = await con.userVerificationTypes.Where(u => u.uvtStatus == true).ToListAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the data. Please try again later." });
            }
        }

        [HttpGet("GetUserVerificationUser")]
        public async Task<ActionResult> GetUserVerificationUser(int id)
        {
            try
            {
                var getData = await con.userVerifications.Where(u => u.usrId == id).ToListAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the data. Please try again later." });
            }
        }

        [HttpPost("AddUserVerificationData")]
        public async Task<ActionResult> AddUserVerificationData(UserVerification data)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var chkData = await con.userVerifications.Where(u => u.uvtId == data.uvtId
                    && u.usrId == data.usrId).OrderByDescending(o => o.uvId).FirstOrDefaultAsync();
                    if (chkData != null)
                    {
                        if (chkData.uvStatus == "Pending" || chkData.uvStatus == "Rejected")
                        {
                            var chkEmail = await con.userVerificationTypes.Where(v => v.uvtId == data.uvtId).FirstOrDefaultAsync();

                            if (chkEmail.uvtName == "Email")
                            {
                                if (data.uvStatus == "Verify")
                                {
                                    if (chkData.uvText == data.uvText)
                                    {
                                        chkData.uvStatusDate = DateTime.Now;
                                        chkData.uvStatus = "Verified";
                                        chkData.uvStatusRemarks = "Your Email is verified successfully";
                                        con.Entry(chkData).State = EntityState.Modified;
                                        try
                                        {
                                            await con.SaveChangesAsync();
                                            return Ok(new { message = "Email Verified successfully" });
                                        }
                                        catch (DbUpdateConcurrencyException)
                                        {
                                            return BadRequest(new { message = "An error occured while adding the User Verification Data. Please try again later" });
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest(new { message = "Invalid email verification code" });
                                    }
                                }
                                else
                                {
                                    //Update User Email
                                    var chkMail = await con.users.Where(u => u.usrEmail == data.uvStatusRemarks && u.usrId != data.usrId).AnyAsync();
                                    if(chkMail == false)
                                    {
                                        var getUser = await con.users.Where(u => u.usrId == data.usrId).FirstOrDefaultAsync();
                                        if (getUser != null)
                                        {
                                            getUser.usrEmail = data.uvStatusRemarks;
                                            con.Entry(getUser).State = EntityState.Modified;
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest(new { message = "This email is already exist.Please choose a different one" });
                                    }
                                    
                                    // Sending EMail Code + Data Entry
                                    data.uvText = RandomString(6);
                                    string emailBody = "<b>" + data.uvText + "</b> is your Universal Skills email verification Code. Please go to verification page and verify you email.";
                                    Mailer email = new Mailer("Care@Universalskills.co", data.uvStatusRemarks, "Email Verification", emailBody);
                                    email.Send();
                                    chkData.uvText = data.uvText;
                                    chkData.uvCreatedDate = DateTime.Now;
                                    chkData.uvStatus = "Pending";
                                    chkData.uvStatusRemarks = null;
                                    chkData.uvStatusDate = DateTime.Now.AddYears(-100);
                                    con.Entry(chkData).State = EntityState.Modified;

                                    
                                    try
                                    {
                                        await con.SaveChangesAsync();
                                        return Ok(new { message = "Please check your email. Verification code sent to your mail box successfully." });
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                                        return BadRequest(new { message = "An error occured while adding the User Verification Data. Please try again later" });
                                    }
                                }

                            }
                            else
                            {
                                if (data.uvImagePath != null)
                                {
                                    chkData.uvImagePath = data.uvImagePath;
                                }
                                if (data.uvText != null)
                                {
                                    chkData.uvText = data.uvText;
                                }
                                chkData.uvCreatedDate = DateTime.Now;
                                chkData.uvStatus = "Pending";
                                chkData.uvStatusRemarks = null;
                                chkData.uvStatusDate = DateTime.Now.AddYears(-100);
                                con.Entry(chkData).State = EntityState.Modified;
                                try
                                {
                                    await con.SaveChangesAsync();
                                    return Ok(new { message = "Data Updated successfully"});
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return BadRequest(new { message = "An error occured while adding the User Verification Data. Please try again later" });
                                }
                            }

                        }
                        else if (chkData.uvStatus == "Under Process" || chkData.uvStatus == "Verified")
                        {
                            return BadRequest(new { message = "You cannot upload new Document because your current document is " + chkData.uvStatus + "" });
                        }
                    }
                    else
                    {
                        var chkUVT = await con.userVerificationTypes.Where(v => v.uvtId == data.uvtId).FirstOrDefaultAsync();
                        if (chkUVT != null)
                        {
                            if (chkUVT.uvtName == "Email")
                            {
                                //Update User Email
                                var chkMail = await con.users.Where(u => u.usrEmail == data.uvStatusRemarks && u.usrId != data.usrId).AnyAsync();
                                if (chkMail == false)
                                {
                                    var getUser = await con.users.Where(u => u.usrId == data.usrId).FirstOrDefaultAsync();
                                    if (getUser != null)
                                    {
                                        getUser.usrEmail = data.uvStatusRemarks;
                                        con.Entry(getUser).State = EntityState.Modified;
                                    }
                                }
                                else
                                {
                                    return BadRequest(new { message = "This email is already exist.Please choose a different one" });
                                }
                                

                                data.uvText = RandomString(6);
                                string emailBody = "<b>" + data.uvText + "</b> is your Universal Skills email verification Code. Please go to verification page and verify you email.";
                                Mailer email = new Mailer("Care@Universalskills.co", data.uvStatusRemarks, "Email Verification", emailBody);
                                email.Send();

                                data.uvCreatedDate = DateTime.Now;
                                data.uvStatus = "Pending";
                                data.uvStatusDate = DateTime.Now.AddYears(-100);
                                data.uvStatusRemarks = null;
                                try
                                {
                                    con.userVerifications.Add(data);
                                    await con.SaveChangesAsync();
                                    return Ok(new { message = "Please check your email. Verification code sent to your mail box successfully." });
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return BadRequest(new { message = "An error occured while adding the User Verification Data. Please try again later" });
                                }
                            }
                        }
                        data.uvCreatedDate = DateTime.Now;
                        data.uvStatus = "Pending";
                        data.uvStatusDate = DateTime.Now.AddYears(-100);
                        data.uvStatusRemarks = null;
                        try
                        {
                            con.userVerifications.Add(data);
                            await con.SaveChangesAsync();
                            return Ok(new { message = "Data Uploaded successfully." });
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return BadRequest(new { message = "An error occured while adding the User Verification Data. Please try again later" });
                        }
                    }

                }
                else
                {
                    return BadRequest(new { message = "Parameteres not Completed please try again" });
                }
                return Ok();
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while Adding the coupan values. Please try again later" });
            }

        }

        [HttpGet("GetVerificationData")]
        public async Task<ActionResult> GetVerificationData()
        {
            try
            {
                var getData = await (from u in con.userVerifications
                                     join us in con.users on u.usrId equals us.usrId
                                     //join ad in con.users on u.uvStatusBy equals ad.usrId
                                     join uvt in con.userVerificationTypes on u.uvtId equals uvt.uvtId
                                     select new
                                     {
                                         u.uvId,
                                         u.uvtId,
                                         u.usrId,
                                         u.uvText,
                                         u.uvCreatedDate,
                                         u.uvStatus,
                                         u.uvStatusRemarks,
                                         u.uvStatusDate,
                                         u.uvImagePath,
                                         uvt.uvtName,
                                         //Admin = ad.usrName,
                                         us.usrName
                                     }).OrderByDescending(o => o.uvId).OrderByDescending(o => o.uvStatus == "Pending").ToListAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the data. Please try again later." });
            }
        }


        [HttpPost("VerifyUserVerificationData")]
        public async Task<ActionResult> VerifyUserVerificationData(UserVerification data)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    data.uvCreatedDate = DateTime.Now;
                    data.uvStatus = "Pending";
                    data.uvStatusDate = DateTime.Now.AddYears(-100);
                    data.uvStatus = "Pending";
                    try
                    {
                        con.userVerifications.Add(data);
                        await con.SaveChangesAsync();
                        return Ok();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return BadRequest(new { message = "An error occured while adding the User Verification Data. Please try again later" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Parameteres not Completed please try again" });
                }

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while Adding the coupan values. Please try again later" });
            }
            
        }

        [HttpGet("GetUserVerificationData")]
        public async Task<ActionResult> GetUserVerificationData(int id)
        {
            try
            {
                var getData = await (from u in con.userVerifications
                                     join us in con.users on u.usrId equals us.usrId
                                     //join ad in con.users on u.uvStatusBy equals ad.usrId
                                     join uvt in con.userVerificationTypes on u.uvtId equals uvt.uvtId
                                     where u.uvId == id
                                     select new
                                     {
                                         u.uvId,
                                         u.uvtId,
                                         u.usrId,
                                         u.uvText,
                                         u.uvCreatedDate,
                                         u.uvStatus,
                                         u.uvStatusRemarks,
                                         u.uvStatusDate,
                                         u.uvImagePath,
                                         uvt.uvtName,
                                         //Admin = ad.usrName,
                                         us.usrName
                                     }).FirstOrDefaultAsync();
                return Ok(getData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the data. Please try again later." });
            }
        }

        [HttpPost("UpdateVerificationData")]
        public async Task<ActionResult> UpdateVerificationData(UserVerification data)
        {
            try
            {
                var getData = await con.userVerifications.Where(u => u.uvId == data.uvId).FirstOrDefaultAsync();

                if (getData != null)
                {
                    getData.uvStatus = data.uvStatus;
                    getData.uvStatusDate = DateTime.Now;
                    getData.uvStatusBy = data.uvStatusBy;
                    if (data.uvStatusRemarks != null)
                    {
                        getData.uvStatusRemarks = data.uvStatusRemarks;
                    }
                    con.Entry(getData).State = EntityState.Modified;
                    try
                    {
                        await con.SaveChangesAsync();
                        return Ok();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return BadRequest(new { message = "An error occured while adding the User Verification Data. Please try again later" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "No Record found" });
                }
                
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while Adding the coupan values. Please try again later" });
            }

        }


        public string RandomString(int length)
        {
            const string chars = "123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}