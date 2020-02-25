using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrDocx.API;
using DrDocx.Models;

namespace DrDocx_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldValueController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public FieldValueController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/FieldValue
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FieldValue>>> GetFieldValues()
        {
            return await _context.FieldValues.ToListAsync();
        }

        // GET: api/FieldValue/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FieldValue>> GetFieldValue(int id)
        {
            var fieldValue = await _context.FieldValues.FindAsync(id);

            if (fieldValue == null)
            {
                return NotFound();
            }

            return fieldValue;
        }

        // PUT: api/FieldValue/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFieldValue(int id, FieldValue fieldValue)
        {
            if (id != fieldValue.Id)
            {
                return BadRequest();
            }

            _context.Entry(fieldValue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FieldValueExists(id))
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

        // POST: api/FieldValue
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<FieldValue>> PostFieldValue(FieldValue fieldValue)
        {
            _context.FieldValues.Add(fieldValue);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFieldValue", new { id = fieldValue.Id }, fieldValue);
        }

        // DELETE: api/FieldValue/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FieldValue>> DeleteFieldValue(int id)
        {
            var fieldValue = await _context.FieldValues.FindAsync(id);
            if (fieldValue == null)
            {
                return NotFound();
            }

            _context.FieldValues.Remove(fieldValue);
            await _context.SaveChangesAsync();

            return fieldValue;
        }

        private bool FieldValueExists(int id)
        {
            return _context.FieldValues.Any(e => e.Id == id);
        }
    }
}
