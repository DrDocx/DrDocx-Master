using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrDocx.API;
using DrDocx.Models;

namespace DrDocx.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldValueGroupController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public FieldValueGroupController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/FieldValueGroup
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FieldValueGroup>>> GetFieldValueGroups()
        {
            return await _context.FieldValueGroups.ToListAsync();
        }

        // GET: api/FieldValueGroup/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FieldValueGroup>> GetFieldValueGroup(int id)
        {
            var fieldValueGroup = await _context.FieldValueGroups.
                Include(fvg => fvg.FieldValues).FirstOrDefaultAsync(fvg => fvg.Id == id);

            if (fieldValueGroup == null)
            {
                return NotFound();
            }

            return fieldValueGroup;
        }

        // PUT: api/FieldValueGroup/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFieldValueGroup(int id, FieldValueGroup fieldValueGroup)
        {
            if (id != fieldValueGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(fieldValueGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FieldValueGroupExists(id))
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

        // POST: api/FieldValueGroup
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<FieldValueGroup>> PostFieldValueGroup(FieldValueGroup fieldValueGroup)
        {
            _context.FieldValueGroups.Add(fieldValueGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFieldValueGroup", new { id = fieldValueGroup.Id }, fieldValueGroup);
        }

        // DELETE: api/FieldValueGroup/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FieldValueGroup>> DeleteFieldValueGroup(int id)
        {
            var fieldValueGroup = await _context.FieldValueGroups.FindAsync(id);
            if (fieldValueGroup == null)
            {
                return NotFound();
            }

            _context.FieldValueGroups.Remove(fieldValueGroup);
            await _context.SaveChangesAsync();

            return fieldValueGroup;
        }
        private bool FieldValueGroupExists(int id)
        {
            return _context.FieldValueGroups.Any(e => e.Id == id);
        }
    }
}
