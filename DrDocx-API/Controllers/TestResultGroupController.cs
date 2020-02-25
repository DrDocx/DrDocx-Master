using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrDocx.API;
using DrDocx.Models;

namespace DrDocx_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestResultGroupController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public TestResultGroupController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/TestResultGroup
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestResultGroup>>> GetTestResultGroups()
        {
            return await _context.TestResultGroups.ToListAsync();
        }

        // GET: api/TestResultGroup/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TestResultGroup>> GetTestResultGroup(int id)
        {
            var testResultGroup = await _context.TestResultGroups.FindAsync(id);

            if (testResultGroup == null)
            {
                return NotFound();
            }

            return testResultGroup;
        }

        // PUT: api/TestResultGroup/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTestResultGroup(int id, TestResultGroup testResultGroup)
        {
            if (id != testResultGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(testResultGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestResultGroupExists(id))
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

        // POST: api/TestResultGroup
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TestResultGroup>> PostTestResultGroup(TestResultGroup testResultGroup)
        {
            _context.TestResultGroups.Add(testResultGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTestResultGroup", new { id = testResultGroup.Id }, testResultGroup);
        }

        // DELETE: api/TestResultGroup/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TestResultGroup>> DeleteTestResultGroup(int id)
        {
            var testResultGroup = await _context.TestResultGroups.FindAsync(id);
            if (testResultGroup == null)
            {
                return NotFound();
            }

            _context.TestResultGroups.Remove(testResultGroup);
            await _context.SaveChangesAsync();

            return testResultGroup;
        }

        private bool TestResultGroupExists(int id)
        {
            return _context.TestResultGroups.Any(e => e.Id == id);
        }
    }
}
