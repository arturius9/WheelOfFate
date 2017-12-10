using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SupportWheelOfFateWebApi.Data;
using SupportWheelOfFateWebApi.Business_Logic;

namespace SupportWheelOfFateWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BAUsController : Controller
    {
        private readonly IBusinessService _context;

        public BAUsController(IBusinessService context)
        {
            _context = context;
        }

        //// GET: api/BAUs
        [HttpGet]
        public async Task<IActionResult> GetBAU()
        {
            var BAU = await Task.Factory.StartNew(()=> _context.GetAllBAUs());

            if (BAU == null)
            {
                return NotFound();
            }

            return Ok(BAU);
        }

        // GET: api/BAUs/2017-11-05
        [HttpGet("{date}")]
        public async Task<IActionResult> GetBAU([FromRoute] DateTime date)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var BAU = await Task.Factory.StartNew(()=> _context.GetBAU(date));

            if (BAU == null)
            {
                return NotFound();
            }

            return Ok(BAU);
        }

        //// PUT: api/BAUs/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutBAU([FromRoute] int id, [FromBody] BAU bAU)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != bAU.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(bAU).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BAUExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/BAUs
        //[HttpPost]
        //public async Task<IActionResult> PostBAU([FromBody] BAU bAU)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.BAU.Add(bAU);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetBAU", new { id = bAU.Id }, bAU);
        //}

        // DELETE: api/BAUs/5
        [HttpDelete()]
        public IActionResult DeleteBAU()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var numberOfBAUsDeleted = _context.DeleteAllBAUs();
 
            return Ok(numberOfBAUsDeleted);
        }

        //private bool BAUExists(int id)
        //{
        //    return _context.BAU.Any(e => e.Id == id);
        //}
    }
}