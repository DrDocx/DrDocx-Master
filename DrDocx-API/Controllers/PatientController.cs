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
            var patient = await GetFullPatient(_context, id);

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
        public async Task<ActionResult<Patient>> PutPatient(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest();
            }
            
            PreparePatientEntities(patient);

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

            return await GetFullPatient(_context, patient.Id);
        }

        // POST: api/Patient
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            PreparePatientEntities(patient);
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

        public void PreparePatientEntities(Patient patient)
        {
            _context.Entry(patient).State = patient.Id == 0 ? EntityState.Added : EntityState.Modified;
            foreach (var fvg in patient.FieldValueGroups)
            {
                fvg.Patient = patient;
                _context.Entry(fvg).State = fvg.Id == 0 ? EntityState.Added : EntityState.Modified;
                foreach (var fieldValue in fvg.FieldValues)
                {
                    fieldValue.ParentGroup = fvg;
                    _context.Entry(fieldValue).State = fieldValue.Id == 0 ? EntityState.Added : EntityState.Modified;
                }
            }

            foreach (var trg in patient.ResultGroups)
            {
                trg.Patient = patient;
                _context.Entry(trg).State = trg.Id == 0 ? EntityState.Added : EntityState.Modified;
                foreach (var result in trg.Tests)
                {
                    result.TestResultGroup = trg;
                    _context.Entry(result).State = result.Id == 0 ? EntityState.Added : EntityState.Modified;
                }
            }
        }
        
        // Note that this method takes in a field group id, NOT a field value group id as it creates a new
        // field value group based on the field group provided.
        [HttpPost("{id}/fieldGroup/{fieldGroupId}")]
        public async Task<ActionResult<List<FieldValueGroup>>> AddFieldGroup(int id, int fieldGroupId)
        {
            var patient= await GetFullPatient(_context, id);
            if (patient == null)
                return NotFound("Patient was not found.");
            
            var patientAlreadyHasFieldGroup = patient.FieldValueGroups.Exists(fvg => fvg.FieldGroup.Id == fieldGroupId);
            if (patientAlreadyHasFieldGroup)
                return BadRequest("Patient already has this field group.");
            
            var fieldGroup = await _context.FieldGroups.FindAsync(fieldGroupId);
            if (fieldGroup == null)
            {
                return NotFound("Field group template was not found");
            }

            var fieldValueGroup = new FieldValueGroup
            {
                FieldGroup = fieldGroup,
                Patient = patient
            };
            fieldValueGroup.FieldGroup.Fields.ForEach(field => fieldValueGroup.FieldValues.Add(new FieldValue
            {
                Field = field,
                FieldTextValue = field.DefaultValue,
                ParentGroup = fieldValueGroup,
                
            }));
            _context.FieldValueGroups.Add(fieldValueGroup);
            await _context.SaveChangesAsync();
            
            return patient.FieldValueGroups;
        }

        // TODO: Put this in a helper class
        public static async Task<Patient> GetFullPatient(DatabaseContext context, int id)
        {
            var patient = await context.Patients
                .Include(p => p.FieldValueGroups)
                .ThenInclude(fvg => fvg.FieldValues)
                    .ThenInclude(fv => fv.Field)
                .Include(p => p.FieldValueGroups)
                    .ThenInclude(fvg => fvg.FieldGroup)
                .Include(p => p.ResultGroups)
                    .ThenInclude(trg => trg.Tests)
                .FirstOrDefaultAsync(fg => fg.Id == id);
            foreach (var fvg in patient.FieldValueGroups)
            {
                fvg.FieldValues = fvg.FieldValues.OrderBy(fv => fv.Field.DateCreated).ToList();
            }
            return patient;
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
