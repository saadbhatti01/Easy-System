using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasySystemAPI.Models;
using System.Net;

namespace EasySystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSignUpCodesController : ControllerBase
    {
        private readonly EasyContext con;
        SendMessage msg = new SendMessage();

        public UserSignUpCodesController(EasyContext context)
        {
            con = context;
        }

        // GET: api/UserSignUpCodes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSignUpCode>>> GetuserSignUps()
        {
            return await con.userSignUps.OrderByDescending(c => c.uscId).ToListAsync();
        }

        // GET: api/UserSignUpCodes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserSignUpCode>> GetUserSignUpCode(int id)
        {
            var userSignUpCode = await con.userSignUps.FindAsync(id);

            if (userSignUpCode == null)
            {
                return NotFound();
            }

            return userSignUpCode;
        }

        // PUT: api/UserSignUpCodes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserSignUpCode(int id, UserSignUpCode userSignUpCode)
        {
            if (id != userSignUpCode.uscId)
            {
                return BadRequest();
            }

            con.Entry(userSignUpCode).State = EntityState.Modified;

            try
            {
                await con.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserSignUpCodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserSignUpCodes
        [HttpPost]
        public async Task<ActionResult<UserSignUpCode>> PostUserSignUpCode(UserSignUpCode userSignUpCode)
        {
            try
            {
                if (userSignUpCode.uscStatus == "SignUp")
                {
                    var chkData = UserSignUpCodePhoneNoExists(userSignUpCode.usPhone);
                    if (chkData == true)
                    {
                        var ChkStatus = await con.userSignUps.Where(e => e.usPhone == userSignUpCode.usPhone).FirstOrDefaultAsync();
                        if (ChkStatus != null)
                        {
                            if (ChkStatus.uscStatus == "Pending")
                            {
                                ChkStatus.uscCode = Convert.ToInt32(RandomString(6));
                                ChkStatus.uscExpire = DateTime.Now.AddMinutes(10);
                                ChkStatus.usEmail = "a@a.a";
                                ChkStatus.uscStatus = "Pending";
                                con.Entry(chkData).State = EntityState.Modified;
                                try
                                {
                                    var msgboday = "Your OTP for Universal Skills verification is " + ChkStatus.uscCode.ToString() + "";
                                    msg.SendSMSTurab(userSignUpCode.usPhone, msgboday);
                                    await con.SaveChangesAsync();
                                    return Ok();
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return BadRequest(new { message = "An error occured. Request not updated" });
                                }
                            }
                            else if (ChkStatus.uscStatus == "Verified")
                            {
                                return BadRequest(new { message = "This Phone Number is verified. Please go to Register Page." });
                            }
                            else if (ChkStatus.uscStatus == "Active")
                            {
                                return BadRequest(new { message = "This Phone Number is already exist please choose a different one. OR For getting password please" });
                            }
                        }

                        return BadRequest(new { message = "This Phone Number is already exist please choose a different one" });
                    }
                    else
                    {
                        userSignUpCode.uscCode = Convert.ToInt32(RandomString(6));
                        userSignUpCode.uscExpire = DateTime.Now.AddMinutes(10);
                        userSignUpCode.usEmail = "care@EasySystem.com";
                        userSignUpCode.uscStatus = "Pending";
                        if (!ModelState.IsValid)
                        {
                            return BadRequest(new { message = "Not a valid Parameters.Please try again" });
                        }
                        con.userSignUps.Add(userSignUpCode);
                        try
                        {
                            var msgboday = "Your OTP for Universal Skills verification is " + userSignUpCode.uscCode.ToString() + "";
                            msg.SendSMSTurab(userSignUpCode.usPhone, msgboday);
                            await con.SaveChangesAsync();
                            return Ok();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return BadRequest(new { message = "An error occured. Please try again" });
                        }
                    }
                }
                else if (userSignUpCode.uscStatus == "Forget")
                {
                    var chkData = await con.userSignUps.Where(x => x.usPhone == userSignUpCode.usPhone).FirstOrDefaultAsync();
                    if (chkData != null)
                    {
                        if (chkData.uscStatus == "Pending")
                        {
                            if (chkData.uscCode == userSignUpCode.uscCode)
                            {
                                if (chkData.uscExpire >= DateTime.Now)
                                {
                                    chkData.uscStatus = "Verified";
                                    con.Entry(chkData).State = EntityState.Modified;

                                    try
                                    {
                                        await con.SaveChangesAsync();
                                        return Ok();
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                                        return BadRequest(new { message = "An error occured. Request not updated" });
                                    }
                                }
                                else
                                {
                                    return BadRequest(new { message = "This code is expired. Please Click on resend code to get the new ONE." });
                                }
                            }
                            else
                            {
                                return BadRequest(new { message = "Invalid Code. Please Enter the correct Code" });
                            }
                        }
                        else if (chkData.uscStatus == "Reset")
                        {
                            if (chkData.uscCode == userSignUpCode.uscCode)
                            {
                                if (chkData.uscExpire >= DateTime.Now)
                                {
                                    chkData.uscStatus = "Verified";
                                    con.Entry(chkData).State = EntityState.Modified;
                                    try
                                    {
                                        await con.SaveChangesAsync();
                                        return Ok();
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                                        return BadRequest(new { message = "An error occured. Request not updated" });
                                    }
                                }
                                else
                                {
                                    return BadRequest(new { message = "This code is expired. Please Click on resend code to get the new ONE." });
                                }
                            }
                            else
                            {
                                return BadRequest(new { message = "Invalid Code. Please Enter the correct Code" });
                            }
                        }
                        else if (chkData.uscStatus == "Active")
                        {
                            chkData.uscCode = Convert.ToInt32(RandomString(6));
                            chkData.uscStatus = "Reset";
                            chkData.uscExpire = DateTime.Now.AddMinutes(10);
                            con.Entry(chkData).State = EntityState.Modified;
                            try
                            {
                                var msgboday = "Your OTP for Universal Skills verification is " + chkData.uscCode.ToString() + "";
                                msg.SendSMSTurab(userSignUpCode.usPhone, msgboday);
                                await con.SaveChangesAsync();
                                return Ok();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return BadRequest(new { message = "An error occured. Request not updated" });
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid Phone Number. Please enter the correct Phone Number" });
                    }
                }

                return Ok();
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "Not a valid Parameters.Please try again" });
            }
        }

        [HttpPost("RegisterPhoneNo")]
        public async Task<ActionResult<UserSignUpCode>> RegisterPhoneNo(PhoneNumber number)
        {
            try
            {
                if (number.Code != null && number.Num != null)
                {
                    var getData = con.countries.Where(c => c.CountryCode == number.Code).FirstOrDefault();
                    if (getData != null)
                    {
                        number.Num = number.Num.Trim();
                        var Num = number.Num;
                        if (Num.Length >= getData.NumLength)
                        {
                            string getDigit = Num.Substring(Num.Length - getData.NumLength);
                            string getFirst = getDigit.Substring(0, 1);
                            if (Convert.ToInt32(getFirst) == getData.NumStart)
                            {
                                var PhoneNo = number.Code + getDigit;
                                UserSignUpCode userSignUpCode = new UserSignUpCode();
                                userSignUpCode.usPhone = PhoneNo;
                                userSignUpCode.usPhone = userSignUpCode.usPhone.Trim();
                                var ChkData = await con.userSignUps.Where(e => e.usPhone == userSignUpCode.usPhone).FirstOrDefaultAsync();
                                if (ChkData != null)
                                {
                                    if (ChkData.uscStatus == "Pending")
                                    {
                                        ChkData.uscCode = Convert.ToInt32(RandomString(6));
                                        ChkData.uscExpire = DateTime.Now.AddMinutes(10);
                                        ChkData.usEmail = "";
                                        ChkData.uscStatus = "Pending";
                                        if (!ModelState.IsValid)
                                        {
                                            return BadRequest(new { message = "Not a valid Parameters.Please try again" });
                                        }
                                        con.Entry(ChkData).State = EntityState.Modified;
                                        try
                                        {
                                            var msgboday = "" + ChkData.uscCode.ToString() + " is your Universal Skills verifcation code.";
                                            msg.SendSMSTurab(userSignUpCode.usPhone, msgboday);

                                            //if (number.Code == "+92")
                                            //{
                                            //    msg.SendSMSTurabPak(userSignUpCode.usPhone, msgboday);
                                            //}
                                            //else
                                            //{
                                            //    msg.SendSMSTurab(userSignUpCode.usPhone, msgboday);
                                            //}
                                            await con.SaveChangesAsync();
                                            return Ok(userSignUpCode);
                                        }
                                        catch (DbUpdateConcurrencyException)
                                        {
                                            return BadRequest(new { message = "Phone number not registered please try again later" });
                                        }
                                    }
                                    else if (ChkData.uscStatus == "Verified")
                                    {
                                        return BadRequest(new { message = "This Phone Number is verified. Please go to Register Page." });
                                    }
                                    else if (ChkData.uscStatus == "Active")
                                    {
                                        return BadRequest(new { message = "This Phone Number is already exist please choose a different one." });
                                    }
                                }
                                else
                                {
                                    userSignUpCode.uscCode = Convert.ToInt32(RandomString(6));
                                    userSignUpCode.uscExpire = DateTime.Now.AddMinutes(10);
                                    userSignUpCode.usEmail = "";
                                    userSignUpCode.uscStatus = "Pending";
                                    if (!ModelState.IsValid)
                                    {
                                        return BadRequest(new { message = "Not a valid Parameters.Please try again" });
                                    }
                                    con.userSignUps.Add(userSignUpCode);
                                    try
                                    {
                                        var msgboday = "" + userSignUpCode.uscCode.ToString() + " is your Universal Skills verifcation code.";
                                        msg.SendSMSTurab(userSignUpCode.usPhone, msgboday);
                                        //if (number.Code == "+92")
                                        //{
                                        //    msg.SendSMSTurabPak(userSignUpCode.usPhone, msgboday);
                                        //}
                                        //else
                                        //{
                                        //    msg.SendSMSTurab(userSignUpCode.usPhone, msgboday);
                                        //}
                                        await con.SaveChangesAsync();
                                        return Ok(userSignUpCode);
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                                        return BadRequest(new { message = "Phone number not registered please try again later" });
                                    }
                                }
                                return BadRequest(new { message = "Not a valid Parameters.Please try again." });

                            }
                            else
                            {
                                return BadRequest(new { message = "Invalid Phone Number. Phone Number of '" + getData.CountryName + "' must be start with '" + getData.NumStart + "' digit" });

                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Not a valid Phone Number.Minimum required length is " + getData.NumLength + " digit of " + getData.CountryName + " number. " });
                        }

                    }
                    else
                    {
                        return BadRequest(new { message = "No Country Code found. Please try again later" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Number and Code not found.Please try again." });
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while registering Phone number. please try again later." });
            }
        }

        [HttpPost("RegisterCodeVerification")]
        public async Task<ActionResult<UserSignUpCode>> RegisterCodeVerification(UserSignUpCode userSignUpCode)
        {
            try
            {
                userSignUpCode.usPhone = userSignUpCode.usPhone.Trim();
                var chkData = await con.userSignUps.Where(x => x.usPhone == userSignUpCode.usPhone).FirstOrDefaultAsync();
                if (chkData != null)
                {
                    if (chkData.uscStatus == "Pending")
                    {
                        if (chkData.uscCode == userSignUpCode.uscCode)
                        {
                            if (chkData.uscExpire >= DateTime.Now)
                            {
                                chkData.uscStatus = "Verified";
                                con.Entry(chkData).State = EntityState.Modified;
                                try
                                {
                                    await con.SaveChangesAsync();
                                    return Ok();
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return BadRequest(new { message = "An error occured. Request not completed" });
                                }
                            }
                            else
                            {
                                return BadRequest(new { message = "This code is expired. Please Click on resend code to get the new ONE." });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Invalid Code. Please Enter the correct Code" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid Request. Please try again later" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Invalid Request. Please First go to Sign Up to provide your phone number." });
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while registering Phone number. please try again later." });
            }
        }


        // DELETE: api/UserSignUpCodes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserSignUpCode>> DeleteUserSignUpCode(int id)
        {
            var userSignUpCode = await con.userSignUps.FindAsync(id);
            if (userSignUpCode == null)
            {
                return NotFound();
            }

            con.userSignUps.Remove(userSignUpCode);
            await con.SaveChangesAsync();

            return userSignUpCode;
        }

        private bool UserSignUpCodeExists(int id)
        {
            return con.userSignUps.Any(e => e.uscId == id);
        }

        private bool UserSignUpCodePhoneNoExists(string PhoneNo)
        {
            return con.userSignUps.Any(e => e.usPhone == PhoneNo);
        }

        private bool UserSignUpCodePhoneNoExistWithStatus(string PhoneNo, string Status)
        {
            return con.userSignUps.Any(e => e.usPhone == PhoneNo && e.uscStatus == Status);
        }

        [HttpGet("ResendCode")]
        public async Task<ActionResult> SignUpResendCode(string PhoneNo)
        {
            try
            {
                PhoneNo = PhoneNo.Trim();
                var ChkNo = PhoneNo.Substring(0, 1);
                if (ChkNo != "+")
                {
                    PhoneNo = "+" + PhoneNo;
                }
                var getData = await con.userSignUps.Where(u => u.usPhone == PhoneNo).FirstOrDefaultAsync();
                if (getData != null)
                {
                    getData.uscCode = Convert.ToInt32(RandomString(6));
                    getData.uscExpire = DateTime.Now.AddMinutes(10);
                    con.Entry(getData).State = EntityState.Modified;
                    try
                    {
                        var msgboday = "" + getData.uscCode.ToString() + " is your Universal Skills verifcation code.";
                        var Code = PhoneNo.Substring(0, 3);
                        msg.SendSMSTurab(getData.usPhone, msgboday);
                        //if (Code == "+92")
                        //{
                        //    msg.SendSMSTurabPak(getData.usPhone, msgboday);
                        //}
                        //else
                        //{
                        //    msg.SendSMSTurab(getData.usPhone, msgboday);
                        //}
                        await con.SaveChangesAsync();
                        return Ok();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return BadRequest(new { message = "An error occured. Request not updated" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Phone Number not found. Request not completed please try gain later " + PhoneNo + " " + getData + "" });
                }

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured. Please try again later" });
            }

        }

        [HttpPost("GetPhoneNoWithCountryCode")]
        public async Task<ActionResult<UserSignUpCode>> GetPhoneNoWithCountryCode(PhoneNumber number)
        {
            try
            {
                if (number.Code != null && number.Num != null)
                {
                    var getData = con.countries.Where(c => c.CountryCode == number.Code).FirstOrDefault();
                    if (getData != null)
                    {
                        number.Num = number.Num.Trim();
                        var Num = number.Num;
                        if (Num.Length >= getData.NumLength)
                        {
                            string getDigit = Num.Substring(Num.Length - getData.NumLength);
                            string getFirst = getDigit.Substring(0, 1);
                            if (Convert.ToInt32(getFirst) == getData.NumStart)
                            {
                                var PhoneNo = number.Code + getDigit;
                                UserSignUpCode userSignUpCode = new UserSignUpCode();
                                userSignUpCode.usPhone = PhoneNo;
                                userSignUpCode.usPhone = userSignUpCode.usPhone.Trim();
                                var ChkData = await con.userSignUps.Where(e => e.usPhone == userSignUpCode.usPhone).FirstOrDefaultAsync();
                                if (ChkData != null)
                                {
                                    return Ok(ChkData);
                                }
                                else
                                {
                                    return BadRequest(new { message = "Phone number not found.Please try again." });
                                }

                            }
                            else
                            {
                                return BadRequest(new { message = "Invalid Phone Number. Phone Number of '" + getData.CountryName + "' must be start with '" + getData.NumStart + "' digit" });

                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Not a valid Phone Number.Minimum required length is " + getData.NumLength + " digit of " + getData.CountryName + " number. " });
                        }

                    }
                    else
                    {
                        return BadRequest(new { message = "No Country Code found. Please try again later" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Number and Code not found.Please try again." });
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while registering Phone number. please try again later." });
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
