using DotnetPgDemo.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetPgDemo.Api.Controllers
{
    [Route("api/[controller]")] //localhost:5065/api/people
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly AppDbcontext _context;
        public PeopleController(AppDbcontext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson(Person person)
        {
            try
            {
                _context.People.AddAsync(person);
                await _context.SaveChangesAsync();

                return CreatedAtRoute("GetPersonById", new { id = person.Id }, person); //201 created
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var people =  await _context.People.ToListAsync();
                return Ok(people); //returns status code 200
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("{id}", Name = "GetPersonById")]
        public async Task<IActionResult> GetPersonById(int id)
        {
            try
            {
                var person = await _context.People.FindAsync(id);
                
                if (person == null)
                {
                    return NotFound($"Person with id: ${id} not found"); //returns status code 404
                }

                return Ok(person); //returns status code 200
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] Person person)
        {
            try
            {
                if (id != person.Id)
                {
                    return BadRequest("Person ID mismatch"); //400 Bad Request
                }

                var personExists = _context.People.Any(p => p.Id == id);

                if (!personExists)
                {
                    return NotFound($"Person with id: ${id} not found"); //404 Not Found
                }

                _context.People.Update(person);
                await _context.SaveChangesAsync();

                return NoContent(); //204 Empty
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                var person = await _context.People.FindAsync(id);

                if (person == null)
                {
                    return NotFound($"Person with id: ${id} not found"); //404 Not Found
                }

                _context.People.Remove(person);
                await _context.SaveChangesAsync();

                return NoContent(); //204 Empty
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
