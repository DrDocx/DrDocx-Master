using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        public async Task<ActionResult<IEnumerable<FieldGroup>>> GetFieldGroups(string includeFields, string isDefault)
        {
            var fieldGroups = _context.FieldGroups.Where(fg => !fg.IsArchived);
            if (includeFields != null && includeFields == "1")
            {
                fieldGroups = fieldGroups.Include(fg => fg.Fields);
            }

            if (isDefault != null && isDefault == "1")
            {
                fieldGroups = fieldGroups.Where(fg => fg.IsDefaultGroup);
            }
            return await fieldGroups.ToListAsync();
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

            fieldGroup.Fields = fieldGroup.Fields.Where(f => !f.IsArchived).ToList();
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
            var fieldGroup = await GetFullFieldGroup(id);
            if (fieldGroup == null)
            {
                return NotFound();
            }

            var associatedValueGroupCount = _context.FieldValueGroups.Count(fvg => fvg.FieldGroupId == fieldGroup.Id);
            if (associatedValueGroupCount > 0)
            {
                fieldGroup.IsArchived = true;
                _context.Entry(fieldGroup).State = EntityState.Modified;
            }
            else
            {
                _context.FieldGroups.Remove(fieldGroup);
            }

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
        public async Task<ActionResult<FieldGroup>> RemoveField(int id, int fieldId)
        {
            var fieldGroup = await GetFullFieldGroup(id);
            if (fieldGroup == null)
                return NotFound("Field group not found.");
            
            var field = await _context.Fields.FindAsync(fieldId);
            if (field == null)
                return NotFound("Could not find the field you tried to remove.");

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

            return fieldGroup;
        }
        private bool FieldGroupExists(int id)
        {
            return _context.FieldGroups.Any(e => e.Id == id);
        }

		// GET: api/FieldGroup/download
		// Downloads selected field groups to exportedFieldGroups.json
		[HttpGet("download")]
		public async Task<IActionResult> DownloadFieldGroups([FromBody] int[] fieldGroupIds)
		{
			var fieldGroups = _context.FieldGroups
				.Include(fg => fg.Fields)
				.Where(fg => !fg.IsArchived);
			
			if(fieldGroupIds.Length > 0)
			{
				fieldGroups = fieldGroups.Where(fg => fieldGroupIds.Contains(fg.Id));
			}

            var fieldGroupsList = await fieldGroups.ToListAsync();

            foreach (var fieldGroup in fieldGroupsList)
            {
                fieldGroup.Id = 0;
                foreach (var field in fieldGroup.Fields)
                {
                    field.Id = 0;
                    field.FieldGroupId = 0;
                }
            }

			string fieldGroupsString = JsonSerializer.Serialize(fieldGroupsList);

            byte[] content = Encoding.UTF8.GetBytes(fieldGroupsString);
            const string contentType = "application/json";
			const string fileName = "exportedFieldGroups.dr";

            return File(content, contentType, fileName);
		}

        // POST: api/FieldGroup/upload
        // Uploads field groups from JSON file.
        [HttpPost("upload")]
        public async Task<IActionResult> UploadReportTemplate([FromForm] IFormFile importedFieldGroupsJson, [FromForm] string templateName)
        {
            string importedFieldGroupsJsonString;
            using (var reader = new StreamReader(importedFieldGroupsJson.OpenReadStream()))
            {
                importedFieldGroupsJsonString = reader.ReadLine();
            }
            List<FieldGroup> fieldGroups = JsonSerializer.Deserialize<List<FieldGroup>>(importedFieldGroupsJsonString);

            foreach (var fieldGroup in fieldGroups)
            {
                _context.FieldGroups.Add(fieldGroup);
                foreach (var field in fieldGroup.Fields)
                {
                    field.FieldGroup = fieldGroup;
                    _context.Fields.Add(field);
                }
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
