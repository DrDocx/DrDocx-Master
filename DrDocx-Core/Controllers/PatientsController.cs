using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrDocx_Core;
using DrDocxModels;
using System.IO;
using System.Text.Json;

namespace DrDocx_Core.Controllers
{
    public struct PatientViewModel
    {
        public Patient Patient;
        public List<TestGroup> TestGroups;
        public List<TestGroupTest> TestGroupTests;
        public List<TestResult> TestResults;
        public List<Test> Tests;
        public List<TestResultGroup> TestResultGroups;
    }

    public class PatientsController : Controller
    {
        private readonly DatabaseContext _context;

        public PatientsController(DatabaseContext context)
        {
            _context = context;
        }

        // POST: Patients/AddTest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTest(int patientId, int testResultGroupId, int testId, [Bind("RawScore,ScaledScore,ZScore,Percentile,ID")] TestResult testResult)
        {

            var testResultGroup = await _context.TestResultGroups.FindAsync(testResultGroupId);
            var test = await _context.Tests.FindAsync(testId);

            testResult.RelatedTest = test;
            if (testResultGroup.Tests == null) testResultGroup.Tests = new List<TestResult>();
            testResultGroup.Tests.Add(testResult);

            _context.Add(testResult);
            await _context.SaveChangesAsync();

            return Redirect("Edit/" + patientId);
        }

        // POST: Patients/AddTest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTest(int patientId, int testResultGroupId, int testResultId)
        {

            var testResultGroup = await _context.TestResultGroups.FindAsync(testResultGroupId);
            var testResult = await _context.TestResults.FindAsync(testResultId);

            testResultGroup.Tests.Remove(testResult);
            _context.Remove(testResult);
            await _context.SaveChangesAsync();

            return Redirect("Edit/" + patientId);
        }



        // POST: TestResultGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] //TODO: BTW I remember deleting one of these things to do something hacky, idk where but you should make sure to put them back... -Nathan
        public async Task<IActionResult> AddTestGroup(int testGroupId, int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            TestResultGroup trg = new TestResultGroup();
            trg.Tests = new List<TestResult>();
            trg.TestGroupInfo = await _context.TestGroups.FindAsync(testGroupId);
            if(patient.ResultGroups == null) patient.ResultGroups = new List<TestResultGroup>();
            patient.ResultGroups.Add(trg);

            if (ModelState.IsValid)
            {
                _context.Add(trg);
                await _context.SaveChangesAsync();
            }
            return Redirect("Edit/" + patientId);
        }



