using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasySystemAPI.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Options;

namespace EasySystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class UsersController : ControllerBase
    {
        private readonly EasyContext con;
        private readonly IOptions<SkillsFee> GetAmount;
        SendMessage msg = new SendMessage();

        private readonly IHostingEnvironment HostingEnvironment;

        public UsersController(EasyContext context, IHostingEnvironment hostingEnvironment, IOptions<SkillsFee> Amount)
        {
            con = context;
            this.HostingEnvironment = hostingEnvironment;
            GetAmount = Amount;
        }


        [HttpGet("RegisteredUsers")]
        public async Task<ActionResult> RegisteredUsers()
        {
            try
            {
                var getData = await (from s in con.users
                                     join c in con.countries on s.CountryId equals c.CountryId
                                     join m in con.users on s.refId equals m.usrId
                                     select new
                                     {
                                         s.usrId,
                                         s.usrName,
                                         s.usrEmail,
                                         s.usrPhone,
                                         s.usrGender,
                                         s.usrCreated,
                                         s.usrStatus,
                                         s.usrCode,
                                         s.usrImage,
                                         s.usrCity,
                                         s.memId,
                                         s.usrPassword,
                                         s.ExpiryDate,
                                         c.CountryName,
                                         Mentor = m.usrName,
                                     }).OrderByDescending(o => o.usrId).ToListAsync();
                return Ok(getData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "There is some error please try again later" });
            }
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> Getusers()
        {
            return await con.users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            var users = await con.users.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult<Users>> GetUser(int id)
        {
            var users = await con.users.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }


        [HttpGet("GetCode")]
        public async Task<ActionResult<Users>> GetCode(int Code)
        {
            var users = await con.users.Where(u => u.usrCode == Code).FirstOrDefaultAsync();

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            if (id != users.usrId)
            {
                return BadRequest();
            }

            con.Entry(users).State = EntityState.Modified;

            try
            {
                await con.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<Users>> PostUsers(RegisterViewModel users)
        {
            try
            {
                if (users.usrStatus == "Register")
                {
                    var ChkData = UserPhoneNoExists(users.usrPhone);
                    if (ChkData == true)
                    {
                        return BadRequest(new { message = "This Phone Number is already exist please choose a different one" });
                    }
                    else
                    {
                        try
                        {
                            //Activate the Status in UserSignUpCode Table
                            var getUsrSignUp = await con.userSignUps.Where(c => c.usPhone == users.usrPhone).FirstOrDefaultAsync();
                            if (getUsrSignUp != null)
                            {
                                getUsrSignUp.uscStatus = "Active";
                                try
                                {
                                    con.Entry(getUsrSignUp).State = EntityState.Modified;
                                    await con.SaveChangesAsync();
                                    //return Ok();
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return BadRequest(new { message = "An error occured while Updating the user Status. Please try again later" });
                                }
                            }
                            //End Activate the Status in UserSignUpCode Table//

                            //string uploadFolder = "";
                            //string fileName = "";
                            //string filePath = "";
                            Users usr = new Users();
                            // for Reference
                            if (usr.usrCode == 0)
                            {
                                usr.refId = 1;
                            }
                            else
                            {
                                var getRefId = await con.users.Where(u => u.usrCode == usr.usrCode).FirstOrDefaultAsync();
                                if (getRefId != null)
                                {
                                    usr.refId = getRefId.usrId;
                                }
                                else
                                {
                                    usr.refId = 1;
                                }
                            }
                            foreach (var i in RandomString(6))
                            {
                                var Code = RandomString(6);
                                var chkCode = CheckUsrCode(Code);
                                if (chkCode == false)
                                {
                                    usr.usrCode = Convert.ToInt32(Code);
                                    break;
                                }
                            }
                            usr.roleId = 3;
                            usr.usrEmail = users.usrEmail;
                            usr.usrName = users.usrName;
                            usr.usrPhone = users.usrPhone;
                            usr.usrGender = users.usrGender;
                            usr.usrPassword = users.usrPassword;
                            usr.usrCreated = DateTime.Now;
                            usr.usrStatus = "Learning";
                            usr.CountryId = 1;
                            usr.usrCity = "Dubai";
                            usr.memId = 1;
                            usr.ExpiryDate = DateTime.Now.AddYears(-100);
                            con.users.Add(usr);
                            try
                            {
                                await con.SaveChangesAsync();
                                return Ok(users);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return BadRequest(new { message = "An error occured while adding the user. Please try again later" });
                            }
                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        {
                            return BadRequest(new { message = "User Not registered. " });
                        }
                    }
                }
                else if (users.usrStatus == "Login")
                {
                    var ChkData = await con.users.Where(u => u.usrPhone == users.usrPhone && u.usrPassword == users.usrPassword).FirstOrDefaultAsync();
                    if (ChkData != null)
                    {
                        return Ok(ChkData);
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid Phone number or password" });
                    }
                }
                else if (users.usrStatus == "Reset")
                {
                    var ChkData = await con.users.Where(u => u.usrPhone == users.usrPhone).FirstOrDefaultAsync();
                    if (ChkData != null)
                    {
                        ChkData.usrPassword = users.usrPassword;
                        con.Entry(ChkData).State = EntityState.Modified;
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
                        return BadRequest(new { message = "Invalid Phone number" });
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured. Request not Completed" });
            }
            return BadRequest(new { message = "Not Added. Request not Completed" });
            // return CreatedAtAction("GetUsers", new { id = users.usrId }, users);
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult<Users>> RegisterUser(RegisterViewModel users)
        {
            try
            {
                users.usrName = users.usrName.Trim();
                users.usrEmail = users.usrEmail.Trim();
                users.usrPassword = users.usrPassword.Trim();
                users.usrPhone = users.usrPhone.Trim();
                var ChkEmail = UserEmailExists(users.usrEmail);
                if (ChkEmail == true)
                {
                    return BadRequest(new { message = "This Email is already exist please choose a different one" });
                }
                else
                {
                    var ChkData = UserPhoneNoExists(users.usrPhone);
                    if (ChkData == true)
                    {
                        return BadRequest(new { message = "This Phone Number is already exist please choose a different one" });
                    }
                    else
                    {
                        //Activate the Status in UserSignUpCode Table
                        var getUsrSignUp = await con.userSignUps.Where(c => c.usPhone == users.usrPhone).FirstOrDefaultAsync();
                        if (getUsrSignUp != null)
                        {
                            if (getUsrSignUp.uscStatus == "Verified")
                            {
                                getUsrSignUp.usEmail = users.usrEmail;
                                getUsrSignUp.uscStatus = "Active";
                                try
                                {
                                    con.Entry(getUsrSignUp).State = EntityState.Modified;
                                    await con.SaveChangesAsync();
                                    //return Ok();
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return BadRequest(new { message = "An error occured while Updating the userSignUp Status. Please try again later" });
                                }
                                //End Activate the Status in UserSignUpCode Table//

                                Users usr = new Users();
                                Users getRefId = new Users();
                                if (users.usrCode == 0)
                                {
                                    usr.usrCode = 123456;
                                    getRefId = await con.users.Where(u => u.usrCode == 123456).FirstOrDefaultAsync();
                                }
                                else
                                {
                                    getRefId = await con.users.Where(u => u.usrCode == users.usrCode).FirstOrDefaultAsync();
                                }

                                // for Mentor
                                if (getRefId != null)
                                {
                                    usr.refId = getRefId.usrId;
                                }
                                else
                                {
                                    usr.refId = 1;
                                }

                                foreach (var i in RandomString(6))
                                {
                                    var Code = RandomString(6);
                                    var chkCode = CheckUsrCode(Code);
                                    if (chkCode == false)
                                    {
                                        usr.usrCode = Convert.ToInt32(Code);
                                        break;
                                    }
                                }
                                usr.roleId = 3;
                                usr.usrEmail = users.usrEmail;
                                usr.usrName = users.usrName;
                                usr.usrPhone = users.usrPhone;
                                usr.usrGender = users.usrGender;
                                var ePass = PasswordEncryption.Encrypts(users.usrPassword);
                                usr.usrPassword = ePass;
                                usr.usrCreated = DateTime.Now;
                                usr.usrStatus = "Learning";
                                usr.usrImage = users.usrImage;
                                usr.CountryId = users.CountryId;
                                usr.usrCity = "Dubai";
                                usr.memId = 1;
                                usr.ExpiryDate = DateTime.Now.AddYears(-100);
                                con.users.Add(usr);
                                try
                                {
                                    await con.SaveChangesAsync();

                                    //Mail to Mentor
                                    string emailBody = "<b>Congratulations " + getRefId.usrName + " </b><br/>A Trainee <b>" + usr.usrName + "-" + usr.usrCode + "</b> has been registered with you by using Universal Skills System to Learn Expert Skills on Dated <b>" + DateTime.Now + "</b>.<br/><br/><br/><br/>" + GetAmount.Value.MailMessage + "";
                                    Mailer email = new Mailer("Care@Universalskills.co", getRefId.usrEmail, "Congratulations", emailBody);
                                    email.Send();
                                    InfoSend info = new InfoSend();
                                    info.infoText = emailBody;
                                    info.infoBy = "Email";
                                    info.infoTo = getRefId.usrId;
                                    info.infoDate = DateTime.Now;
                                    info.infoStatus = "Success";
                                    con.infoSends.Add(info);

                                    //Mail to New User
                                    string emailBody1 = "<b>Congratulations " + usr.usrName + "-" + usr.usrCode + " </b><br/>You have been registered at <b>Universal Skills</b> on Dated <b>" + DateTime.Now + "</b> and your mentor is <b>" + getRefId.usrName + "-" + getRefId.usrCode + "</b>.<br/><br/><br/><br/>" + GetAmount.Value.MailMessage + "";
                                    Mailer email1 = new Mailer("Care@Universalskills.co", usr.usrEmail, "Congratulations", emailBody1);
                                    email1.Send();
                                    InfoSend info1 = new InfoSend();
                                    info1.infoText = emailBody1;
                                    info1.infoBy = "Email";
                                    info1.infoTo = usr.usrId;
                                    info1.infoDate = DateTime.Now;
                                    info1.infoStatus = "Success";
                                    con.infoSends.Add(info1);



                                    //Mail for Company
                                    //string emailBodyCompany = "<b>" + usr.usrName + "-" + usr.usrCode + " </b>has been registered at <b>Universal Skills</b> on Dated <b>" + DateTime.Now + "</b> under the mentor <b>" + getRefId.usrName + "-" + getRefId.usrCode + "</b>." +
                                    //    "The mentor <b>" + getRefId.usrName + "-" + getRefId.usrCode + "</b> will get "+GetAmount.Value.TuitionFee+ "pkr/- as Tuition Fee from <b>Universal Skills</b>";
                                    //Mailer emailCompany = new Mailer("Care@Universalskills.co", GetAmount.Value.CompanyMail, "Transaction Completed", emailBodyCompany);
                                    //emailCompany.Send();
                                    string emailBodyCompany = "";
                                    if(usr.CountryId == 6)
                                    {
                                        emailBodyCompany = "<b>" + usr.usrName + "-" + usr.usrCode + " </b>has been registered at <b>Universal Skills</b> on Dated <b>" + DateTime.Now + "</b> under the mentor <b>" + getRefId.usrName + "-" + getRefId.usrCode + "</b>." +
                                           "The mentor <b>" + getRefId.usrName + "-" + getRefId.usrCode + "</b> will get " + GetAmount.Value.TuitionFee + "pkr/- as Tuition Fee from <b>Universal Skills</b>";
                                    }
                                    else
                                    {
                                        emailBodyCompany = "<b>" + usr.usrName + "-" + usr.usrCode + " </b>has been registered at <b>Universal Skills</b> on Dated <b>" + DateTime.Now + "</b> under the mentor <b>" + getRefId.usrName + "-" + getRefId.usrCode + "</b>." +
                                        "The mentor <b>" + getRefId.usrName + "-" + getRefId.usrCode + "</b> will get " + GetAmount.Value.TuitionFee + " USD as Tuition Fee from <b>Universal Skills</b>";
                                    }
                                    InfoSend_Company cinfo = new InfoSend_Company();
                                    cinfo.cInfoText = emailBodyCompany;
                                    cinfo.cInfoBy = "Email";
                                    cinfo.cInfoTo = usr.usrId;
                                    cinfo.cInfoDate = DateTime.Now;
                                    cinfo.cInfoStatus = "Success";
                                    con.infoSend_Companies.Add(cinfo);

                                    await con.SaveChangesAsync();
                                    return Ok(usr);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return BadRequest(new { message = "An error occured while adding the user. Please try again later" });
                                }
                                //Add the User Data
                            }
                            else if (getUsrSignUp.uscStatus == "Pending")
                            {
                                return BadRequest(new { message = "Your Phone number is not Verified. Please Verify the Phone Number to Register yourself." });
                            }
                            else
                            {
                                return BadRequest(new { message = "Invalid Request. Please try again." });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "This Phone number is not exist. Please First Verify the Phone number to Register yourself" });
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while registering the User. please try again later. " + ex + "" });
            }
        }

        [HttpPost("ForgetPassword")]
        public async Task<ActionResult<Users>> ForgetPassword(PhoneNumber number)
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
                                Users usr = new Users();
                                usr.usrPhone = PhoneNo;
                                usr.usrPhone = usr.usrPhone.Trim();
                                var ChkData = UserPhoneNoExists(usr.usrPhone);
                                if (ChkData == false)
                                {
                                    return BadRequest(new { message = "Invalid Phone Number or this Phone Number is not registered." });
                                }
                                else
                                {
                                    var chkData = await con.userSignUps.Where(x => x.usPhone == usr.usrPhone).FirstOrDefaultAsync();
                                    if (chkData != null)
                                    {
                                        if (chkData.uscStatus == "Active" || chkData.uscStatus == "Reset")
                                        {
                                            chkData.uscCode = Convert.ToInt32(RandomString(6));
                                            chkData.uscStatus = "Reset";
                                            chkData.uscExpire = DateTime.Now.AddMinutes(10);
                                            con.Entry(chkData).State = EntityState.Modified;
                                            try
                                            {
                                                var msgboday = "" + chkData.uscCode.ToString() + " is your Universal Skills verification code";
                                                msg.SendSMSTurab(usr.usrPhone, msgboday);
                                                //if (number.Code == "+92")
                                                //{
                                                //    msg.SendSMSTurabPak(usr.usrPhone, msgboday);
                                                //}
                                                //else
                                                //{
                                                //    msg.SendSMSTurab(usr.usrPhone, msgboday);
                                                //}

                                                await con.SaveChangesAsync();
                                                return Ok(chkData);
                                            }
                                            catch (DbUpdateConcurrencyException)
                                            {
                                                return BadRequest(new { message = "An error occured. Request not updated" });
                                            }
                                        }
                                        else
                                        {
                                            return BadRequest(new { message = "Invalid Request. Request not Completed" });
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest(new { message = "Invalid Phone Number. Please enter the correct Phone Number" });
                                    }
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
                return BadRequest(new { message = "An error occured. Request not completed" });
            }
        }

        [HttpPost("VerifyForgetPasswordCode")]
        public async Task<ActionResult<UserSignUpCode>> VerifyForgetPasswordCode(UserSignUpCode usr)
        {
            try
            {
                var ChkData = UserPhoneNoExists(usr.usPhone);
                if (ChkData == false)
                {
                    return BadRequest(new { message = "Invalid Phone Number or this Phone Number is not registered." });
                }
                else
                {
                    var chkData = await con.userSignUps.Where(x => x.usPhone == usr.usPhone).FirstOrDefaultAsync();
                    if (chkData != null)
                    {
                        if (chkData.uscStatus == "Reset")
                        {
                            if (chkData.uscCode == usr.uscCode)
                            {
                                if (chkData.uscExpire >= DateTime.Now)
                                {
                                    chkData.uscStatus = "Active";
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
                                    return BadRequest(new { message = "This code is expired. Please wait Click on resend code to get the new ONE." });
                                }
                            }
                            else
                            {
                                return BadRequest(new { message = "Invalid Code. Please Enter the correct Code" });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Invalid Request. Request not Completed" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid Phone Number. Please enter the correct Phone Number" });
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured. Request not completed" });
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult<Users>> ResetPassword(Users usr)
        {
            try
            {
                usr.usrPhone = usr.usrPhone.Trim();
                usr.usrPassword = usr.usrPassword.Trim();
                var ChkData = await con.users.Where(u => u.usrPhone == usr.usrPhone).FirstOrDefaultAsync();
                if (ChkData == null)
                {
                    return BadRequest(new { message = "Invalid Phone Number or this Phone Number is not registered." });
                }
                else
                {
                    var chkData = await con.userSignUps.Where(x => x.usPhone == usr.usrPhone).FirstOrDefaultAsync();
                    if (chkData != null)
                    {
                        if (chkData.uscStatus == "Active")
                        {
                            var ePass = PasswordEncryption.Encrypts(usr.usrPassword);
                            ChkData.usrPassword = ePass;
                            con.Entry(ChkData).State = EntityState.Modified;
                            try
                            {
                                await con.SaveChangesAsync();
                                return Ok();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return BadRequest(new { message = "An error occured. Password not updated" });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Invalid Request. Request not Completed" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid Phone Number. Please enter the correct Phone Number" });
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured. Request not completed" });
            }
        }

        [HttpPost("UserLogin")]
        public async Task<ActionResult<Users>> UserLogin(PhoneNumber number)
        {
            try
            {
                //Encrypt Current Database Password
                //var getU = con.users.ToList();
                //if (getU.Count > 0)
                //{
                //    foreach (var i in getU)
                //    {
                //        var getUsers = con.users.Where(u => u.usrId == i.usrId).FirstOrDefault();
                //        getUsers.usrPassword = PasswordEncryption.Encrypts(getUsers.usrPassword);
                //        con.Entry(i).State = EntityState.Modified;
                //    }
                //    con.SaveChanges();
                //}


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
                                var ePass = PasswordEncryption.Encrypts(number.Password);
                                var ChkData = await con.users.Where(u => u.usrPhone == PhoneNo && u.usrPassword == ePass).FirstOrDefaultAsync();
                                if (ChkData != null)
                                {
                                    return Ok(ChkData);
                                }
                                else
                                {
                                    return BadRequest(new { message = "Invalid Phone number or password" });
                                    //return BadRequest(new { message = "Invalid Email or password" });
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
            catch (Exception)
            {
                return BadRequest(new { message = "An error occured. Request not completed." });
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Users>> DeleteUsers(int id)
        {
            var users = await con.users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            con.users.Remove(users);
            await con.SaveChangesAsync();

            return users;
        }

        [HttpGet("GetRefData")]
        public async Task<ActionResult<Users>> GetRefData(int id)
        {
            var users = await con.users.Where(u => u.usrId == id).FirstOrDefaultAsync();

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        [HttpPost("ChangeUserPassword")]
        public async Task<ActionResult> ChangeUserPassword(ChangePassword pass)
        {
            try
            {
                var getUser = await con.users.Where(u => u.usrId == pass.Id).FirstOrDefaultAsync();
                if (getUser != null)
                {
                    var ECPass = PasswordEncryption.Encrypts(pass.cPassword);
                    if (getUser.usrPassword == ECPass)
                    {
                        if (pass.nPassword == pass.rPassword)
                        {
                            var ePass = PasswordEncryption.Encrypts(pass.nPassword);
                            getUser.usrPassword = ePass;
                            con.Entry(getUser).State = EntityState.Modified;
                            try
                            {
                                await con.SaveChangesAsync();
                                return Ok();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return BadRequest(new { message = "An error occured while updating password. Please try again later" });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "New Password and Re-Type Must matched" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Incorrect Current Password.Please Enter correct password." });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Invalid Password" });

                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "There is some error to update the Password. Please try again later" });
            }
        }


        [HttpPost("ChangeUserInfo")]
        public async Task<ActionResult> ChangeUserInfo(Users usr)
        {
            try
            {
                var getUser = await con.users.Where(u => u.usrId == usr.usrId).FirstOrDefaultAsync();
                if (getUser != null)
                {
                    if (usr.usrGender != null)
                    {
                        getUser.usrGender = usr.usrGender;
                    }
                    if (usr.usrName != null)
                    {
                        getUser.usrName = usr.usrName;
                    }

                    if (usr.usrImage != null)
                    {
                        getUser.usrImage = usr.usrImage;
                    }
                    con.Entry(getUser).State = EntityState.Modified;
                    try
                    {
                        await con.SaveChangesAsync();
                        return Ok();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return BadRequest(new { message = "An error occured while updating user Info. Please try again later" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "User Id not found" });

                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "There is some error to update the user info. Please try again later" });
            }
        }



        [HttpGet("GetCountryId")]
        public async Task<ActionResult> GetCountryId(string code)
        {
            if (code != null)
            {
                var Code = "+" + code;
                var country = await con.countries.Where(c => c.CountryCode == Code).FirstOrDefaultAsync();
                if (country == null)
                {
                    return BadRequest(new { message = "No Record found" });
                }
                else
                {
                    return Ok(country);
                }
            }
            else
            {
                return BadRequest(new { message = "Code not found" });
            }


        }

        [HttpGet("GetCountryName")]
        public async Task<ActionResult> GetCountryName(int id)
        {
            var country = await con.countries.Where(c => c.CountryId == id).FirstOrDefaultAsync();
            if (country == null)
            {
                return BadRequest(new { message = "No Record found" });
            }
            else
            {
                return Ok(country.CountryName);
            }
        }

        [HttpGet("CheckVerification")]
        public async Task<ActionResult> CheckVerification(int id)
        {
            var getTypes = await con.userVerificationTypes.ToListAsync();
            if (getTypes.Count > 0)
            {
                foreach (var i in getTypes)
                {
                    var chckData = await con.userVerifications.Where(us => us.usrId == id && us.uvtId == i.uvtId).FirstOrDefaultAsync();
                    if (chckData != null)
                    {
                        if (chckData.uvStatus == "Verified")
                        {

                        }
                        else
                        {
                            return BadRequest(new { message = "Not verified" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Not verified" });
                    }
                }
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Not verified" });
            }

        }

        [HttpGet("GetMyAllMentor")]
        public async Task<ActionResult> GetMyAllMentor(int id)
        {
            try
            {
                var getData = await (from u in con.users
                                     join cm in con.users on u.refId equals cm.usrId
                                     join om in con.UserMentors on u.usrId equals om.mentId
                                     join m in con.users on om.mentId equals m.usrId
                                     where om.usrId == id
                                     select new
                                     {
                                         CoreMentor = cm.usrName + "-" + cm.usrCode,
                                         MentorId = om.mentId,
                                         MentorName = m.usrName + "-" + m.usrCode,
                                         JoiningDate = om.createdDate,
                                         LeftDate = om.deletedDate,
                                         Status = om.umStatus
                                     }).OrderByDescending(o => o.Status == true).OrderByDescending(o => o.JoiningDate).ToListAsync();
                return Ok(getData);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "An error occured getting the request. Please try again later" });
            }

        }

        [HttpPost("MakeMentor")]
        public async Task<ActionResult> MakeMentor(UserMentors data)
        {
            try
            {
                var chkData = await con.UserMentors.Where(m => m.usrId == data.usrId && m.mentId == data.mentId).FirstOrDefaultAsync();
                if (chkData == null)
                {
                    data.umStatus = true;
                    data.createdDate = DateTime.Now;
                    data.deletedDate = DateTime.Now.AddYears(-100);
                    con.UserMentors.Add(data);
                    await con.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    chkData.umStatus = true;
                    chkData.deletedDate = DateTime.Now.AddYears(-100);
                    con.Entry(chkData).State = EntityState.Modified;
                    await con.SaveChangesAsync();
                    return Ok();
                }

            }
            catch (Exception)
            {
                return BadRequest(new { message = "Please try again later" });
            }
        }

        [HttpPost("LeaveMentor")]
        public async Task<ActionResult> LeaveMentor(UserMentors data)
        {
            try
            {
                var chkData = await con.UserMentors.Where(m => m.usrId == data.usrId && m.mentId == data.mentId).FirstOrDefaultAsync();
                if (chkData != null)
                {
                    chkData.umStatus = false;
                    chkData.deletedDate = DateTime.Now;
                    con.Entry(chkData).State = EntityState.Modified;
                    await con.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = "No data updated" });
                }

            }
            catch (Exception)
            {
                return BadRequest(new { message = "Please try again later" });
            }
        }

        [HttpPost("ChangeCoreMentor")]
        public async Task<ActionResult> ChangeCoreMentor(UserMentors data)
        {
            try
            {
                var chkData = await con.UserMentors.Where(m => m.usrId == data.usrId && m.mentId == data.mentId).FirstOrDefaultAsync();
                if (chkData != null)
                {
                    con.UserMentors.Remove(chkData);
                    await con.SaveChangesAsync();
                }

                var getUsr = await con.users.Where(u => u.usrId == data.usrId).FirstOrDefaultAsync();
                int OldMentor = getUsr.refId;
                if (getUsr != null)
                {
                    getUsr.refId = data.mentId;
                    con.Entry(getUsr).State = EntityState.Modified;
                    await con.SaveChangesAsync();

                    var getOldMen = await con.users.Where(u => u.usrId == OldMentor).FirstOrDefaultAsync();

                    if (getOldMen != null)
                    {
                        var getNewMen = await con.users.Where(u => u.usrId == data.mentId).FirstOrDefaultAsync();
                        if (getNewMen != null)
                        {
                            //Mail to User
                            string emailBody = "<b>Dear " + getUsr.usrName + " </b><br/>" +
                                "You have changed your core mentor on dated <b>" + DateTime.Now + "</b>. Your old mentor was <b>" + getOldMen.usrName + "-" + getOldMen.usrCode + "</b>, now your new mentor is <b>" + getNewMen.usrName + "-" + getNewMen.usrCode + "</b>. We hope you will learn more with your new mentor. Best of luck.<br/><br/><br/><br/>" + GetAmount.Value.MailMessage + "";
                            Mailer email = new Mailer("Care@Universalskills.co", getUsr.usrEmail, "Mentor Changed", emailBody);
                            email.Send();
                            InfoSend info = new InfoSend();
                            info.infoText = emailBody;
                            info.infoBy = "Email";
                            info.infoTo = getUsr.usrId;
                            info.infoDate = DateTime.Now;
                            info.infoStatus = "Success";
                            con.infoSends.Add(info);

                            //Mail to OldMentor
                            string emailBody1 = "<b>Dear  " + getOldMen.usrName + " </b><br/>" +
                                "Your trainee <b>" + getUsr.usrName + " - " + getUsr.usrCode + "</b> has changed mentor on dated <b>" + DateTime.Now + "</b>, this trainee does not belong to you anymore. Do not worry, hope for the best for future. We wish you work so hard and never lose your trainee ever. <br/><br/><br/><br/>" + GetAmount.Value.MailMessage + "";
                            Mailer email1 = new Mailer("Care@Universalskills.co", getOldMen.usrEmail, "Lost Trainee", emailBody1);
                            email1.Send();
                            InfoSend info1 = new InfoSend();
                            info1.infoText = emailBody1;
                            info1.infoBy = "Email";
                            info1.infoTo = getOldMen.usrId;
                            info1.infoDate = DateTime.Now;
                            info1.infoStatus = "Success";
                            con.infoSends.Add(info1);


                            //Mail to NewMentor
                            string emailBody2 = "<b>Congratulations  " + getNewMen.usrName + " </b><br/>" +
                                "A Trainee <b>" + getUsr.usrName + " - " + getUsr.usrCode + "</b> has been made you new mentor by using Universal Skills System to Learn Expert Skills on Dated <b>" + DateTime.Now + "</b>. <br/><br/><br/><br/>" + GetAmount.Value.MailMessage + "";
                            Mailer email2 = new Mailer("Care@Universalskills.co", getNewMen.usrEmail, "Congratulations", emailBody2);
                            email2.Send();
                            InfoSend info2 = new InfoSend();
                            info2.infoText = emailBody2;
                            info2.infoBy = "Email";
                            info2.infoTo = getNewMen.usrId;
                            info2.infoDate = DateTime.Now;
                            info2.infoStatus = "Success";
                            con.infoSends.Add(info2);


                            UserLostTrainee ult = new UserLostTrainee();
                            ult.usrIdM = getOldMen.usrId;
                            ult.usrIdC = getUsr.usrId;
                            ult.ultDate = DateTime.Now;
                            //var Clickhere = "https://universalskills.co/Home/TermsAndConditions";
                            ult.ultReason = "Your trainee <b>" + getUsr.usrName + " - " + getUsr.usrCode + "</b> has changed mentor on dated <b>" + DateTime.Now + "</b>, this trainee does not belong to you anymore. Do not worry, hope for the best for future. We wish you work so hard and never lose your trainee ever. < br/><br/><br/><br/>" + GetAmount.Value.MailMessage + "";
                            ult.ultStatus = "Lost";
                            con.userLostTrainees.Add(ult);


                            await con.SaveChangesAsync();

                            return Ok();

                        }
                    }
                }
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Please try again later" });
            }
        }

        [HttpGet("GetMyOtherTrainees")]
        public async Task<ActionResult> GetMyOtherTrainees(int id)
        {
            try
            {
                var getData = await (from u in con.users
                                     join om in con.UserMentors on u.usrId equals om.usrId
                                     where om.mentId == id
                                     select new
                                     {
                                         MentorId = om.usrId,
                                         MentorName = u.usrName + "-" + u.usrCode,
                                         JoiningDate = om.createdDate,
                                         LeftDate = om.deletedDate,
                                         Status = om.umStatus
                                     }).OrderByDescending(o => o.Status == true).OrderByDescending(o => o.JoiningDate).ToListAsync();
                return Ok(getData);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "An error occured getting the request. Please try again later" });
            }

        }

        [HttpGet("CatchTheTrainee")]
        public async Task<ActionResult<IEnumerable<Users>>> CatchTheTrainee(int id)
        {
            return await con.users.Where(u => u.CountryId == id && u.usrCreated.Date == DateTime.Now.Date && u.refId == 1 && u.usrStatus == "Learning").ToListAsync();
        }

        [HttpPost("AddLoginLogs")]
        public async Task<ActionResult> AddLoginLogs(Users usr)
        {
            try
            {
                var chkLog = await con.loginLogs.Where(u => u.UsrId == usr.usrId).FirstOrDefaultAsync();
                if(chkLog != null)
                {
                    chkLog.LogDateTime = DateTime.Now;
                    con.Entry(chkLog).State = EntityState.Modified;
                    await con.SaveChangesAsync();
                }
                else
                {
                    LoginLogs log = new LoginLogs();
                    log.UsrId = usr.usrId;
                    log.MentorId = usr.refId;
                    log.LogDateTime = DateTime.Now;
                    con.loginLogs.Add(log);
                    await con.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

            }
            return Ok();
        }

        [HttpGet("RecentLoginLogs")]
        public async Task<ActionResult> RecentLoginLogs()
        {
            try
            {
                var getUsrs = await con.users.ToListAsync();
                if(getUsrs.Count > 0)
                {
                    List<LoginLogs> lList = new List<LoginLogs>();
                    foreach(var i in getUsrs)
                    {
                        var chkLog = await con.loginLogs.Where(u => u.UsrId == i.usrId).AnyAsync();
                        if(chkLog == false)
                        {
                            LoginLogs log = new LoginLogs();
                            log.UsrId = i.usrId;
                            log.MentorId = i.refId;
                            log.LogDateTime = i.usrCreated;
                            lList.Add(log);
                        }
                    }
                    con.loginLogs.AddRange(lList);
                    await con.SaveChangesAsync();
                    return Ok();
                }
                
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error occured"});
            }
            return Ok();
        }

        //[HttpGet("GetLogs")]
        //public async Task<ActionResult<List<LoginLogs>>> GetLogs(int id)
        //{
        //    return await con.loginLogs.Where(u => u.MentorId == id).ToListAsync();
        //}

        [HttpGet("GetTeam")]
        public async Task<ActionResult> GetTeam(int id)
        {
            var getData =  await (from s in con.users
                           join l in con.loginLogs on s.usrId equals l.UsrId
                           where s.refId == id
                           select new
                           {
                               s.usrId,
                               s.refId,
                               s.usrName,
                               s.usrPhone,
                               s.usrCreated,
                               s.ExpiryDate,
                               s.usrGender,
                               s.usrStatus,
                               l.LogDateTime
                            }).OrderByDescending(o => o.usrId).ToListAsync();

            return Ok(getData);
        }

        private bool UsersExists(int id)
        {
            return con.users.Any(e => e.usrId == id);
        }

        private bool UserPhoneNoExists(string PhoneNo)
        {
            return con.users.Any(e => e.usrPhone == PhoneNo);
        }

        private bool UserEmailExists(string email)
        {
            return con.users.Any(e => e.usrEmail == email);
        }

        private bool CheckUsrCode(string Code)
        {
            return con.users.Any(e => e.usrCode == Convert.ToInt32(Code));
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
