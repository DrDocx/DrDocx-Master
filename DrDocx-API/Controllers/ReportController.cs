using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrDocx.Models;
using DrDocx.WordDocEditing;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/Report
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new [] { "value1", "value2" };
        }

        
        [HttpGet("download/{patientId}")]
        public async Task<IActionResult> DownloadPatientReport(int patientId)
        {
            if (!_context.Patients.Any(e => e.Id == patientId))
            {
                return NotFound();
            }
            var patient = await _context.Patients.FindAsync(patientId);
            var link = await GeneratePatientReport(patient);
            var net = new System.Net.WebClient();
            var data = net.DownloadData(link);
            var content = new System.IO.MemoryStream(data);
            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var fileName = $"Patient-{patient.Name}-Report.docx";
            return File(content, contentType, fileName);
        }
        
        private async Task<string> GeneratePatientReport(Patient patient)
        {
            // Create local report directory
            var strippedPatientName = patient.Name.Replace(" ", "-");
            var reportDir = "Patients/" + strippedPatientName + "/";
            var reportTemplatePath = "Templates/report_template.docx";

            Directory.CreateDirectory(reportDir);

            await GenerateReport(patient, reportTemplatePath, reportDir);

            return reportDir + "/" + strippedPatientName + ".docx";
        }

        private async Task GenerateReport(Patient patient, string templatePath, string reportDir)
        {
            var report = new WordAPI(templatePath,reportDir + "/" + patient.Name.Replace(" ","-") + ".docx",readOnly: false);
            report.GenerateReport(patient,reportDir);
            report.Close();
        }
    }
}
