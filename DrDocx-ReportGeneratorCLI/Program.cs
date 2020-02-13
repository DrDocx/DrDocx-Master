using System;
using System.IO;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Packaging;

using Microcharts;
using Entry = Microcharts.Entry;

using SkiaSharp;

using DrDocx.Models;
using static DrDocx.WordDocEditing.WordAPI;
using static DrDocx.WordDocEditing.ChartAPI;

namespace DrDocx.ReportGenCLI
{
	static class Program
	{
		static void Main(string[] args)
		{
			
			Random random = new Random();
			List<TestResult> results = new List<TestResult>();
			for (int i = 0; i < 21; i++)
			{
				double r = random.NextDouble() * 6 - 3;
				results.Add(new TestResult(){
					RelatedTest = new Test(){Name = "LD" + i.ToString()},
					ZScore = (int)(random.NextDouble()*6 - 3),
					Percentile = (int)(random.NextDouble()*100)
					});
			}

			List<TestResultGroup> resultGroups = new List<TestResultGroup>(new TestResultGroup[]{
				new TestResultGroup(){
					TestGroupInfo = new TestGroup(){Name = "Symptom Checklist - 90 - Revised"},
					Tests = results
				}
				});

			Patient johnDoe = new Patient(){
				Name = "John Doe",
				PreferredName = "Johnny",
				DateOfBirth = new DateTime(620243894905389400),
				DateOfTesting = new DateTime(628243894905389400),
				MedicalRecordNumber = 123456,
				Address = "4831 Washington Avenue",
				Medications = "Flovent",
				ResultGroups = resultGroups
			};

			Patient patient = johnDoe;

			string templatePath = @"templates\report_template.docx";
			string newfilePath = @"generated_reports\" + patient.Name + ".docx";

			//GenerateReport(patient,templatePath,newfilePath);
			MakePatientPercentileChart(patient,patient.Name + "1");
			Console.WriteLine("Report generated at " + newfilePath);

		}
	}
}