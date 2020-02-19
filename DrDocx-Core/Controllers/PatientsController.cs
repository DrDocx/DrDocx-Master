using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using DrDocx.Core;
using DrDocx.Models;
using DrDocx.WordDocEditing;

using System.IO;
using System.Text.Json;

namespace DrDocx.Core.Controllers
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
        [HttpPost] //TODO: BTW I remember deleting one of these things to do something hacky, idk where but you should make sure to put them back... -Nathan
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
            var reportDir = "Patients/" + strippedPatientName + "/";
            var reportTemplatePath = "Templates/report_template.docx";

            System.IO.Directory.CreateDirectory(reportDir);

            await GenerateReport(patient, reportTemplatePath, reportDir);

            return reportDir + "/" + strippedPatientName + ".docx";
        }

        private async Task GenerateReport(Patient patient, string templatePath, string reportDir)
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

            WordAPI report = new WordAPI(templatePath,reportDir + "/" + patient.Name.Replace(" ","-") + ".docx",readOnly: false);
            await report.GenerateReport(patient,reportDir);
            report.Close();
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
