using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrDocx_Core;
using DrDocxModels;

namespace DrDocx_Core.Controllers
{
    public class TestGroupsController : Controller
    {
        private readonly DatabaseContext _context;

        public TestGroupsController(DatabaseContext context)
        {
            _context = context;
        }

        // POST: TestGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Id")] TestGroup testGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testGroup);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }


        // POST: TestGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTest(int groupId, int testId)
        {
            var testGroupTest = new TestGroupTest { TestGroupId=groupId, TestId=testId };
            _context.Add(testGroupTest);
            await _context.SaveChangesAsync();
          
            return RedirectToAction("Index", "Home");
        }


        // POST: TestGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTest(int groupId, int testId)
        {
            var testGroup = await _context.TestGroups.FindAsync(groupId);
            var testGroupTest = await _context.TestGroupTests.FindAsync(groupId, testId);
            _context.Remove(testGroupTest);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // POST: TestGroups/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var testGroup = await _context.TestGroups.FindAsync(id);
            _context.TestGroups.Remove(testGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool TestGroupExists(int id)
        {
            return _context.TestGroups.Any(e => e.Id == id);
        }
    }
}
