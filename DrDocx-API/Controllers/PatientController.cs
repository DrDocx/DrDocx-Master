using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrDocx.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrDocx.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public PatientController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Patient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientInfo>>> GetPatients()
        {
            var patientsQuery = from patient in _context.Patients 
                select new PatientInfo
                {
                    Name = patient.Name,
                    DateModified = patient.DateModified,
                    Id = patient.Id
                };

            return await patientsQuery.ToListAsync();
        }

        // GET: api/Patient/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        // PUT: api/Patient/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest();
            }

            patient.DateModified = DateTime.UtcNow;
            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
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

        // POST: api/Patient
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPatient", new { id = patient.Id }, patient);
        }

        // DELETE: api/Patient/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Patient>> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return patient;
        }

        // Note that this method takes in a field group id, NOT a field value group id as it creates a new
        // field value group based on the field group provided.
        [HttpPost("{id}/fieldValueGroup/{fieldGroupId}")]
        public async Task<ActionResult<FieldValueGroup>> AddFieldGroup(int id, int fieldGroupId)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();
            
            var patientAlreadyHasFieldGroup = patient.FieldValueGroups.Exists(fvg => fvg.FieldGroup.Id == fieldGroupId);
            if (patientAlreadyHasFieldGroup)
                return BadRequest("Patient already has this field group.");
            
            var fieldGroup = await _context.FieldGroups.FindAsync(fieldGroupId);
            if (fieldGroup == null)
            {
                return NotFound("Field group template was not found");
            }

            var fieldValueGroup = new FieldValueGroup(fieldGroup) { Patient = patient };
            _context.FieldValueGroups.Add(fieldValueGroup);
            await _context.SaveChangesAsync();
            
            return fieldValueGroup;
        }
        
        
        [HttpDelete("{id}/fieldValueGroup/{fieldValueGroupId}")]
        public async Task<ActionResult<FieldValueGroup>> DeleteFieldGroup(int id, int fieldValueGroupId)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();
            
            var patientFvg = patient.FieldValueGroups.FirstOrDefault(fvg => fvg.Id == fieldValueGroupId);
            if (patientFvg == null)
                return BadRequest("Patient does not have this field value group.");

            _context.FieldValueGroups.Remove(patientFvg);
            await _context.SaveChangesAsync();
            
            return patientFvg;
        }
        
        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
