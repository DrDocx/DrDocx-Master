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
    public class FieldGroupController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public FieldGroupController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/FieldGroup
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FieldGroup>>> GetFieldGroups()
        {
            return await _context.FieldGroups.ToListAsync();
        }

        // GET: api/FieldGroup/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FieldGroup>> GetFieldGroup(int id)
        {
            var fieldGroup = await GetFullFieldGroup(id);

            if (fieldGroup == null)
            {
                return NotFound();
            }

            return fieldGroup;
        }

        // PUT: api/FieldGroup/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFieldGroup(int id, FieldGroup fieldGroup)
        {
            if (id != fieldGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(fieldGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FieldGroupExists(id))
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

        // POST: api/FieldGroup
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<FieldGroup>> PostFieldGroup(FieldGroup fieldGroup)
        {
            _context.FieldGroups.Add(fieldGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFieldGroup", new { id = fieldGroup.Id }, fieldGroup);
        }

        // DELETE: api/FieldGroup/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FieldGroup>> DeleteFieldGroup(int id)
        {
            var fieldGroup = await _context.FieldGroups.FindAsync(id);
            if (fieldGroup == null)
            {
                return NotFound();
            }

            _context.FieldGroups.Remove(fieldGroup);
            await _context.SaveChangesAsync();

            return fieldGroup;
        }
        
        [HttpPost("{id}/field")]
        public async Task<ActionResult<FieldGroup>> AddField(int id, Field field)
        {
            var fieldGroup = await GetFullFieldGroup(id);
            if (fieldGroup == null)
                return NotFound("Field group not found.");

            if (fieldGroup.Fields.Exists(f => f.MatchText == field.MatchText))
                return BadRequest("Field with that match text already exists in this group.");

            field.FieldGroup = fieldGroup;
            _context.Fields.Add(field);
            await _context.SaveChangesAsync();

            return fieldGroup;
        }

        private Task<FieldGroup> GetFullFieldGroup(int id)
        {
            return _context.FieldGroups
                .Include(fg => fg.Fields)
                .FirstOrDefaultAsync(fg => fg.Id == id);
        }

        [HttpDelete("{id}/field/{fieldId}")]
        public async Task<ActionResult<FieldGroup>> AddField(int id, int fieldId)
        {
            var fieldGroup = await _context.FieldGroups.FindAsync(id);
            if (fieldGroup == null)
                return NotFound("Field group not found.");
            
            var field = await _context.Fields.FindAsync(fieldId);
            if (field == null)
                return NotFound("Could not find the field you tried to remove.");

            _context.Fields.Remove(field);
            await _context.SaveChangesAsync();

            return fieldGroup;
        }
        private bool FieldGroupExists(int id)
        {
            return _context.FieldGroups.Any(e => e.Id == id);
        }
    }
}
