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
    public class MyHelpingMaterialController : ControllerBase
    {
        private readonly EasyContext con;
        public MyHelpingMaterialController(EasyContext context)
        {
            con = context;
        }

        [HttpGet("GetAllHelpingMaterial")]
        public async Task<ActionResult> GetHelpingMaterial()
        {
            try
            {
                var getHelpingMaterial = await con.userHelpingMaterial.ToListAsync();
                return Ok(getHelpingMaterial);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occured while getting the Helping material data. Please try again later. " + ex + "" });
            }

        }


        [HttpGet("GetMyHelpingMaterial")]
        public async Task<ActionResult<List<UserHelpingMaterial>>> GetMyHelpingMaterial()
        {
            try
            {
                var getData = await con.userHelpingMaterial.Where(b => b.umsStatus == true).ToListAsync();
                return Ok(getData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occured while getting the Helping material data. Please try again later. " + ex + "" });
            }

        }


        [HttpPost("AddHelpingMaterial")]
        public async Task<ActionResult> AddHelpingMaterial(UserHelpingMaterial hm)
        {
            try
            {
                hm.umsCreatedBy = 1;
                hm.umsCreatedDate = DateTime.Now;
                con.userHelpingMaterial.Add(hm);
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
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occured while getting the Helping material data. Please try again later. " + ex + "" });
            }

        }

        [HttpDelete("DelHelpingMaterial")]
        public async Task<ActionResult> DelHelpingMaterial(int id)
        {
            var getData = await con.userHelpingMaterial.FindAsync(id);
            if (getData == null)
            {
                return NotFound();
            }

            con.userHelpingMaterial.Remove(getData);
            await con.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("GetHelpingMaterial")]
        public async Task<ActionResult<UserHelpingMaterial>> GetHelpingMaterial(int id)
        {
            try
            {
                var GetData = await con.userHelpingMaterial.FindAsync(id);

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
                return BadRequest(new { message = "An error occured while getting the helping material data. Please try again later." });
            }
        }

        [HttpPut("UpdateHelpingMaterial")]
        public async Task<IActionResult> UpdateHelpingMaterial(int id, UserHelpingMaterial hm)
        {
            try
            {
                if (id != hm.umsId)
                {
                    return BadRequest(new { message = "Data not updated" });
                }
                var getData = await con.userHelpingMaterial.FindAsync(id);
                if (getData != null)
                {
                    getData.umsTitle = hm.umsTitle;
                    getData.umsType = hm.umsType;
                    if (hm.umsPath != null)
                    {
                        getData.umsPath = hm.umsPath;
                    }
                    getData.umsFontName = hm.umsFontName;
                    getData.umsFontColour = hm.umsFontColour;
                    getData.umsFontSize = hm.umsFontSize;
                    getData.umsFontAlignment = hm.umsFontAlignment;
                    getData.umsFor = hm.umsFor;
                    getData.umsStatus = hm.umsStatus;
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

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occured while updating Info. Please try again later " + ex + "" });
            }

        }
    }
}