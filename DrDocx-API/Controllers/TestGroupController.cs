using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TestGroupController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public TestGroupController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/TestGroup
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestGroup>>> GetTestGroups()
        {
            return await _context.TestGroups.ToListAsync();
        }

        // GET: api/TestGroup/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TestGroup>> GetTestGroup(int id)
        {
            var testGroup = await _context.TestGroups.FindAsync(id);

            if (testGroup == null)
            {
                return NotFound();
            }

            return testGroup;
        }

        // PUT: api/TestGroup/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTestGroup(int id, TestGroup testGroup)
        {
            if (id != testGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(testGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestGroupExists(id))
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

        // POST: api/TestGroup
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TestGroup>> PostTestGroup(TestGroup testGroup)
        {
            _context.TestGroups.Add(testGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTestGroup", new { id = testGroup.Id }, testGroup);
        }

        // DELETE: api/TestGroup/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TestGroup>> DeleteTestGroup(int id)
        {
            var testGroup = await _context.TestGroups.FindAsync(id);
            if (testGroup == null)
            {
                return NotFound();
            }

            _context.TestGroups.Remove(testGroup);
            await _context.SaveChangesAsync();

            return testGroup;
        }

        [HttpPut("{id}/test/{testId}")]
        public async Task<ActionResult<TestGroup>> AddTestToTestGroup(int id, int testId)
        {
            var testGroup = await _context.TestGroups.FindAsync(id);
            if (testGroup == null)
            {
                return NotFound();
            }

            var test = await _context.Tests.FindAsync(testId);
            if (test == null)
            {
                return NotFound();
            }

            var joinRecord = new TestGroupTest
            {
                TestGroupId = id,
                TestId = testId
            };
            _context.TestGroupTests.Add(joinRecord);
            await _context.SaveChangesAsync();
            return testGroup;
        }

        /*private async Task<TestGroup> GetFullTestGroup(int id)
        {
            throw new NotImplementedException();
        }*/
        private bool TestGroupExists(int id)
        {
            return _context.TestGroups.Any(e => e.Id == id);
        }
    }
}
