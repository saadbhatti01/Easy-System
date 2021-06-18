using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EasySystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransectionController : ControllerBase
    {
        private readonly EasyContext con;
        private readonly IOptions<SkillsFee> GetAmount;

        public TransectionController(EasyContext context, IOptions<SkillsFee> Amount)
        {
            con = context;
            GetAmount = Amount;
        }

        [HttpPost("SignUpTransaction")]
        public async Task<ActionResult> SignUpTransaction(Transections trn)
        {
            try
            {
                trn.tDate = DateTime.Now;
                trn.tDateTime = DateTime.Now;
                trn.TuitionAmount = GetAmount.Value.SignUpAmount;
                trn.SoftServiceCharges = 0;
                trn.ThirdPartyCharges = 0;
                trn.tPaying = 1;
                trn.tStatus = "Verified";
                con.transections.Add(trn);
                await con.SaveChangesAsync();
                //var getUser = await con.users.Where(u => u.usrId == trn.tReceiving).FirstOrDefaultAsync();

                var getUser = await (from s in con.users
                                     join m in con.users on s.refId equals m.usrId
                                     where s.usrId == trn.tReceiving
                                     select new
                                     {
                                         s.usrId,
                                         s.usrName,
                                         s.usrEmail,
                                         s.usrCode,
                                         Mentor = m.usrName,
                                         MentorCode = m.usrCode
                                     }).FirstOrDefaultAsync();

                var msg = "";
                if (getUser != null)
                {
                    string emailBody1 = "<b>Congratulations " + getUser.usrName + "</b> you have received <b>" + GetAmount.Value.SignUpAmount + "/- pkr</b> on Sign Up with us on Dated <b>" + DateTime.Now + "</b> and your mentor is <b>" + getUser.Mentor + "-" + getUser.MentorCode + "</b>.<br/><br/><br/><br/>" + GetAmount.Value.MailMessage + "";
                    Mailer email1 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Congratulations", emailBody1);
                    email1.Send();

                    InfoSend info1 = new InfoSend();
                    info1.infoText = emailBody1;
                    info1.infoBy = "Email";
                    info1.infoTo = getUser.usrId;
                    info1.infoDate = DateTime.Now;
                    info1.infoStatus = "Success";
                    con.infoSends.Add(info1);
                    await con.SaveChangesAsync();


                    msg = emailBody1;
                }

                return Ok(new { message = "" + msg + "" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Data not added." });
            }
        }

        [HttpPost("AddTransection")]
        public async Task<ActionResult<Transections>> AddTransection(Transections trn)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Not a valid Parameters.Please try again" });
                }
                else
                {
                    double totalAmount = trn.ThirdPartyCharges + trn.SoftServiceCharges + trn.TuitionAmount;
                    var getMentorData = await con.users.Where(u => u.usrId.Equals(trn.tReceiving)).FirstOrDefaultAsync();
                    if (getMentorData != null)
                    {
                        var getUser = await con.users.Where(u => u.usrId == trn.tPaying).FirstOrDefaultAsync();
                        if (getUser != null)
                        {
                            if (getMentorData.usrStatus == "Active")
                            {
                                if (getMentorData.ExpiryDate > DateTime.Now)
                                {
                                    try
                                    {
                                        //make the expiryDate
                                        if (getUser.ExpiryDate > DateTime.Now)
                                        {
                                            getUser.ExpiryDate = getUser.ExpiryDate.AddMonths(1); ;
                                        }
                                        else
                                        {
                                            getUser.ExpiryDate = DateTime.Now.AddMonths(1); ;
                                        }

                                        getUser.usrStatus = "Active";
                                        con.Entry(getUser).State = EntityState.Modified;

                                        con.transections.Add(trn);
                                        await con.SaveChangesAsync();

                                        ////Check for Current User Balance
                                        //var getUserSum = con.transections.Where(t => t.tReceiving == getUser.usrId && t.tStatus == "Verified").Sum(t => t.TuitionAmount);

                                        ////Check for Current Mentor Balance
                                        //var getMentorSum = con.transections.Where(t => t.tReceiving == getUser.usrId && t.tStatus == "Verified").Sum(t => t.TuitionAmount);


                                        //Mail to Mentor
                                        string emailBody = "";
                                        if (getMentorData.CountryId == 6)
                                        {
                                            emailBody = "Congratulation !!<br/><b>" + getMentorData.usrName + " </b> you have received <b>" + trn.TuitionAmount + "/- pkr from your trainee <b>" + getUser.usrName + "-" + getUser.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                        }
                                        else
                                        {
                                            emailBody = "Congratulation !!<br/><b>" + getMentorData.usrName + " </b> you have received <b>" + trn.TuitionAmount + "/- USD from your trainee <b>" + getUser.usrName + "-" + getUser.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                        }
                                        Mailer email = new Mailer("Care@Universalskills.co", getMentorData.usrEmail, "Amount Transferred", emailBody);
                                        email.Send();
                                        InfoSend info = new InfoSend();
                                        info.infoText = emailBody;
                                        info.infoBy = "Email";
                                        info.infoTo = getMentorData.usrId;
                                        info.infoDate = DateTime.Now;
                                        info.infoStatus = "Success";
                                        con.infoSends.Add(info);

                                        string emailBody1 = "";
                                        //Mail to User
                                        if (getUser.CountryId == 6)
                                        {
                                            emailBody1 = "<b>" + getUser.usrName + "</b> you have paid <b>" + totalAmount + "/- pkr</b> on dated <b>" + DateTime.Now + "</b>, <b>" + trn.TuitionAmount + "</b>/- pkr has been reflected in your Mentor's (<b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>) account, <b>" + trn.ThirdPartyCharges + "</b>/- pkr is money transferred fee and <b>" + trn.SoftServiceCharges + "</b>/- pkr is system fee and transaction# is <b>" + trn.tNumber + "</b>.";
                                        }
                                        else
                                        {
                                            emailBody1 = "<b>" + getUser.usrName + "</b> you have paid <b>" + totalAmount + "/- USD</b> on dated <b>" + DateTime.Now + "</b>, <b>" + trn.TuitionAmount + "</b>/- USD has been reflected in your Mentor's (<b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>) account, <b>" + trn.ThirdPartyCharges + "</b>/- USD is money transferred fee and <b>" + trn.SoftServiceCharges + "</b>/- USD is system fee and transaction# is <b>" + trn.tNumber + "</b>.";
                                        }
                                        Mailer email1 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Fee Paid", emailBody1);
                                        email1.Send();
                                        InfoSend info1 = new InfoSend();
                                        info1.infoText = emailBody1;
                                        info1.infoBy = "Email";
                                        info1.infoTo = getUser.usrId;
                                        info1.infoDate = DateTime.Now;
                                        info1.infoStatus = "Success";
                                        con.infoSends.Add(info1);

                                        await con.SaveChangesAsync();

                                        return Ok();
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                                        return BadRequest(new { message = "An error occured while adding transaction Info. Please try again later" });
                                    }
                                }
                                else
                                {

                                    //Add Entry into TraineeLost Table
                                    UserLostTrainee ult = new UserLostTrainee();
                                    ult.usrIdM = trn.tReceiving;
                                    ult.usrIdC = trn.tPaying;
                                    ult.ultDate = DateTime.Now;
                                    if (getMentorData.ExpiryDate.Year > 2000)
                                    {
                                        ult.ultReason = "You have lost your trainee " + getUser.usrName + "(" + getUser.usrCode + ") on dated " + DateTime.Now + " due to unpaid fee " +
                                                          "because your fee last date was " + getMentorData.ExpiryDate + ". With sorrow this is informed you that you cannot take back your team member " + getUser.usrName + "(" + getUser.usrCode + ") " +
                                                          "as per terms, for more detail please see terms and conditions section. Please always be active and pay your fee to your mentor on time to avoid inconvience.";
                                    }
                                    else
                                    {
                                        ult.ultReason = "You have lost your trainee " + getUser.usrName + "(" + getUser.usrCode + ") on dated " + DateTime.Now + " due to unpaid fee " +
                                                                                                  "because you did not pay your fee to your mentor yet. With sorrow this is informed you that you cannot take back your team member " + getUser.usrName + "(" + getUser.usrCode + ") " +
                                                                                                  "as per terms, for more detail please see terms and conditions section. Please always be active and pay your fee to your mentor on time to avoid inconvience.";
                                    }
                                    ult.ultStatus = "Lost";
                                    con.userLostTrainees.Add(ult);
                                    //End Entry into TraineeLost Table


                                    trn.tReceiving = 1;
                                    con.transections.Add(trn);

                                    if (getUser.ExpiryDate > DateTime.Now)
                                    {
                                        getUser.ExpiryDate = getUser.ExpiryDate.AddMonths(1);
                                    }
                                    else
                                    {
                                        getUser.ExpiryDate = DateTime.Now.AddMonths(1);
                                    }

                                    //Updating User Entries
                                    getUser.refId = 1;
                                    getUser.usrStatus = "Active";
                                    con.Entry(getUser).State = EntityState.Modified;

                                    try
                                    {
                                        await con.SaveChangesAsync();

                                        //Mail for Transaction

                                        //Mail to Mentor
                                        //string emailBody = "<b>Congratulations &nbsp;" + getMentorData.usrName + " </b><br/><b>" + trn.TuitionAmount + "</b>has been tranferred in your <b>Universal Skills</b> account from your trainee <b>" + getUser.usrName + "-" + getUser.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                        //Mailer email = new Mailer("Care@Universalskills.co", getMentorData.usrEmail, "Amount Transferred", emailBody);
                                        //email.Send();
                                        //InfoSend info = new InfoSend();
                                        //info.infoText = emailBody;
                                        //info.infoBy = "Email";
                                        //info.infoTo = getMentorData.usrId; 
                                        //info.infoDate = DateTime.Now;
                                        //info.infoStatus = "Success";
                                        //con.infoSends.Add(info);

                                        //Mail to New User
                                        //string emailBody1 = "<b>" + getUser.usrName + " </b><br/><b>" + trn.TuitionAmount + "</b>has been paid. to your mentor <b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                        string emailBody1 = "";
                                        if (getMentorData.ExpiryDate.Year < 2000)
                                        {
                                            if (getMentorData.CountryId == 6)
                                            {
                                                emailBody1 = "<b>" + getUser.usrName + "</b> you have paid <b>" + totalAmount + "/- pkr</b> on dated <b>" + DateTime.Now + "</b>, <b>" + trn.TuitionAmount + "</b>/- pkr has been reflected in your Mentor's (<b>Universal Skills</b>) account, <b>" + trn.ThirdPartyCharges + "</b>/- pkr is money transferred fee and <b>" + trn.SoftServiceCharges + "</b>/- pkr is system fee and transaction# is <b>" + trn.tNumber + "</b>." +
                                                  " <br/>Your mentor (<b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>) did not pay fee to his mentor so as per policy now your mentor (<b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>) has been lost you. Now, your new mentor is company Universal Skills.";
                                            }
                                            else
                                            {
                                                emailBody1 = "<b>" + getUser.usrName + "</b> you have paid <b>" + totalAmount + "/- USD</b> on dated <b>" + DateTime.Now + "</b>, <b>" + trn.TuitionAmount + "</b>/- USD has been reflected in your Mentor's (<b>Universal Skills</b>) account, <b>" + trn.ThirdPartyCharges + "</b>/- USD is money transferred fee and <b>" + trn.SoftServiceCharges + "</b>/- USD is system fee and transaction# is <b>" + trn.tNumber + "</b>." +
                                                  " <br/>Your mentor (<b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>) did not pay fee to his mentor so as per policy now your mentor (<b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>) has been lost you. Now, your new mentor is company Universal Skills.";
                                            }

                                        }
                                        else
                                        {
                                            if (getUser.CountryId == 6)
                                            {
                                                emailBody1 = "<b>" + getUser.usrName + "</b> you have paid <b>" + totalAmount + "/- pkr</b> on dated <b>" + DateTime.Now + "</b>, <b>" + trn.TuitionAmount + "</b>/- pkr has been reflected in your Mentor's (<b>Universal Skills</b>) account, <b>" + trn.ThirdPartyCharges + "</b>/- pkr is money transferred fee and <b>" + trn.SoftServiceCharges + "</b>/- pkr is system fee and transaction# is <b>" + trn.tNumber + "</b>." +
                                                "<br/><b>" + getUser.usrName + " </b><br/> Your mentor <b> " + getMentorData.usrName + " - " + getMentorData.usrCode + " </b> 's subscription was expired on Dated <b>" + getMentorData.ExpiryDate + "</b>" +
                                                ", as per policy now your mentor has been lost you. Now, your new mentor is company <b>Universal Skills</b>";

                                            }
                                            else
                                            {
                                                emailBody1 = "<b>" + getUser.usrName + "</b> you have paid <b>" + totalAmount + "/- USD</b> on dated <b>" + DateTime.Now + "</b>, <b>" + trn.TuitionAmount + "</b>/- USD has been reflected in your Mentor's (<b>Universal Skills</b>) account, <b>" + trn.ThirdPartyCharges + "</b>/- USD is money transferred fee and <b>" + trn.SoftServiceCharges + "</b>/- USD is system fee and transaction# is <b>" + trn.tNumber + "</b>." +
                                                "<br/><b>" + getUser.usrName + " </b><br/> Your mentor <b> " + getMentorData.usrName + " - " + getMentorData.usrCode + " </b> 's subscription was expired on Dated <b>" + getMentorData.ExpiryDate + "</b>" +
                                                ", as per policy now your mentor has been lost you. Now, your new mentor is company <b>Universal Skills</b>";

                                            }

                                        }

                                        Mailer email1 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Fee Paid", emailBody1);
                                        email1.Send();
                                        InfoSend info1 = new InfoSend();
                                        info1.infoText = emailBody1;
                                        info1.infoBy = "Email";
                                        info1.infoTo = getUser.usrId;
                                        info1.infoDate = DateTime.Now;
                                        info1.infoStatus = "Success";
                                        con.infoSends.Add(info1);

                                        await con.SaveChangesAsync();

                                        //Mail for Trainee Lost


                                        //Mail to Mentor
                                        Mailer email = new Mailer("Care@Universalskills.co", getMentorData.usrEmail, "Lost Trainee", ult.ultReason);
                                        email.Send();
                                        InfoSend info = new InfoSend();
                                        info.infoText = ult.ultReason;
                                        info.infoBy = "Email";
                                        info.infoTo = getMentorData.usrId;
                                        info.infoDate = DateTime.Now;
                                        info.infoStatus = "Success";
                                        con.infoSends.Add(info);
                                        await con.SaveChangesAsync();

                                        //Mail to New User
                                        //string emailBody2 = "<b>" + getUser.usrName + " </b><br/>your mentor <b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>'s subscription was expired on Dated <b>" + getMentorData.ExpiryDate + "</b>" +
                                        //    ", as per policy now your mentor has been lost you, your new Mentor is company <b>Universal Skills</b>";
                                        //Mailer email2 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Lost Mentor", emailBody2);
                                        //email2.Send();
                                        //InfoSend info2 = new InfoSend();
                                        //info2.infoText = emailBody1;
                                        //info2.infoBy = "Email";
                                        //info2.infoTo = getUser.usrId;
                                        //info2.infoDate = DateTime.Now;
                                        //info2.infoStatus = "Success";
                                        //con.infoSends.Add(info2);


                                        //End Mail

                                        return Ok();
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                                        return BadRequest(new { message = "An error occured while adding the transaction and trainee Info. Please try again later" });
                                    }
                                }
                            }
                            else
                            {
                                //Add Entry into TraineeLost Table
                                UserLostTrainee ult = new UserLostTrainee();
                                ult.usrIdM = trn.tReceiving;
                                ult.usrIdC = trn.tPaying;
                                ult.ultDate = DateTime.Now;
                                var Clickhere = "https://universalskills.co/Home/TermsAndConditions";
                                ult.ultReason = "You have lost your trainee " + getUser.usrName + "(" + getUser.usrCode + ") on dated " + DateTime.Now + " because your status " +
                                                  "was 'learning' as per policy 'learner mentor' loss their trainees when trainee pays fee. With sorrow this is informed you that you cannot take back your team member " + getUser.usrName + "(" + getUser.usrCode + ") " +
                                                  "as per terms, for more detail please see terms and conditions please " + Clickhere + " section. Please always be active and pay your fee to your mentor on time to avoid inconvience.";

                                ult.ultStatus = "Lost";
                                con.userLostTrainees.Add(ult);
                                //End Entry into TraineeLost Table


                                trn.tReceiving = 1;
                                con.transections.Add(trn);

                                if (getUser.ExpiryDate > DateTime.Now)
                                {
                                    getUser.ExpiryDate = getUser.ExpiryDate.AddMonths(1);
                                }
                                else
                                {
                                    getUser.ExpiryDate = DateTime.Now.AddMonths(1);
                                }

                                //Updating User Entries
                                getUser.refId = 1;
                                getUser.usrStatus = "Active";
                                con.Entry(getUser).State = EntityState.Modified;

                                try
                                {
                                    await con.SaveChangesAsync();

                                    //Mail for Transaction

                                    //Mail to Mentor
                                    //string emailBody = "<b>Congratulations " + getMentorData.usrName + " </b><br/><b>" + trn.TuitionAmount + "</b>has been tranferred in your <b>Universal Skills</b> account from your trainee <b>" + getUser.usrName + "-" + getUser.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                    //Mailer email = new Mailer("Care@Universalskills.co", getMentorData.usrEmail, "Amount Transferred", emailBody);
                                    //email.Send();
                                    //InfoSend info = new InfoSend();
                                    //info.infoText = emailBody;
                                    //info.infoBy = "Email";
                                    //info.infoTo = getMentorData.usrId; 
                                    //info.infoDate = DateTime.Now;
                                    //info.infoStatus = "Success";
                                    //con.infoSends.Add(info);

                                    //Mail to New User for fee paid and Lost mentor
                                    //string emailBody1 = "<b>" + getUser.usrName + " </b><br/><b>" + trn.TuitionAmount + "</b>has been paid. to your mentor <b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                    string emailBody1 = "";
                                    if (getUser.CountryId == 6)
                                    {
                                        emailBody1 = "<b>" + getUser.usrName + "</b> you have paid <b>" + totalAmount + "/- pkr</b> on dated <b>" + DateTime.Now + "</b>, <b>" + trn.TuitionAmount + "</b>/- pkr has been reflected in your Mentor's (<b>Universal Skills</b>) account, <b>" + trn.ThirdPartyCharges + "</b>/- pkr is money transferred fee and <b>" + trn.SoftServiceCharges + "</b>/- pkr is system fee and transaction# is <b>" + trn.tNumber + "</b>." +
                                        " <br/>Your mentor (<b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>) did not pay fee to his mentor so as per policy now your mentor (<b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>) has been lost you. Now, your new mentor is company Universal Skills.";
                                    }
                                    else
                                    {
                                        emailBody1 = "<b>" + getUser.usrName + "</b> you have paid <b>" + totalAmount + "/- USD</b> on dated <b>" + DateTime.Now + "</b>, <b>" + trn.TuitionAmount + "</b>/- USD has been reflected in your Mentor's (<b>Universal Skills</b>) account, <b>" + trn.ThirdPartyCharges + "</b>/- USD is money transferred fee and <b>" + trn.SoftServiceCharges + "</b>/- USD is system fee and transaction# is <b>" + trn.tNumber + "</b>." +
                                        " <br/>Your mentor (<b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>) did not pay fee to his mentor so as per policy now your mentor (<b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>) has been lost you. Now, your new mentor is company Universal Skills.";
                                    }
                                    Mailer email1 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Fee Paid", emailBody1);
                                    email1.Send();
                                    InfoSend info1 = new InfoSend();
                                    info1.infoText = emailBody1;
                                    info1.infoBy = "Email";
                                    info1.infoTo = getUser.usrId;
                                    info1.infoDate = DateTime.Now;
                                    info1.infoStatus = "Success";
                                    con.infoSends.Add(info1);

                                    //Mail for Trainee Lost
                                    //Mail to Mentor
                                    Mailer email2 = new Mailer("Care@Universalskills.co", getMentorData.usrEmail, "Lost Trainee", ult.ultReason);
                                    email2.Send();
                                    InfoSend info2 = new InfoSend();
                                    info2.infoText = ult.ultReason;
                                    info2.infoBy = "Email";
                                    info2.infoTo = getMentorData.usrId;
                                    info2.infoDate = DateTime.Now;
                                    info2.infoStatus = "Success";
                                    con.infoSends.Add(info2);
                                    await con.SaveChangesAsync();

                                    //Mail to New User
                                    //string emailBody2 = "<b>" + getUser.usrName + " </b><br/>your mentor <b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b>'s subscription was expired on Dated <b>" + getMentorData.ExpiryDate + "</b>" +
                                    //    ", as per policy now your mentor has been lost you, your new Mentor is company <b>Universal Skills</b>";
                                    //Mailer email3 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Lost Mentor", emailBody2);
                                    //email3.Send();
                                    //InfoSend info3 = new InfoSend();
                                    //info3.infoText = emailBody1;
                                    //info3.infoBy = "Email";
                                    //info3.infoTo = getUser.usrId;
                                    //info3.infoDate = DateTime.Now;
                                    //info3.infoStatus = "Success";
                                    //con.infoSends.Add(info3);

                                    //await con.SaveChangesAsync();
                                    //End Mail

                                    return Ok();
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return BadRequest(new { message = "An error occured while adding the transaction and trainee Info. Please try again later" });
                                }
                            }

                        }
                        else
                        {
                            return BadRequest(new { message = "No user record found. Please contact to Universal Skills." });
                        }

                    }
                    else
                    {
                        return BadRequest(new { message = "No Mentor record found. Please contact to Universal Skills." });
                    }
                }
            }
            // The variable 'ex' is declared but never used
            catch (Exception ex)
            // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "Data not added." });
            }
        }

        [HttpPost("MyWallet")]
        public async Task<ActionResult<IEnumerable<Transections>>> MyWallet(GetMyWallet wallet)
        {
            try
            {
                if (wallet.Status != null)
                {
                    if (wallet.Status == "Paid Fee")
                    {
                        return await con.transections.Where(t => t.tPaying == wallet.usrId
                        && t.tDate >= wallet.FromDate && t.tDate <= wallet.ToDate).OrderByDescending(t => t.tId).ToListAsync();
                    }
                    else if (wallet.Status == "Pending Received Fee")
                    {
                        return await con.transections.Where(t => t.tReceiving == wallet.usrId && t.tStatus == "Pending"
                        && t.tDate >= wallet.FromDate && t.tDate <= wallet.ToDate).OrderByDescending(t => t.tId).ToListAsync();
                    }
                    else if (wallet.Status == "Received Fee")
                    {
                        return await con.transections.Where(t => t.tReceiving == wallet.usrId && t.tStatus == "Verified"
                        && t.tDate >= wallet.FromDate && t.tDate <= wallet.ToDate).OrderByDescending(t => t.tId).ToListAsync();
                    }
                    else if (wallet.Status == "Transferred Amount")
                    {
                        return await con.transections.Where(t => t.tReceiving == wallet.usrId && t.tStatus == "Completed"
                        && t.tDate >= wallet.FromDate && t.tDate <= wallet.ToDate).OrderByDescending(t => t.tId).ToListAsync();
                    }
                    else
                    {
                        return BadRequest(new { message = "No transaction Record found" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Status not found. Please select the status and try again" });
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the transaction Info. Please try again later" });
            }
            //return BadRequest(new { message = "No Record Found" });
        }

        [HttpGet("GetTransaction")]
        public async Task<ActionResult> GetTransaction()
        {
            try
            {
                List<AppTransaction> transactionsList = new List<AppTransaction>();
                var getTrans = await con.transections.Where(t => t.tStatus == "Pending").OrderByDescending(d => d.tDateTime).ToListAsync();
                if (getTrans.Count != 0)
                {
                    foreach (var i in getTrans)
                    {
                        AppTransaction ap = new AppTransaction();
                        ap.Id = i.tId;
                        var getFrom = await con.users.Where(u => u.usrId == i.tPaying).FirstOrDefaultAsync();
                        if (getFrom != null)
                        {
                            ap.From = getFrom.usrName + "-" + getFrom.usrCode;
                        }
                        var getTo = await con.users.Where(u => u.usrId == i.tReceiving).FirstOrDefaultAsync();

                        if (getTo != null)
                        {
                            ap.To = getTo.usrName + "-" + getTo.usrCode;
                        }
                        ap.Date = i.tDate;
                        ap.Status = i.tStatus;
                        transactionsList.Add(ap);
                    }
                    return Ok(transactionsList);
                }
                else
                {
                    return BadRequest(new { message = "No pending transaction Record found" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "There is some error of getting transaction info" });
            }

        }

        [HttpGet("ApprovedTransaction")]
        public async Task<ActionResult> ApprovedTransaction(int id)
        {
            try
            {
                var getTrans = await con.transections.FindAsync(id);
                if (getTrans == null)
                {
                    return BadRequest(new { message = "No pending transaction Record found" });
                }
                else
                {
                    getTrans.tStatus = "Verified";
                    try
                    {
                        con.Entry(getTrans).State = EntityState.Modified;
                        await con.SaveChangesAsync();
                        return Ok();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return BadRequest(new { message = "An error occured while adding the User Draw Request. Please try again later" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "There is some error of getting transaction info" });
            }

        }

        [HttpPost("AddUDRequest")]
        public async Task<ActionResult<UserDrawRequest>> AddUDRequest(UserDrawRequest udr)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        con.userDrawRequests.Add(udr);
                        await con.SaveChangesAsync();
                        return Ok();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return BadRequest(new { message = "An error occured while adding the User Draw Request. Please try again later" });
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
                return BadRequest(new { message = "An error occured while sending the User Draw Request. Please try again later" });
            }
        }

        [HttpPost("GetDrawRequest")]
        public async Task<ActionResult> GetDrawRequest(GetMyWallet wallet)
        {
            try
            {
                if (wallet.usrCode != 0)
                {
                    if (wallet.Status != null)
                    {
                        if (wallet.Status == "All")
                        {
                            var Getrequest = await (from r in con.userDrawRequests
                                                    join u in con.users on r.usrId equals u.usrId
                                                    join ub in con.userBankInfos on r.ubId equals ub.ubId
                                                    join b in con.banks on ub.BankId equals b.BankId
                                                    where r.udrCreatedDate >= wallet.FromDate
                                                    && r.udrCreatedDate <= wallet.ToDate && u.usrCode == wallet.usrCode
                                                    select new
                                                    {
                                                        u.usrId,
                                                        u.usrName,
                                                        u.usrCode,
                                                        r.udrId,
                                                        r.udrImage,
                                                        r.udrCode,
                                                        r.udrDetail,
                                                        r.udrAmount,
                                                        r.udrCreatedDate,
                                                        r.udrActionDate,
                                                        r.udrRemarks,
                                                        r.udrStatus,
                                                        ub.ubId,
                                                        ub.ubNumber,
                                                        ub.ubTitle,
                                                        b.BankId,
                                                        b.BankName
                                                    }).OrderByDescending(o => o.udrId).ToListAsync();
                            return Ok(Getrequest);
                        }
                        else
                        {
                            var Getrequest = await (from r in con.userDrawRequests
                                                    join u in con.users on r.usrId equals u.usrId
                                                    join ub in con.userBankInfos on r.ubId equals ub.ubId
                                                    join b in con.banks on ub.BankId equals b.BankId
                                                    where r.udrStatus == wallet.Status && r.udrCreatedDate >= wallet.FromDate
                                                    && r.udrCreatedDate <= wallet.ToDate && u.usrCode == wallet.usrCode
                                                    select new
                                                    {
                                                        u.usrId,
                                                        u.usrName,
                                                        u.usrCode,
                                                        r.udrId,
                                                        r.udrCode,
                                                        r.udrImage,
                                                        r.udrDetail,
                                                        r.udrAmount,
                                                        r.udrCreatedDate,
                                                        r.udrActionDate,
                                                        r.udrRemarks,
                                                        r.udrStatus,
                                                        ub.ubId,
                                                        ub.ubNumber,
                                                        ub.ubTitle,
                                                        b.BankId,
                                                        b.BankName
                                                    }).OrderByDescending(o => o.udrId).ToListAsync();
                            return Ok(Getrequest);
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Status not found. Please select the status and try again" });
                    }
                }
                else if (wallet.usrCode == 0 && wallet.usrId == 0)
                {
                    if (wallet.Status != null)
                    {
                        if (wallet.Status == "All")
                        {
                            var Getrequest = await (from r in con.userDrawRequests
                                                    join u in con.users on r.usrId equals u.usrId
                                                    join ub in con.userBankInfos on r.ubId equals ub.ubId
                                                    join b in con.banks on ub.BankId equals b.BankId
                                                    where r.udrCreatedDate >= wallet.FromDate
                                                    && r.udrCreatedDate <= wallet.ToDate
                                                    select new
                                                    {
                                                        u.usrId,
                                                        u.usrName,
                                                        u.usrCode,
                                                        r.udrId,
                                                        r.udrImage,
                                                        r.udrCode,
                                                        r.udrDetail,
                                                        r.udrAmount,
                                                        r.udrCreatedDate,
                                                        r.udrActionDate,
                                                        r.udrRemarks,
                                                        r.udrStatus,
                                                        ub.ubId,
                                                        ub.ubNumber,
                                                        ub.ubTitle,
                                                        b.BankId,
                                                        b.BankName
                                                    }).OrderByDescending(o => o.udrId).ToListAsync();
                            return Ok(Getrequest);
                        }
                        else
                        {
                            var Getrequest = await (from r in con.userDrawRequests
                                                    join u in con.users on r.usrId equals u.usrId
                                                    join ub in con.userBankInfos on r.ubId equals ub.ubId
                                                    join b in con.banks on ub.BankId equals b.BankId
                                                    where r.udrStatus == wallet.Status && r.udrCreatedDate >= wallet.FromDate
                                                    && r.udrCreatedDate <= wallet.ToDate
                                                    select new
                                                    {
                                                        u.usrId,
                                                        u.usrName,
                                                        u.usrCode,
                                                        r.udrId,
                                                        r.udrCode,
                                                        r.udrImage,
                                                        r.udrDetail,
                                                        r.udrAmount,
                                                        r.udrCreatedDate,
                                                        r.udrActionDate,
                                                        r.udrRemarks,
                                                        r.udrStatus,
                                                        ub.ubId,
                                                        ub.ubNumber,
                                                        ub.ubTitle,
                                                        b.BankId,
                                                        b.BankName
                                                    }).OrderByDescending(o => o.udrId).ToListAsync();
                            return Ok(Getrequest);
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Status not found. Please select the status and try again" });
                    }
                }
                else
                {
                    if (wallet.Status == "All" && wallet.usrId != 0)
                    {
                        var Getrequest = await (from r in con.userDrawRequests
                                                join u in con.users on r.usrId equals u.usrId
                                                join ub in con.userBankInfos on r.ubId equals ub.ubId
                                                join b in con.banks on ub.BankId equals b.BankId
                                                where r.udrCreatedDate >= wallet.FromDate
                                                && r.udrCreatedDate <= wallet.ToDate
                                                && r.usrId == wallet.usrId
                                                select new
                                                {
                                                    u.usrId,
                                                    u.usrName,
                                                    u.usrCode,
                                                    r.udrId,
                                                    r.udrImage,
                                                    r.udrCode,
                                                    r.udrDetail,
                                                    r.udrAmount,
                                                    r.udrCreatedDate,
                                                    r.udrActionDate,
                                                    r.udrRemarks,
                                                    r.udrStatus,
                                                    ub.ubId,
                                                    ub.ubNumber,
                                                    ub.ubTitle,
                                                    b.BankId,
                                                    b.BankName
                                                }).OrderByDescending(o => o.udrId).ToListAsync();
                        return Ok(Getrequest);
                    }
                    else
                    {
                        var Getrequest = await (from r in con.userDrawRequests
                                                join u in con.users on r.usrId equals u.usrId
                                                join ub in con.userBankInfos on r.ubId equals ub.ubId
                                                join b in con.banks on ub.BankId equals b.BankId
                                                where r.udrStatus == wallet.Status && r.udrCreatedDate >= wallet.FromDate
                                                && r.udrCreatedDate <= wallet.ToDate
                                                && r.usrId == wallet.usrId
                                                select new
                                                {
                                                    u.usrId,
                                                    u.usrName,
                                                    u.usrCode,
                                                    r.udrId,
                                                    r.udrCode,
                                                    r.udrImage,
                                                    r.udrDetail,
                                                    r.udrAmount,
                                                    r.udrCreatedDate,
                                                    r.udrActionDate,
                                                    r.udrRemarks,
                                                    r.udrStatus,
                                                    ub.ubId,
                                                    ub.ubNumber,
                                                    ub.ubTitle,
                                                    b.BankId,
                                                    b.BankName
                                                }).OrderByDescending(o => o.udrId).ToListAsync();
                        return Ok(Getrequest);
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the transaction Info. Please try again later" });
            }
        }

        [HttpGet("GetDrawInfo")]
        public async Task<ActionResult> GetDrawInfo(int id)
        {
            try
            {
                var GetData = await (from r in con.userDrawRequests
                                     join u in con.users on r.usrId equals u.usrId
                                     join ub in con.userBankInfos on r.ubId equals ub.ubId
                                     join b in con.banks on ub.BankId equals b.BankId
                                     where r.udrId == id
                                     select new
                                     {
                                         u.usrId,
                                         u.usrName,
                                         u.usrCode,
                                         r.udrId,
                                         r.udrCode,
                                         r.udrDetail,
                                         r.udrAmount,
                                         r.udrCreatedDate,
                                         r.udrActionDate,
                                         r.udrRemarks,
                                         r.udrStatus,
                                         r.udrImage,
                                         ub.ubId,
                                         ub.ubNumber,
                                         ub.ubTitle,
                                         b.BankId,
                                         b.BankName
                                     }).FirstOrDefaultAsync();

                if (GetData == null)
                {
                    return NotFound();
                }

                return Ok(GetData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }

        [HttpPut("ApproveDrawRequest")]
        public async Task<IActionResult> ApproveDrawRequest(int id, DrawVM draw)
        {
            try
            {
                if (id != draw.udrId)
                {
                    return BadRequest();
                }
                var getData = await con.userDrawRequests.FindAsync(id);
                if (getData != null)
                {
                    if (draw.udrStatus == "Completed")
                    {
                        UserPayment payment = new UserPayment();
                        payment.usrId = draw.usrId;
                        payment.udrId = draw.udrId;
                        payment.ubId = draw.ubId;
                        payment.upDetail = draw.udrDetail;
                        payment.pDate = DateTime.Now;
                        payment.upAmount = draw.udrAmount;
                        payment.upStatus = draw.udrStatus;
                        payment.upImage = draw.udrImage;
                        con.userPayments.Add(payment);

                        //Update Transection Receveing status
                        var getTrn = await con.transections.Where(t => t.tReceiving == draw.usrId && t.tStatus == "Verified").ToListAsync();
                        if (getTrn.Count != 0)
                        {
                            foreach (var i in getTrn)
                            {
                                i.tStatus = "Completed";
                                con.Entry(i).State = EntityState.Modified;
                            }
                        }
                    }
                    getData.udrActionDate = DateTime.Now;
                    getData.udrStatus = draw.udrStatus;
                    getData.udrRemarks = draw.udrRemarks;
                    getData.udrImage = draw.udrImage;
                    getData.ubId = draw.ubId;
                    con.Entry(getData).State = EntityState.Modified;
                }
                try
                {
                    await con.SaveChangesAsync();
                    if (draw.udrStatus == "Completed")
                    {
                        var usr = await con.users.Where(s => s.usrId == draw.usrId).FirstOrDefaultAsync();
                        if (usr != null)
                        {
                            var getUsrBankInfo = await con.userBankInfos.Where(u => u.ubId == draw.ubId).FirstOrDefaultAsync();
                            if (getUsrBankInfo != null)
                            {
                                //Mail to New User
                                string emailBody1 = "";
                                if (usr.CountryId == 6)
                                {
                                    emailBody1 = "<b>" + usr.usrName + "-" + usr.usrCode + " </b><br/><b>" + draw.udrAmount + "/- pkr</b> amount tranferred into your bank account from <b>Universal Skills</b> on Dated <b>" + DateTime.Now + "</b>." +
                                    "<br/>Account Details:<br/>Account Title: " + getUsrBankInfo.ubTitle + "" +
                                    "<br/>Account Number: " + getUsrBankInfo.ubNumber + "";
                                }
                                else
                                {
                                    emailBody1 = "<b>" + usr.usrName + "-" + usr.usrCode + " </b><br/><b>" + draw.udrAmount + "/- USD</b> amount tranferred into your bank account from <b>Universal Skills</b> on Dated <b>" + DateTime.Now + "</b>." +
                                    "<br/>Account Details:<br/>Account Title: " + getUsrBankInfo.ubTitle + "" +
                                    "<br/>Account Number: " + getUsrBankInfo.ubNumber + "";
                                }
                                Mailer email1 = new Mailer("Care@Universalskills.co", usr.usrEmail, "Amount Tranferred", emailBody1);
                                email1.Send();
                                InfoSend info1 = new InfoSend();
                                info1.infoText = emailBody1;
                                info1.infoBy = "Email";
                                info1.infoTo = usr.usrId;
                                info1.infoDate = DateTime.Now;
                                info1.infoStatus = "Success";
                                con.infoSends.Add(info1);


                                Mailer emailCompany = new Mailer("Care@Universalskills.co", GetAmount.Value.CompanyMail, "Transaction Completed", emailBody1);
                                emailCompany.Send();
                                InfoSend_Company cinfo = new InfoSend_Company();
                                cinfo.cInfoText = emailBody1;
                                cinfo.cInfoBy = "Email";
                                cinfo.cInfoTo = usr.usrId;
                                cinfo.cInfoDate = DateTime.Now;
                                cinfo.cInfoStatus = "Success";
                                con.infoSend_Companies.Add(cinfo);

                                await con.SaveChangesAsync();

                            }
                        }

                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest(new { message = "An error occured while updating Info. Please try again later" });
                }

                return NoContent();
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }

        }

        [HttpPost("UsedCoupan")]
        public async Task<ActionResult<UserDrawRequest>> UsedCoupan(Coupan cpn)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var CheckData = await con.coupans.Where(c => c.cCode == cpn.cCode).FirstOrDefaultAsync();
                    if (CheckData != null)
                    {
                        try
                        {
                            if (CheckData.cExpiryDate > DateTime.Now)
                            {
                                if (CheckData.cStatus == false)
                                {
                                    //Add Entry on transection Page
                                    var getUser = await con.users.Where(u => u.usrId == cpn.cUsedUserId).FirstOrDefaultAsync();
                                    if (getUser != null)
                                    {
                                        if (CheckData.cAssignedUserId == getUser.refId || CheckData.cAssignedUserId == getUser.usrId)
                                        {
                                            CheckData.cUsedUserId = cpn.cUsedUserId;
                                            CheckData.cUsedDate = DateTime.Now;
                                            CheckData.cStatus = true;
                                            con.Entry(CheckData).State = EntityState.Modified;

                                            var getMentor = await con.users.Where(u => u.usrId == getUser.refId).FirstOrDefaultAsync();
                                            if (getMentor != null)
                                            {
                                                if (getMentor.usrStatus == "Learning")
                                                {
                                                    //Add Entry into TraineeLost Table
                                                    UserLostTrainee ult = new UserLostTrainee();
                                                    ult.usrIdM = getUser.refId; ;
                                                    ult.usrIdC = getUser.usrId;
                                                    ult.ultDate = DateTime.Now;
                                                    ult.ultReason = "You have lost your trainee " + getUser.usrName + "(" + getUser.usrCode + ") on dated " + DateTime.Now + " because your status " +
                                                       "was learning as per policy 'learner mentor' loss their trainees when trainee pays fee. With sorrow this is informed you that you cannot take back your team member " + getUser.usrName + "(" + getUser.usrCode + ") " +
                                                       "as per terms, for more detail please see terms and conditions section. Please always be active and pay your fee to your mentor on time to avoid inconvience.";
                                                    ult.ultStatus = "Lost";
                                                    con.userLostTrainees.Add(ult);
                                                    //End Entry into TraineeLost Table


                                                    var SerialNo = GetSerialNo();
                                                    Transections trn = new Transections();
                                                    trn.tDate = DateTime.Now.Date;
                                                    trn.tDateTime = DateTime.Now;
                                                    trn.tStatus = "Pending";
                                                    trn.tNarration = "Used Coupan Code of " + getUser.usrName + "(" + getUser.usrCode + ")";
                                                    trn.tPaying = getUser.usrId;
                                                    trn.tReceiving = 1;
                                                    trn.TuitionAmount = CheckData.cAmount;
                                                    trn.SoftServiceCharges = 0;
                                                    trn.ThirdPartyCharges = 0;
                                                    trn.tNumber = "" + getUser.usrCode + "-" + DateTime.Now.ToString("ddMMyy") + "-" + SerialNo + "";
                                                    con.transections.Add(trn);

                                                    //Updating User Epiry Date
                                                    if (getUser.ExpiryDate > DateTime.Now)
                                                    {
                                                        getUser.ExpiryDate = getUser.ExpiryDate.AddDays(15);
                                                    }
                                                    else
                                                    {
                                                        getUser.ExpiryDate = DateTime.Now.AddDays(15);
                                                    }

                                                    getUser.refId = 1;
                                                    getUser.usrStatus = "Active";
                                                    con.Entry(getUser).State = EntityState.Modified;

                                                    try
                                                    {
                                                        //con.userDrawRequests.Add(udr);
                                                        await con.SaveChangesAsync();

                                                        //Mail for Transaction

                                                        //Mail to User
                                                        //string emailBody1 = "<b>" + getUser.usrName + " </b><br/><b>" + trn.TuitionAmount + "</b>has been paid. to your mentor <b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                                        string emailBody1 = "";
                                                        if (getUser.CountryId == 6)
                                                        {
                                                            emailBody1 = "<b>" + getUser.usrName + " </b><br/>You have paid <b>" + trn.TuitionAmount + "/- pkr</b> on Dated <b>" + DateTime.Now + "</b>.";
                                                        }
                                                        else
                                                        {
                                                            emailBody1 = "<b>" + getUser.usrName + " </b><br/>You have paid <b>" + trn.TuitionAmount + "/- USD</b> on Dated <b>" + DateTime.Now + "</b>.";
                                                        }
                                                        Mailer email1 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Fee Paid", emailBody1);
                                                        email1.Send();
                                                        InfoSend info1 = new InfoSend();
                                                        info1.infoText = emailBody1;
                                                        info1.infoBy = "Email";
                                                        info1.infoTo = getUser.usrId;
                                                        info1.infoDate = DateTime.Now;
                                                        info1.infoStatus = "Success";
                                                        con.infoSends.Add(info1);

                                                        await con.SaveChangesAsync();



                                                        //Mail for Trainee Lost
                                                        //Mail to Mentor
                                                        Mailer email2 = new Mailer("Care@Universalskills.co", getMentor.usrEmail, "Lost Trainee", ult.ultReason);
                                                        email2.Send();
                                                        InfoSend info2 = new InfoSend();
                                                        info2.infoText = ult.ultReason;
                                                        info2.infoBy = "Email";
                                                        info2.infoTo = getMentor.usrId;
                                                        info2.infoDate = DateTime.Now;
                                                        info2.infoStatus = "Success";
                                                        con.infoSends.Add(info2);

                                                        //Mail to New User
                                                        string emailBody2 = "<b>" + getUser.usrName + " </b><br/>your mentor <b>" + getMentor.usrName + "-" + getMentor.usrCode + "</b>'s subscription was expired on Dated <b>" + getMentor.ExpiryDate + "</b>" +
                                                            ", as per policy now your mentor has been lost you, your new Mentor is company <b>Universal Skills</b>";
                                                        Mailer email3 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Lost Mentor", emailBody2);
                                                        email3.Send();
                                                        InfoSend info3 = new InfoSend();
                                                        info3.infoText = emailBody1;
                                                        info3.infoBy = "Email";
                                                        info3.infoTo = getUser.usrId;
                                                        info3.infoDate = DateTime.Now;
                                                        info3.infoStatus = "Success";
                                                        con.infoSends.Add(info3);

                                                        await con.SaveChangesAsync();
                                                        //End Mail


                                                        return Ok();
                                                    }
                                                    catch (DbUpdateConcurrencyException)
                                                    {
                                                        return BadRequest(new { message = "An error occured while adding the Info. Please try again later" });
                                                    }
                                                }
                                                else if (getMentor.ExpiryDate < DateTime.Now)
                                                {
                                                    //Add Entry into TraineeLost Table
                                                    UserLostTrainee ult = new UserLostTrainee();
                                                    ult.usrIdM = getUser.refId;
                                                    ult.usrIdC = getUser.usrId;
                                                    ult.ultDate = DateTime.Now;
                                                    ult.ultReason = "You have lost your trainee " + getUser.usrName + "(" + getUser.usrCode + ") on dated " + DateTime.Now + " due to unpaid fee " +
                                                       "because your fee last date was " + getMentor.ExpiryDate + ". With sorrow this is informed you that you cannot take back your team member " + getUser.usrName + "(" + getUser.usrCode + ") " +
                                                       "as per terms, for more detail please see terms and conditions section. Please always be active and pay your fee to your mentor on time to avoid inconvience.";
                                                    ult.ultStatus = "Lost";
                                                    con.userLostTrainees.Add(ult);
                                                    //End Entry into TraineeLost Table


                                                    var SerialNo = GetSerialNo();
                                                    Transections trn = new Transections();
                                                    trn.tDate = DateTime.Now.Date;
                                                    trn.tDateTime = DateTime.Now;
                                                    trn.tStatus = "Pending";
                                                    trn.tNarration = "Used Coupan Code of " + getUser.usrName + "(" + getUser.usrCode + ")";
                                                    trn.tPaying = getUser.usrId;
                                                    trn.tReceiving = 1;
                                                    trn.TuitionAmount = 0;
                                                    trn.SoftServiceCharges = 0;
                                                    trn.ThirdPartyCharges = 0;
                                                    trn.tNumber = "" + getUser.usrCode + "-" + DateTime.Now.ToString("ddMMyy") + "-" + SerialNo + "";
                                                    con.transections.Add(trn);

                                                    //Updating User Epiry Date
                                                    if (getUser.ExpiryDate > DateTime.Now)
                                                    {
                                                        getUser.ExpiryDate = getUser.ExpiryDate.AddDays(15);
                                                    }
                                                    else
                                                    {
                                                        getUser.ExpiryDate = DateTime.Now.AddDays(15);
                                                    }

                                                    getUser.refId = 1;
                                                    getUser.usrStatus = "Active";
                                                    con.Entry(getUser).State = EntityState.Modified;

                                                    try
                                                    {
                                                        await con.SaveChangesAsync();

                                                        //Mail for Transaction

                                                        //Mail to User
                                                        //string emailBody1 = "<b>" + getUser.usrName + " </b><br/><b>" + trn.TuitionAmount + "</b>has been paid. to your mentor <b>" + getMentorData.usrName + "-" + getMentorData.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                                        string emailBody1 = "";
                                                        if (getUser.CountryId == 6)
                                                        {
                                                            emailBody1 = "<b>" + getUser.usrName + " </b><br/>You have paid <b>" + trn.TuitionAmount + "/- pkr</b> on Dated <b>" + DateTime.Now + "</b>.";
                                                        }
                                                        else
                                                        {
                                                            emailBody1 = "<b>" + getUser.usrName + " </b><br/>You have paid <b>" + trn.TuitionAmount + "/- USD</b> on Dated <b>" + DateTime.Now + "</b>.";
                                                        }
                                                        Mailer email1 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Fee Paid", emailBody1);
                                                        email1.Send();
                                                        InfoSend info1 = new InfoSend();
                                                        info1.infoText = emailBody1;
                                                        info1.infoBy = "Email";
                                                        info1.infoTo = getUser.usrId;
                                                        info1.infoDate = DateTime.Now;
                                                        info1.infoStatus = "Success";
                                                        con.infoSends.Add(info1);

                                                        await con.SaveChangesAsync();



                                                        //Mail for Trainee Lost
                                                        //Mail to Mentor
                                                        Mailer email2 = new Mailer("Care@Universalskills.co", getMentor.usrEmail, "Lost Trainee", ult.ultReason);
                                                        email2.Send();
                                                        InfoSend info2 = new InfoSend();
                                                        info2.infoText = ult.ultReason;
                                                        info2.infoBy = "Email";
                                                        info2.infoTo = getMentor.usrId;
                                                        info2.infoDate = DateTime.Now;
                                                        info2.infoStatus = "Success";
                                                        con.infoSends.Add(info2);

                                                        //Mail to New User
                                                        string emailBody2 = "<b>" + getUser.usrName + " </b><br/>your mentor <b>" + getMentor.usrName + "-" + getMentor.usrCode + "</b>'s subscription was expired on Dated <b>" + getMentor.ExpiryDate + "</b>" +
                                                            ", as per policy now your mentor has been lost you, your new Mentor is company <b>Universal Skills</b>";
                                                        Mailer email3 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Lost Mentor", emailBody2);
                                                        email3.Send();
                                                        InfoSend info3 = new InfoSend();
                                                        info3.infoText = emailBody1;
                                                        info3.infoBy = "Email";
                                                        info3.infoTo = getUser.usrId;
                                                        info3.infoDate = DateTime.Now;
                                                        info3.infoStatus = "Success";
                                                        con.infoSends.Add(info3);

                                                        await con.SaveChangesAsync();
                                                        //End Mail



                                                        return Ok();
                                                    }
                                                    catch (DbUpdateConcurrencyException)
                                                    {
                                                        return BadRequest(new { message = "An error occured while adding the Info. Please try again later" });
                                                    }
                                                }
                                                else
                                                {
                                                    var SerialNo = GetSerialNo();
                                                    Transections trn = new Transections();
                                                    trn.tDate = DateTime.Now.Date;
                                                    trn.tDateTime = DateTime.Now;
                                                    trn.tStatus = "Pending";
                                                    trn.tNarration = "Used Coupan Code of " + getUser.usrName + "(" + getUser.usrCode + ")";
                                                    trn.tPaying = getUser.usrId;
                                                    trn.tReceiving = getUser.refId;
                                                    trn.TuitionAmount = 0;
                                                    trn.SoftServiceCharges = 0;
                                                    trn.ThirdPartyCharges = 0;
                                                    trn.tNumber = "" + getUser.usrCode + "-" + DateTime.Now.ToString("ddMMyy") + "-" + SerialNo + "";
                                                    con.transections.Add(trn);

                                                    //Updating User Epiry Date
                                                    if (getUser.ExpiryDate > DateTime.Now)
                                                    {
                                                        getUser.ExpiryDate = getUser.ExpiryDate.AddDays(15);
                                                    }
                                                    else
                                                    {
                                                        getUser.ExpiryDate = DateTime.Now.AddDays(15);
                                                    }

                                                    getUser.usrStatus = "Active";
                                                    con.Entry(getUser).State = EntityState.Modified;

                                                    try
                                                    {
                                                        //con.userDrawRequests.Add(udr);
                                                        await con.SaveChangesAsync();

                                                        //Mail to Mentor
                                                        string emailBody = "";
                                                        if (getMentor.CountryId == 6)
                                                        {
                                                            emailBody = "<b>Congratulations" + getMentor.usrName + " </b><br/><b>" + trn.TuitionAmount + "/- pkr</b> has been tranferred in your <b>Universal Skills</b> account from your trainee <b>" + getUser.usrName + "-" + getUser.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                                        }
                                                        else
                                                        {
                                                            emailBody = "<b>Congratulations" + getMentor.usrName + " </b><br/><b>" + trn.TuitionAmount + "/- USD</b> has been tranferred in your <b>Universal Skills</b> account from your trainee <b>" + getUser.usrName + "-" + getUser.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                                        }
                                                        Mailer email = new Mailer("Care@Universalskills.co", getMentor.usrEmail, "Amount Transferred", emailBody);
                                                        email.Send();
                                                        InfoSend info = new InfoSend();
                                                        info.infoText = emailBody;
                                                        info.infoBy = "Email";
                                                        info.infoTo = getMentor.usrId;
                                                        info.infoDate = DateTime.Now;
                                                        info.infoStatus = "Success";
                                                        con.infoSends.Add(info);

                                                        //Mail to User
                                                        string emailBody1 = "";
                                                        if (getUser.CountryId == 6)
                                                        {
                                                            emailBody1 = "<b>" + getUser.usrName + " </b><br/><b>" + trn.TuitionAmount + "/- pkr</b> has been paid to your mentor <b>" + getMentor.usrName + "-" + getMentor.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                                        }
                                                        else
                                                        {
                                                            emailBody1 = "<b>" + getUser.usrName + " </b><br/><b>" + trn.TuitionAmount + "/- USD</b> has been paid to your mentor <b>" + getMentor.usrName + "-" + getMentor.usrCode + "</b> on Dated <b>" + DateTime.Now + "</b>.";
                                                        }
                                                        Mailer email1 = new Mailer("Care@Universalskills.co", getUser.usrEmail, "Fee Paid", emailBody1);
                                                        email1.Send();
                                                        InfoSend info1 = new InfoSend();
                                                        info1.infoText = emailBody1;
                                                        info1.infoBy = "Email";
                                                        info1.infoTo = getUser.usrId;
                                                        info1.infoDate = DateTime.Now;
                                                        info1.infoStatus = "Success";
                                                        con.infoSends.Add(info1);

                                                        await con.SaveChangesAsync();

                                                        return Ok();
                                                    }
                                                    catch (DbUpdateConcurrencyException)
                                                    {
                                                        return BadRequest(new { message = "An error occured while adding the Info. Please try again later" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                return BadRequest(new { message = "No Mentor record found" });
                                            }
                                        }
                                        else
                                        {
                                            return BadRequest(new { message = "This coupan does not belong to your mentor. Please contact to your mentor" });
                                        }

                                    }
                                    else
                                    {
                                        return BadRequest(new { message = "User record not found." });
                                    }

                                }
                                else
                                {
                                    return BadRequest(new { message = "This coupan is already used." });
                                }

                            }
                            else
                            {
                                return BadRequest(new { message = "This coupan is expired." });
                            }
                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        {

                            return BadRequest(new { message = "Coupan Values is not correct." });
                        }

                    }
                    else
                    {
                        return BadRequest(new { message = "Incorrect Coupan Value." });
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
                return BadRequest(new { message = "An error occured while Checking the coupan values. Please try again later" });
            }
        }

        [HttpPost("AddCoupan")]
        public async Task<ActionResult<UserDrawRequest>> AddCoupan(Coupan cpn)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int cBy = cpn.cCreatedBy;
                    int qty = cpn.cCode;
                    int AssignTo = cpn.cAssignedUserId;
                    DateTime eDate = cpn.cExpiryDate;
                    double amount = cpn.cAmount;


                    for (var i = 0; i < qty; i++)
                    {
                        Coupan c = new Coupan();
                        c.cCode = Convert.ToInt32(RandomString(8));
                        c.cAmount = amount;
                        c.cStatus = false;
                        c.cCreatedDate = DateTime.Now;
                        c.cCreatedBy = cBy;
                        c.cExpiryDate = eDate;
                        c.cAssignedUserId = AssignTo;
                        c.cUsedDate = DateTime.Now.AddYears(-100);
                        con.coupans.Add(c);
                        try
                        {
                            await con.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return BadRequest(new { message = "An error occured while adding the Info. Please try again later" });
                        }
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
            return Ok();
        }

        [HttpGet("GetCoupan")]
        public async Task<ActionResult> GetCoupan()
        {
            try
            {
                List<CoupanVM> cList = new List<CoupanVM>();
                var GetData = await con.coupans.OrderBy(c => c.cStatus == false).ToListAsync();
                if (GetData.Count > 0)
                {
                    foreach (var i in GetData)
                    {
                        CoupanVM c = new CoupanVM();
                        c.cId = i.cId;
                        c.cAmount = i.cAmount;
                        c.cCode = i.cCode;
                        c.cStatus = i.cStatus;
                        c.cCreatedDate = i.cCreatedDate;
                        c.cExpiryDate = i.cExpiryDate;
                        c.cUsedDate = i.cUsedDate;
                        var getCreatedBy = await con.users.Where(a => a.usrId == i.cCreatedBy).FirstOrDefaultAsync();
                        if (getCreatedBy != null)
                        {
                            c.CreatedBy = getCreatedBy.usrName + "(" + getCreatedBy.usrCode + ")";
                        }
                        var getAssignTo = await con.users.Where(a => a.usrId == i.cAssignedUserId).FirstOrDefaultAsync();
                        if (getAssignTo != null)
                        {
                            c.AssignedUser = getAssignTo.usrName + "(" + getAssignTo.usrCode + ")";
                        }
                        var getUsedBy = await con.users.Where(a => a.usrId == i.cUsedUserId).FirstOrDefaultAsync();
                        if (getUsedBy != null)
                        {
                            c.UsedUser = getUsedBy.usrName + "(" + getUsedBy.usrCode + ")";
                        }
                        cList.Add(c);
                    }
                }
                if (cList == null)
                {
                    return NotFound();
                }

                return Ok(cList);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the coupan Info. Please try again later." });
            }
        }

        [HttpGet("GetMyCoupan")]
        public async Task<ActionResult> GetMyCoupan(int id)
        {
            try
            {
                List<CoupanVM> cList = new List<CoupanVM>();
                var GetData = await con.coupans.Where(p => p.cAssignedUserId == id).OrderBy(c => c.cStatus == false).ToListAsync();
                if (GetData.Count > 0)
                {
                    foreach (var i in GetData)
                    {
                        CoupanVM c = new CoupanVM();
                        c.cId = i.cId;
                        c.cAmount = i.cAmount;
                        c.cCode = i.cCode;
                        c.cStatus = i.cStatus;
                        c.cCreatedDate = i.cCreatedDate;
                        c.cExpiryDate = i.cExpiryDate;
                        c.cUsedDate = i.cUsedDate;
                        var getCreatedBy = await con.users.Where(a => a.usrId == i.cCreatedBy).FirstOrDefaultAsync();
                        if (getCreatedBy != null)
                        {
                            c.CreatedBy = getCreatedBy.usrName + "(" + getCreatedBy.usrCode + ")";
                        }
                        var getAssignTo = await con.users.Where(a => a.usrId == i.cAssignedUserId).FirstOrDefaultAsync();
                        if (getAssignTo != null)
                        {
                            c.AssignedUser = getAssignTo.usrName + "(" + getAssignTo.usrCode + ")";
                        }
                        var getUsedBy = await con.users.Where(a => a.usrId == i.cUsedUserId).FirstOrDefaultAsync();
                        if (getUsedBy != null)
                        {
                            c.UsedUser = getUsedBy.usrName + "(" + getUsedBy.usrCode + ")";
                        }
                        cList.Add(c);
                    }
                }

                return Ok(cList);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the coupan Info. Please try again later." });
            }
        }

        [HttpGet("PopulateUBInfo")]
        public async Task<ActionResult<IEnumerable<UserBankInfo>>> PopulateUBInfo(int id)
        {
            List<UserBankInfo> list = new List<UserBankInfo>();
            var BankData = await con.userBankInfos.Where(b => b.usrId == id &&
           (b.Status == "Verified" || b.Status == "Used")).ToListAsync();
            if (BankData != null)
            {
                foreach (var i in BankData)
                {
                    UserBankInfo info = new UserBankInfo();
                    var getbankName = await con.banks.Where(b => b.BankId == i.BankId).FirstOrDefaultAsync();
                    if (getbankName != null)
                    {
                        info.ubId = i.ubId;
                        info.ubDetail = getbankName.BankName + " (" + i.ubNumber + ")";
                        list.Add(info);
                    }
                }
            }
            return list;
        }


        [HttpGet("GetInfoSend")]
        public async Task<ActionResult> GetInfoSend()
        {
            try
            {
                var GetData = await (from i in con.infoSends
                                     join u in con.users on i.infoTo equals u.usrId

                                     select new
                                     {
                                         User = u.usrName,
                                         Code = u.usrCode,
                                         i.infoId,
                                         i.infoBy,
                                         i.infoDate,
                                         i.infoStatus,
                                         i.infoText
                                     }).OrderByDescending(o => o.infoId).ToListAsync();

                if (GetData == null)
                {
                    return NotFound();
                }

                return Ok(GetData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }

        [HttpGet("GetInfoSendForCompany")]
        public async Task<ActionResult> GetInfoSendForCompany()
        {
            try
            {
                var GetData = await (from i in con.infoSend_Companies
                                     join u in con.users on i.cInfoTo equals u.usrId

                                     select new
                                     {
                                         User = u.usrName,
                                         Code = u.usrCode,
                                         i.cInfoId,
                                         i.cInfoBy,
                                         i.cInfoDate,
                                         i.cInfoStatus,
                                         i.cInfoText
                                     }).OrderByDescending(o => o.cInfoId).ToListAsync();

                if (GetData == null)
                {
                    return NotFound();
                }

                return Ok(GetData);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return BadRequest(new { message = "An error occured while getting the user bank Info data. Please try again later." });
            }
        }

        [HttpGet("GetFeeBreakup")]
        public async Task<ActionResult<FeeBreakup>> GetFeeBreakup()
        {
            try
            {
                var data = await con.feeBreakups.Where(f => f.fbStatus == true).FirstOrDefaultAsync();
                if (data != null)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(new { message = "no record found" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "error" });
            }


        }

        [HttpGet("GetSerialNo")]
        public string GetSerialNo()
        {
            var SerialNo = "1";
            var getLastNo = con.transections.OrderByDescending(t => t.tId).FirstOrDefault();
            if (getLastNo != null)
            {
                //var Num = getLastNo.tNumber.Substring(getLastNo.tNumber.Length - 1);
                var Num = getLastNo.tNumber.Split('-').Skip(2).FirstOrDefault();
                SerialNo = (Convert.ToInt32(Num) + 1).ToString();
                return SerialNo;
            }
            else
            {
                return SerialNo;
            }
        }


        [HttpGet("GetUDRSerialNo")]
        public string GetUDRSerialNo()
        {
            var SerialNo = "1";
            var getLastNo = con.userDrawRequests.OrderByDescending(t => t.udrId).FirstOrDefault();
            if (getLastNo != null)
            {
                //var Num = getLastNo.tNumber.Substring(getLastNo.tNumber.Length - 1);
                var Num = getLastNo.udrCode.Split('-').Skip(2).FirstOrDefault();
                SerialNo = (Convert.ToInt32(Num) + 1).ToString();
                return SerialNo;
            }
            else
            {
                return SerialNo;
            }
        }

        [HttpGet("GetFeeAmount")]
        public SkillsFee GetFeeAmount()
        {
            SkillsFee fee = new SkillsFee();
            fee.Total = GetAmount.Value.Total;
            fee.SoftwdareFee = GetAmount.Value.SoftwdareFee;
            fee.TuitionFee = GetAmount.Value.TuitionFee;
            fee.ThirdPartyFee = GetAmount.Value.ThirdPartyFee;
            fee.CompanyMail = GetAmount.Value.CompanyMail;
            fee.MailMessage = GetAmount.Value.MailMessage;
            fee.DrawAmount = GetAmount.Value.DrawAmount;
            fee.SignUpAmount = GetAmount.Value.SignUpAmount;

            //for International
            fee.IntTotal = GetAmount.Value.IntTotal;
            fee.IntTuitionFee = GetAmount.Value.IntTuitionFee;
            fee.IntSoftwdareFee = GetAmount.Value.IntSoftwdareFee;
            fee.IntThirdPartyFee = GetAmount.Value.IntThirdPartyFee;
            fee.IntDrawAmount = GetAmount.Value.IntDrawAmount;

            return fee;
        }

        [HttpGet("GetUserBalancec")]
        public double GetUserBalancec(int id)
        {
            double Balance = 0;
            var getSum = con.transections.Where(t => t.tReceiving == id && t.tStatus == "Verified").Sum(t => t.TuitionAmount);
            if (getSum != 0)
            {
                Balance = Convert.ToDouble(getSum);
                return Balance;
            }
            else
            {
                return Balance;
            }
        }

        [HttpGet("GetUserCode")]
        public ActionResult GetUserCode()
        {
            List<UserCode> cList = new List<UserCode>();
            var Code = (from s in con.users
                        where s.usrStatus == "Active"
                        select new
                        {
                            s.usrId,
                            s.usrCode,
                            s.usrName
                        }).OrderBy(o => o.usrName).ToList();
            if (Code.Count > 0)
            {
                foreach (var i in Code)
                {
                    UserCode c = new UserCode();
                    c.usrId = i.usrId;
                    c.usrCode = i.usrCode;
                    c.usrName = i.usrName + "-" + i.usrCode + "";
                    cList.Add(c);
                }
            }
            return Ok(cList);
        }

        [HttpGet("GetAllUserCode")]
        public ActionResult GetAllUserCode()
        {
            List<UserCode> cList = new List<UserCode>();
            var Code = (from s in con.users
                        select new
                        {
                            s.usrId,
                            s.usrCode,
                            s.usrName
                        }).OrderBy(o => o.usrName).ToList();
            if (Code.Count > 0)
            {
                foreach (var i in Code)
                {
                    UserCode c = new UserCode();
                    c.usrId = i.usrId;
                    c.usrCode = i.usrCode;
                    c.usrName = i.usrName + "-" + i.usrCode + "";
                    cList.Add(c);
                }
                return Ok(cList);
            }
            else
            {
                return BadRequest(new { message = "No Record found" });
            }

        }

        [HttpGet("CheckDrawRequest")]
        public async Task<ActionResult<UserDrawRequest>> CheckDrawRequest(int id)
        {
            var chkReq = await con.userDrawRequests.Where(u => u.usrId == id && (u.udrStatus != "Completed" && u.udrStatus != "Rejected")).FirstOrDefaultAsync();
            if (chkReq != null)
            {
                return BadRequest(chkReq);
            }
            else
            {
                return Ok();
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