        // POST: TestResultGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] //TODO: BTW I remember deleting one of these things to do something hacky, idk where but you should make sure to put them back... -Nathan
        public async Task<IActionResult> RemoveTestGroup(int testResultGroupId, int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            var testResultGroup = await _context.TestResultGroups.FindAsync(testResultGroupId);
            var v = new PatientViewModel
            {
                Patient = patient,
                Tests = await _context.Tests.ToListAsync(),
                TestGroupTests = await _context.TestGroupTests.ToListAsync(),
                TestGroups = await _context.TestGroups.ToListAsync(),
                TestResultGroups = await _context.TestResultGroups.ToListAsync(),
                TestResults = await _context.TestResults.ToListAsync(),
            };
            if (testResultGroup.Tests != null)
            {
                foreach(var testResult in testResultGroup.Tests)
                {
                    _context.Remove(testResult);
                }
            }
            _context.Remove(testResultGroup);
            await _context.SaveChangesAsync();

            return Redirect("Edit/" + patientId);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Diagnosis,Name,PreferredName,Address,Medications,DateOfBirth,DateOfTesting,Notes,MedicalRecordNumber,AgeAtTesting,Id")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return Redirect("Edit/" + patient.Id);
            }
            return View(patient);
        }


        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(new PatientViewModel
            {
                Patient = patient,
                Tests = await _context.Tests.ToListAsync(),
                TestGroupTests = await _context.TestGroupTests.ToListAsync(),
                TestGroups = await _context.TestGroups.ToListAsync(),
                TestResultGroups = await _context.TestResultGroups.ToListAsync(),
                TestResults = await _context.TestResults.ToListAsync(),
            });
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Diagnosis,Name,PreferredName,Address,Medications,DateOfBirth,DateOfTesting,Notes,MedicalRecordNumber,AgeAtTesting,Id")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(new PatientViewModel { Patient = patient,
                Tests = await _context.Tests.ToListAsync(),
                TestGroups = await _context.TestGroups.ToListAsync(),
                TestGroupTests = await _context.TestGroupTests.ToListAsync(),
                TestResultGroups = await _context.TestResultGroups.ToListAsync(),
                TestResults = await _context.TestResults.ToListAsync(),
            });
        }

        [HttpGet]
        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }

        [HttpGet]
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
            var workingDir = Directory.CreateDirectory(@"./tmp/reports/patient-" + strippedPatientName);
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "/DrDocx-Core/DrDocx-Core";
            var reportGenDirectory = projectDirectory + "/report-gen";
            var reportTemplatePath = reportGenDirectory + "/Report_Template.dotx";
            var reportFileName = "Patient-" + strippedPatientName;
            var reportPath = workingDir.Name + reportFileName;
            var reportStaticPath = projectDirectory + "/wwwroot/reports" + reportFileName;
            var visualizationsDirectory = reportGenDirectory + "/visualizations";

            await Task.WhenAll(GenerateReportSansVisuals(patient, reportTemplatePath, reportPath), GenerateTestVisualizations(patient, workingDir, reportGenDirectory, visualizationsDirectory));
            await CombineReportAndVisualizations(reportPath, visualizationsDirectory);
            bool readyToDelete = await ServeGeneratedReportStatically(reportPath, reportStaticPath);
            //workingDir.Delete(readyToDelete);
            return reportStaticPath;
        }

        private async Task GenerateReportSansVisuals(Patient patient, string templatePath, string reportPath)
        {
            var v = new PatientViewModel
            {
                Patient = patient,
                Tests = await _context.Tests.ToListAsync(),
                TestGroupTests = await _context.TestGroupTests.ToListAsync(),
                TestGroups = await _context.TestGroups.ToListAsync(),
                TestResultGroups = await _context.TestResultGroups.ToListAsync(),
                TestResults = await _context.TestResults.ToListAsync(),
            };
            var testGroups = patient.ResultGroups;
            Dictionary<string, string> templateReplacements = new Dictionary<string, string>
            {
                { "NAME", patient.Name },
                { "PREFERRED_NAME", patient.PreferredName },
                { "MEDICATIONS", patient.Medications },
                { "ADDRESS", patient.Address },
                { "MEDICAL_RECORD_NUMBER", patient.MedicalRecordNumber.ToString() },
                { "AGE_AT_TESTING", "19" }, // Hardcoded as calculation method does not yet exist
                { "TEST_DATE", patient.DateOfTesting.ToString() },
                { "DOB", patient.DateOfBirth.ToString() }
            };

            await ReportGen.ReportGen.GenerateReport(patient, templatePath, reportPath, templateReplacements, testGroups);
        }

        private async Task GenerateTestVisualizations(Patient patient, DirectoryInfo tmpDir, string reportGenDirectory, string visualizationsDir)
        {
            var v = new PatientViewModel
            {
                Patient = patient,
                Tests = await _context.Tests.ToListAsync(),
                TestGroupTests = await _context.TestGroupTests.ToListAsync(),
                TestGroups = await _context.TestGroups.ToListAsync(),
                TestResultGroups = await _context.TestResultGroups.ToListAsync(),
                TestResults = await _context.TestResults.ToListAsync(),
            };
            var resultGroups = patient.ResultGroups;

            var trgDict = new Dictionary<string, List<TestResult>>();
            foreach (var trGroup in resultGroups)
            {
                trgDict.Add(trGroup.TestGroupInfo.Name, trGroup.Tests);
            }
            var serializeOptions = new JsonSerializerOptions { MaxDepth = 10 };
            var output = JsonSerializer.Serialize<Dictionary<string, List<TestResult>>>(trgDict, serializeOptions);
            var resultJsonPath = tmpDir.FullName + "/test-result-data.json";
            System.IO.File.WriteAllText(resultJsonPath, output);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "python.exe";
            startInfo.WorkingDirectory = reportGenDirectory;
            startInfo.Arguments = $"chartGen.py {resultJsonPath} {visualizationsDir}";
            process.StartInfo = startInfo;
            process.Start();
        }

        private async Task CombineReportAndVisualizations(string reportSansVisualsPath, string visualizationDir)
        {
            var imagesInVisualizationDir = Directory.GetFiles(visualizationDir, "*.png");
            await ReportGen.ReportGen.CombineReportAndVisualizations(reportSansVisualsPath, imagesInVisualizationDir);
        }

        private async Task<bool> ServeGeneratedReportStatically(string reportGenPath, string reportStaticPath)
        {
            if (System.IO.File.Exists(reportStaticPath))
            {
                System.IO.File.Delete(reportStaticPath);
            }
            System.IO.File.Copy(reportGenPath, reportStaticPath);
            
            return true;
        }
    }
}
