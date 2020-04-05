using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrDocx.API;
using DrDocx.Models;
using DrDocx.Models.Helpers;
using DrDocx.API.Helpers;

namespace DrDocx.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public FieldController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Field
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Field>>> GetFields()
        {
            return await _context.Fields.ToListAsync();
        }

        // GET: api/Field/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Field>> GetField(int id)
        {
            var @field = await _context.Fields.FindAsync(id);

            if (@field == null)
            {
                return NotFound();
            }

            return @field;
        }

        // PUT: api/Field/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutField(int id, Field @field)
        {
            if (id != field.Id)
            {
                return BadRequest("The request was incorrectly formed: field id and field object's id do not match.");
            }

            _context.Entry(field).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordExists.FieldExists(_context, id))
                {
                    return NotFound("The field you tried to update could not be found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Field
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Field>> PostField(Field field)
        {
            if (!FieldHelper.FieldTypeIsValid(field.Type))
                return BadRequest("Invalid field type provided.");
            if (!RecordExists.FieldGroupExists(_context, field.FieldGroupId))
                return BadRequest("Field group that you're adding this field to could not be found");

            _context.Fields.Add(field);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetField", new { id = @field.Id }, @field);
        }

        // DELETE: api/Field/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Field>> DeleteField(int id)
        {
            var field = await _context.Fields.FindAsync(id);
            if (field == null)
            {
                return NotFound("The field you tried to delete could not be found.");
            }

            var associatedValueCount = _context.FieldValues.Count(fv => fv.FieldId == field.Id);
            if (associatedValueCount > 0)
            {
                field.IsArchived = true;
                _context.Entry(field).State = EntityState.Modified;
            }
            else
            {
                _context.Fields.Remove(field);
            }

            await _context.SaveChangesAsync();

            return field;
        }
    }
}
