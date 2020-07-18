using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DrDocx.Models;
using DrDocx.WordDocEditing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrDocx.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ReportController(DatabaseContext context)
        {
            _context = context;
        }

        // NOTE: This stupid ass intake has the most absurd bug of all time: it ignores the first form field you pass it
        // so you have to pass it a garbage key/value followed by the file and the name. Fix your shit, Microsoft.
        
        [HttpPost("upload")]
        public async Task<ActionResult<ReportTemplate>> UploadReportTemplate([FromForm] IFormFile templateFile, [FromForm] string templateName)
        {
            if (templateFile == null)
                return BadRequest("The file was not properly uploaded. Please try again.");
            if (templateName == null)
                return BadRequest("No template name was provided. Please provide one and try again.");
            var generatedFileName = GenerateFileName(templateName, Paths.RelativeTemplatesDir, Path.GetExtension(templateFile.FileName));
            if (generatedFileName == null)
                return BadRequest("Could not generate a file name. Please check your templates directory for problems and try again");
            
            var fullFilePath = Path.Combine(Paths.RelativeTemplatesDir,generatedFileName);
            using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
            {
                await templateFile.CopyToAsync(fileStream);
            }
            
            var reportTemplate = new ReportTemplate
            {
                Name = templateName,
                FileName = generatedFileName,
                FilePath = Path.Combine(Paths.WorkingDirectory, fullFilePath)
            };
            _context.ReportTemplates.Add(reportTemplate);

            await _context.SaveChangesAsync();

            return CreatedAtAction("UploadReportTemplate", new { id = reportTemplate.Id }, reportTemplate);
        }

        /// <summary>
        /// Generate a file name for the template based on its display name that isn't already taken in the templates dir
        /// </summary>
        /// <param name="inputName">The name from which to generate a file name</param>
        /// <param name="directory">The directory that the file would go in, used for avoiding a duplicate name.</param>
        /// <param name="fileExtension">The file's extension.</param>
        /// <returns>An unused file name for the template.</returns>
        private static string GenerateFileName(string inputName, string directory, string fileExtension = ".docx")
        {
            const int maxFileNameLength = 32;
            var rgx = new Regex("[^a-zA-Z0-9-]");
            var cutName = inputName.Substring(0, inputName.Length > maxFileNameLength ? maxFileNameLength : inputName.Length);
            var strippedCutName = inputName.Replace(" ", "-");
            var cleanedStrippedCutName = rgx.Replace(strippedCutName, "");
            var cleanedFileName = $"{cleanedStrippedCutName}{fileExtension}";
            var fileNameTaken = System.IO.File.Exists(Path.Combine(directory, cleanedFileName));
            if (!fileNameTaken)
                return cleanedFileName;
            // Otherwise, add numbers to the file name until we get a free one.
            for (var appendedFileNum = 1; appendedFileNum < 256; appendedFileNum++)
            {
                var newFileName = $"{cleanedStrippedCutName}-{appendedFileNum}{fileExtension}";
                if (!System.IO.File.Exists($"{directory}/{newFileName}"))
                    return newFileName;
            }

            return null;
        }
        
        // GET: api/Report
        [HttpGet]
        public IEnumerable<ReportTemplate> Get(int templateId, int patientId)
        {
            return _context.ReportTemplates;
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReportTemplate>> DeleteReport(int id)
        {
            var reportTemplate = await _context.ReportTemplates.FindAsync(id);
            if (reportTemplate == null)
            {
                return NotFound();
            }

            // var templatePath = $"{Paths.RelativeTemplatesDir}/{reportTemplate.FileName}";
            System.IO.File.Delete(reportTemplate.FilePath);

            _context.ReportTemplates.Remove(reportTemplate);
            await _context.SaveChangesAsync();

            return reportTemplate;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTemplate(int id, ReportTemplate template)
        {
            if (id != template.Id)
            {
                return BadRequest();
            }
            
            _context.Entry(template).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemplateExists(id))
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
        
        /// <summary>
        /// Generates and sends a patient report given the specified patient and template.
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpGet("{templateId}/download/{patientId}")]
        public async Task<IActionResult> DownloadPatientReport(int templateId, int patientId)
        {
            if (!_context.Patients.Any(e => e.Id == patientId))
                return NotFound("The specified patient could not be found.");
            if (!_context.ReportTemplates.Any(t => t.Id == templateId))
                return NotFound("The specified template could not be found.");
            var patient = await PatientController.GetFullPatient(_context, patientId);
            var template = await _context.ReportTemplates.FindAsync(templateId);
            
            var content = GeneratePatientReport(patient, template).ToArray();
            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            var datePrefix = DateTime.Now.ToString("yyyy-MM-dd");
            var patientFileName = GenerateFileName($"{datePrefix}-{patient.Name}", Paths.RelativeReportsDir);

            return File(content, contentType, patientFileName);
        }
        
        private MemoryStream GeneratePatientReport(Patient patient, ReportTemplate template)
        {
            var reportTemplatePath = template.FilePath;

            MemoryStream docStream = new MemoryStream(System.IO.File.ReadAllBytes(reportTemplatePath));

            using (var docEditor = new WordAPI(docStream, readOnly: false))
            {
                docEditor.GenerateReport(patient);
            }

            return docStream;
        }

        private bool TemplateExists(int id)
        {
            return _context.ReportTemplates.Any(e => e.Id == id);
        }
    }
}
