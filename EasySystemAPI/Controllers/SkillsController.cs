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
    public class SkillsController : ControllerBase
    {
        private readonly EasyContext con;
        public SkillsController(EasyContext context)
        {
            con = context;
        }


        [HttpGet("GetSkillTypeList")]
        public async Task<ActionResult> GetSkillTypeList()
        {
            var getData = await con.skillTypes.OrderByDescending(o => o.StId).ToListAsync();
            return Ok(getData);
        }

        [HttpGet("GetSkillList")]
        public async Task<ActionResult> GetSkillList()
        {
            var getData = await con.skillTypes.ToListAsync();
            return Ok(getData);
        }

        [HttpGet("GetMaterialTypeById")]
        public async Task<ActionResult> GetMaterialTypeById(int id)
        {
            var getData = await (from sk in con.skillMaterials
                                 join sm in con.skillTypes on sk.StId equals sm.StId
                                 join u in con.users on sk.CreatedBy equals u.usrId
                                 where sk.StId == id
                                 select new
                                 {
                                     sk.StId,
                                     sk.SmId,
                                     SkillName = sm.StName,
                                     sk.SmTitle,
                                     sk.SmStatus,
                                     sk.SmContent,
                                     CreatedBy = u.usrName + "-" + u.usrCode
                                 }).OrderByDescending(o => o.SmId).ToListAsync();
            return Ok(getData);
            //return await con.userSkills.Where(b => b.usrId == id).ToListAsync();
        }

        [HttpPost("AddSkillMaterial")]
        public async Task<ActionResult<SkillMaterial>> AddSkillMaterial(SkillMaterial data)
        {
            try
            {
                con.skillMaterials.Add(data);
                await con.SaveChangesAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "an error occured. Data not added" });
            }
        }

        [HttpPost("AddSkillMaterialDetail")]
        public async Task<ActionResult> AddSkillMaterialDetail(List<SkillMaterialDetail> data)
        {
            try
            {
                con.skillMaterialDetails.AddRange(data);
                await con.SaveChangesAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "an error occured. Data not added " + ex + "" });
            }
        }

        [HttpDelete("DelSkillMaterial")]
        public async Task<ActionResult> DelSkillMaterial(int id)
        {
            try
            {
                var ChkData = await con.skillMaterialDetails.Where(d => d.SmId == id).ToListAsync();
                if (ChkData.Count > 0)
                {
                    con.skillMaterialDetails.RemoveRange(ChkData);
                }

                var getData = await con.skillMaterials.FindAsync(id);
                if (getData == null)
                {
                    return BadRequest(new { message = "No Record found" });
                }

                con.skillMaterials.Remove(getData);

                await con.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error occured" });
            }
        }

        [HttpGet("EditSkillMaterial")]
        public async Task<ActionResult<SkillMaterial>> EditSkillMaterial(int id)
        {
            return await con.skillMaterials.Where(b => b.SmId == id).FirstOrDefaultAsync();
        }

        [HttpPut("UpdateSkillMaterial")]
        public async Task<IActionResult> UpdateSkillMaterial(int id, SkillMaterial data)
        {
            if (id != data.SmId)
            {
                return BadRequest();
            }
            var getData = await con.skillMaterials.Where(u => u.SmId == id).FirstOrDefaultAsync();
            if (getData != null)
            {
                getData.StId = data.StId;
                getData.SmTitle = data.SmTitle;
                if (data.SmContent != null)
                {
                    getData.SmContent = data.SmContent;
                }

                getData.SmStatus = data.SmStatus;
                con.Entry(getData).State = EntityState.Modified;
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

        [HttpGet("CheckSkillMaterialDetail")]
        public async Task<ActionResult> CheckSkillMaterialDetail(int id)
        {
            var ChkData = await con.skillMaterials.Where(s => s.StId == id && s.SmStatus == true).AnyAsync();
            if (ChkData == true)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("GetSkillMaterial")]
        public async Task<ActionResult> GetSkillMaterial(int id)
        {
            var getData = await con.skillMaterials.Where(s => s.StId == id && s.SmStatus == true).ToListAsync();
            if (getData != null)
            {
                return Ok(getData);
            }
            else
            {
                return BadRequest(new { message = "no record found" });
            }
        }

        [HttpGet("GetSkillMaterialDetail")]
        public async Task<ActionResult> GetSkillMaterialDetail(int id)
        {
            var getData = await con.skillMaterialDetails.Where(s => s.StId == id).ToListAsync();
            if (getData.Count > 0)
            {
                return Ok(getData);
            }
            else
            {
                return BadRequest(new { message = "no record found" });
            }
        }

        [HttpGet("GetSkillMaterialAdmin")]
        public async Task<ActionResult> GetSkillMaterialAdmin(int id)
        {
            var getData = await (from s in con.skillMaterials
                                 join st in con.skillTypes on s.StId equals st.StId
                                 join u in con.users on s.CreatedBy equals u.usrId
                                 where s.SmId == id
                                 select new
                                 {
                                     st.StId,
                                     SkillName = st.StName,
                                     s.SmId,
                                     s.SmTitle,
                                     s.SmContent,
                                     s.SmStatus,
                                     CreatedBy = u.usrName
                                 }).FirstOrDefaultAsync();
            if (getData != null)
            {
                return Ok(getData);
            }
            else
            {
                return BadRequest(new { message = "no record found" });
            }
        }

        [HttpGet("GetSkillMaterialDetailAdmin")]
        public async Task<ActionResult> GetSkillMaterialDetailAdmin(int id)
        {
            var getData = await con.skillMaterialDetails.Where(s => s.SmId == id).ToListAsync();
            if (getData.Count > 0)
            {
                return Ok(getData);
            }
            else
            {
                return BadRequest(new { message = "no record found" });
            }
        }


        [HttpGet("DelMaterialVideo")]
        public async Task<ActionResult> DelMaterialVideo(int id)
        {
            try
            {
                var getData = await con.skillMaterialDetails.FindAsync(id);
                if (getData != null)
                {
                    con.skillMaterialDetails.Remove(getData);
                    con.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }


        [HttpGet("GetSkillType")]
        public async Task<ActionResult<List<SkillType>>> GetSkillType()
        {
            return await con.skillTypes.Where(s => s.StId == s.SubType).ToListAsync();
        }

        [HttpGet("EditSkillType")]
        public async Task<ActionResult<SkillType>> EditSkillType(int id)
        {
            return await con.skillTypes.Where(s => s.StId == id).FirstOrDefaultAsync();
        }

        [HttpPost("UpdateSkillTypeName")]
        public async Task<ActionResult> UpdateSkillTypeName(SkillType data)
        {
            try
            {
                var getData = await con.skillTypes.Where(s => s.StId == data.StId).FirstOrDefaultAsync();
                if (getData != null)
                {
                    getData.StName = data.StName;
                    con.Entry(getData).State = EntityState.Modified;
                    con.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception)
            {

            }
            return BadRequest();
        }

        [HttpGet("SearchForCourse")]
        public List<string> SearchForCourse(string term)
        {
            List<string> AutoCourse;
            AutoCourse = con.skillTypes.Where(s => s.StName.StartsWith(term) || s.StName.Contains(term) && s.StStatus == true).Select(s => s.StName).ToList();
            return AutoCourse;
        }

        [HttpGet("SearchedSkill")]
        public async Task<ActionResult<SkillType>> SearchedSkill(string Course)
        {
            return await con.skillTypes.Where(s => s.StName == Course).FirstOrDefaultAsync();
        }

        [HttpGet("GetSearchCourses")]
        public async Task<ActionResult<SkillType>> GetSearchCourses()
        {
            var getData = await (from qs in con.question_Stories
                                 join s in con.skillTypes on qs.StId equals s.StId
                                 select new
                                 {
                                     s.StId,
                                     s.StName,
                                     s.StDetail,
                                     s.StStatus,
                                     s.StImage,
                                     s.StCoverImage,
                                     s.SubType
                                 }).Distinct().ToListAsync();
                            
            return Ok(getData);
        }
    }
}
