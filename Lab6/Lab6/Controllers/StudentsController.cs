using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab6.Data;
using Lab6.Models;

namespace Lab6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] // returned when we return list of Students successfully
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // returned when there is an error in processing the request
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]  // returned when student successfully fetched
        [ProducesResponseType(StatusCodes.Status404NotFound)]  //returned when student not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]  // returned when there is an error in processing request
        public async Task<ActionResult<Student>> GetStudent(Guid id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]   // returned when student successfully updated
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // returned when id provided and does not match student
        [ProducesResponseType(StatusCodes.Status404NotFound)]   // returned when student is not in the list
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]  // returned when there is an error in processing the request
        public async Task<IActionResult> PutStudent(Guid id, Student student)
        {
            if (id != student.ID)
            {
                return BadRequest();
            }
            Student learner;
            try
            {
                learner = await _context.Students.FindAsync(id);
                learner.FirstName = student.FirstName;
                learner.LastName = student.LastName;
                learner.Program = student.Program;

                _context.Update(learner);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(learner);
        }

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]    // returned when student was successfully created
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // returned when provided id or student is wrong
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]  // returned when there is an error in processing the request
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.ID }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]  // retuuned when student was deleted successfully
        [ProducesResponseType(StatusCodes.Status404NotFound)]   // returned when student was not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]  // returned when there is an error in processing the request
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status202Accepted);
        }

        private bool StudentExists(Guid id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
