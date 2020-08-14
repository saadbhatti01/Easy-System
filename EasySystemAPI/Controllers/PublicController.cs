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
    public class PublicController : ControllerBase
    {
        private readonly EasyContext con;
        public PublicController(EasyContext context)
        {
            con = context;
        }

        [HttpGet("GetTrainingInfo")]
        public async Task<ActionResult<List<TrainingInfo>>> GetTrainingInfo()
        {
            return await con.trainingInfos.Where(u => u.tiStatus == "Active").OrderByDescending(c => c.tiId).ToListAsync();
        }

        [HttpGet("GetTrainingType")]
        public async Task<ActionResult<List<TrainingType>>> GetTrainingType()
        {
            return await con.trainingTypes.Where(u => u.ttStatus == "Active").OrderByDescending(c => c.ttId).ToListAsync();
        }

        [HttpGet("GetHelpingMaterialInfo")]
        public async Task<ActionResult<List<UserHelpingMaterial>>> GetHelpingMaterialInfo()
        {
            return await con.userHelpingMaterial.Where(u => u.umsStatus == true).ToListAsync();
        }

        [HttpGet("GetAmount")]
        public async Task<ActionResult<TrainingInfo>> GetAmount(int id)
        {
            var data = await con.trainingInfos.Where(u => u.tiId == id).FirstOrDefaultAsync();

            if (data == null)
            {
                return NotFound();
            }

            return data;
        }

        [HttpPost("PayFee")]
        public async Task<ActionResult> PayFee(FeeList fee)
        {
            fee.flStatus = "Pending";
            fee.flCreatedDate = DateTime.Now;
            fee.RemarksByAdmin = "";
            con.feeLists.Add(fee);
            await con.SaveChangesAsync();
            string emailBody = "<b>Dear&nbsp;" + fee.flTraineeName + "</b><br/> This is the confirmation email that your test fee has been deposited.";
            Mailer email = new Mailer("Care@Universalskills.co", fee.flTraineeEmail, "Test Email", emailBody);
            email.Send();
            return Ok();
        }

        [HttpGet("GetTrainingDetail")]
        public async Task<ActionResult<TrainingInfo>> GetTrainingDetail(int id)
        {
            var data = await con.trainingInfos.Where(u => u.tiId == id).FirstOrDefaultAsync();

            if (data == null)
            {
                return NotFound();
            }

            return data;
        }

        [HttpPost("AdminLogin")]
        public async Task<ActionResult<Users>> AdminLogin(Users users)
        {
            var data = await con.users.Where(u => u.usrEmail == users.usrEmail && u.usrPassword == users.usrPassword).FirstOrDefaultAsync();

            if (data == null)
            {
                return BadRequest(new { message = "Invalid email or password"});
            }

            return Ok(data);
        }

        [HttpGet("AdminGetTrainingInfo")]
        public async Task<ActionResult<List<TrainingInfo>>> AdminGetTrainingInfo()
        {
            var getData = await (from u in con.trainingInfos
                                 join b in con.trainingTypes on u.ttId equals b.ttId

                                 select new
                                 {
                                     u.tiId,
                                     u.tiName,
                                     u.tiDetail,
                                     u.tiPriceDuration,
                                     u.tiPrice,
                                     u.tiDuration,
                                     u.tiStatus,
                                     b.ttName
                                 }).OrderByDescending(o => o.tiId).ToListAsync();
            return Ok(getData);
        }

        [HttpPost("AdminAddTrainingInfo")]
        public async Task<ActionResult<List<TrainingInfo>>> AdminAddTrainingInfo(TrainingInfo data)
        {
            if(data != null)
            {
                data.tiImage = "Leave It";
                data.tiCoverPage = "Leave It";
                con.trainingInfos.Add(data);
                await con.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost("AdminUpdateTrainingInfo")]
        public async Task<ActionResult> AdminUpdateTrainingInfo(TrainingInfo data)
        {
            var getData = await con.trainingInfos.Where(t => t.tiId == data.tiId).FirstOrDefaultAsync();
            if(getData != null)
            {
                getData.tiName = data.tiName;
                getData.tiPrice = data.tiPrice;
                getData.tiPriceDuration = data.tiPriceDuration;
                getData.tiStatus = data.tiStatus;
                getData.tiDetail = data.tiDetail;
                getData.ttId = data.ttId;
                getData.tiDuration = data.tiDuration;
                con.Entry(getData).State = EntityState.Modified;
                await con.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "No record found." });
            }
        }

        [HttpGet("AdminDeleteTrainingInfo")]
        public async Task<ActionResult> AdminDeleteTrainingInfo(int id)
        {
            var data = await con.trainingInfos.Where(u => u.tiId == id).FirstOrDefaultAsync();

            if (data == null)
            {
                return NotFound();
            }
            con.trainingInfos.Remove(data);
            await con.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("AdminGetFeeList")]
        public async Task<ActionResult<List<FeeList>>> AdminGetFeeList()
        {
           return await con.feeLists.OrderByDescending(c => c.tiId).ToListAsync();
            
        }

        [HttpGet("GetTrainingInfoCount")]
        public int GetTrainingInfoCount()
        {
            int Count = 0;
            Count =  con.trainingInfos.Count();
            return Count;
        }

        [HttpGet("TrainingTypeCount")]
        public int TrainingTypeCount()
        {
            int Count = 0;
            Count = con.trainingTypes.Count();
            return Count;
        }

        [HttpGet("AdminDashboardFeeList")]
        public async Task<ActionResult<List<FeeListVM>>> AdminDashboardFeeList()
        {
            var getData = await (from u in con.feeLists
                                 join b in con.trainingInfos on u.tiId equals b.tiId

                                 select new
                                 {
                                     u.fLId,
                                     u.flStatus,
                                     b.tiPrice
                                 }).OrderByDescending(o => o.fLId).ToListAsync();
            return Ok(getData);
        }
    }
}