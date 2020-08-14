using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace EasySystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnaireController : ControllerBase
    {
        private readonly EasyContext con;
        public QuestionnaireController(EasyContext context)
        {
            con = context;
        }

        [HttpGet("GetSkillType")]
        public async Task<ActionResult<List<SkillType>>> GetSkillType()
        {
            return await con.skillTypes.Where(b => b.StStatus == true).ToListAsync();
        }

        [HttpGet("GetQuestions")]
        public async Task<ActionResult> GetQuestions()
        {
            try
            {
                var getData = await (from q in con.question_Stories
                                     join s in con.skillTypes on q.StId equals s.StId
                                     select new
                                     {
                                         q.qId,
                                         q.qQuestion,
                                         q.qOpt1,
                                         q.qOpt2,
                                         q.qOpt3,
                                         q.qOpt4,
                                         q.qAnswer,
                                         q.qCategory,
                                         q.qStatus,
                                         s.StName,
                                         s.StId
                                     }).OrderByDescending(o => o.qId).ToListAsync();
                return Ok(getData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "There is some error please try agai later" });
            }
        }

        [HttpPost("AddQuestion")]
        public async Task<ActionResult> AddQuestion(Question_Story data)
        {
            try
            {
                con.question_Stories.Add(data);
                await con.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "There is some error please try again later" });
            }
        }

        [HttpGet("EditQuestion")]
        public async Task<ActionResult<Question_Story>> EditQuestion(int id)
        {
            return await con.question_Stories.Where(q => q.qId == id).FirstOrDefaultAsync();
        }

        [HttpPut("UpdateQuestion")]
        public async Task<ActionResult> UpdateQuestion(int id, Question_Story data)
        {
            try
            {
                var getData = await con.question_Stories.FindAsync(id);
                if (getData != null)
                {
                    getData.qQuestion = data.qQuestion;
                    getData.qOpt1 = data.qOpt1;
                    getData.qOpt2 = data.qOpt2;
                    getData.qOpt3 = data.qOpt3;
                    getData.qOpt4 = data.qOpt4;
                    getData.qAnswer = data.qAnswer;
                    getData.qCategory = data.qCategory;
                    getData.qStatus = data.qStatus;
                    getData.StId = data.StId;
                    con.Entry(getData).State = EntityState.Modified;
                    await con.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = "No record found" });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = "There is some error please try again later" });
            }
        }

        [HttpDelete("DelQuestion")]
        public async Task<ActionResult> DelQuestion(int id)
        {
            try
            {
                var getData = await con.question_Stories.FindAsync(id);
                if (getData != null)
                {
                    con.question_Stories.Remove(getData);
                    await con.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "There is some error please try again later" });
            }
            return Ok();
        }

        [HttpGet("GetSkills")]
        public async Task<ActionResult<List<SkillType>>> GetSkills()
        {
            return await con.skillTypes.Where(b => b.StStatus == true && b.StId == b.SubType).ToListAsync();
        }

        [HttpGet("GetTestSkills")]
        public async Task<ActionResult<List<SkillType>>> GetTestSkills(int id)
        {
            return await con.skillTypes.Where(b => b.SubType == id && b.StId != b.SubType).ToListAsync();
        }

        [HttpPost("GetTestQuestions")]
        public async Task<ActionResult<List<Question_Story>>> GetTestQuestions(UserSkills data)
        {
            var chkData = await con.userQuestionnaireResults.Where(u => u.usrId == data.usrId && u.StId == data.StId).ToListAsync();
            if (chkData.Count > 0)
            {
                List<SkillType> sList = new List<SkillType>();
                foreach (var i in chkData)
                {
                    if (i.uqrStatus == "Passed")
                    {
                        return BadRequest(new { message = "You already Pass this test" });
                    }
                    else
                    {
                        if (i.uqrStatus == "Failed")
                        {
                            //GetCategories
                            var GetCategories = (from sa in con.question_Stories
                                                 where !con.userQuestionnaireResults
                                                          .Any(o => o.qCategory == sa.qCategory && o.StId == sa.StId && o.usrId == data.usrId)
                                                 where sa.StId == data.StId

                                                 select new
                                                 {
                                                     sa.qCategory
                                                 }).ToList();

                            if (GetCategories.Count > 0)
                            {
                                foreach (var ca in GetCategories)
                                {
                                    SkillType s = new SkillType();
                                    var ci = ca.qCategory.ToString();
                                    var chkC = sList.Where(w => w.StCategory == ci).Any();
                                    if (chkC == false)
                                    {
                                        s.StCategory = ca.qCategory.ToString();
                                        sList.Add(s);
                                    }
                                }
                            }
                        }
                    }
                }
                if (sList.Count > 0)
                {
                    foreach (var c in sList)
                    {
                        int catId = int.Parse(c.StCategory);
                        return await con.question_Stories.Where(b => b.StId == data.StId && b.qCategory == catId && b.qStatus == true).ToListAsync();
                    }
                }
                else
                {
                    return await con.question_Stories.Where(b => b.StId == data.StId && b.qCategory == 1 && b.qStatus == true).ToListAsync();
                }
            }
            else
            {
                return await con.question_Stories.Where(b => b.StId == data.StId && b.qCategory == 1 && b.qStatus == true).ToListAsync();
            }
            return Ok();
            //
        }

        [HttpPost("TestVerification")]
        public async Task<ActionResult<List<Question_Story>>> TestVerification(Question_Story data)
        {
            return await con.question_Stories.Where(b => b.StId == data.StId && b.qCategory == data.qCategory && b.qStatus == true).ToListAsync();
        }

        [HttpPost("AddTestResult")]
        public async Task<ActionResult> AddTestResult(TestVM data)
        {
            try
            {
                UserQuestionnaireResult q = new UserQuestionnaireResult();
                q.StId = data.StId;
                q.uqrDate = data.Date;
                q.usrId = data.usrId;
                q.uqrStatus = data.uqrStatus;
                q.qCategory = data.qCategory;
                con.userQuestionnaireResults.Add(q);
                await con.SaveChangesAsync();
                if (data.uqrStatus == "Passed")
                {
                    UserCertificate cer = new UserCertificate();
                    cer.StId = data.StId;
                    cer.ucDate = data.Date;
                    cer.ucNumber = data.ucNumber;
                    cer.usrId = data.usrId;
                    cer.uqrId = q.uqrId;
                    con.userCertificates.Add(cer);
                    await con.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Result not updated" });
            }
        }

        [HttpGet("CheckNumber")]
        public async Task<ActionResult> CheckNumber(string No)
        {
            var CheckNo = await con.userCertificates.Where(c => c.ucNumber == No).AnyAsync();
            if (CheckNo == false)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Number Already exist" });
            }
        }

        [HttpGet("GetMyCertificateData")]
        public async Task<ActionResult> GetMyCertificateData(int id)
        {
            var getData = await (from q in con.userQuestionnaireResults
                                 join c in con.userCertificates on q.uqrId equals c.uqrId
                                 join s in con.skillTypes on q.StId equals s.StId
                                 where q.usrId == id
                                 select new
                                 {
                                     s.StName,
                                     q.uqrDate,
                                     c.ucNumber,
                                     c.ucId,
                                     q.uqrStatus,
                                 }).ToListAsync();

            return Ok(getData);
        }
    }
}
