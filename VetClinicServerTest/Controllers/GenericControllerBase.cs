using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VetClinicServerTest.Models;

namespace VetClinicServerTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericControllerBase<T> : ControllerBase where T : ModelBase
    {
        private readonly ClinicContext _context;

        public GenericControllerBase(ClinicContext context)
        {
            _context = context;
        }

        // GET: api/{entity}
        [HttpGet]
        public virtual async Task<IActionResult> GetMany()
        {
            var entities = await _context.Set<T>().ToListAsync();
            return Ok(entities);
        }

        // GET: api/{entity}/5
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetOne(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // PUT: api/{entity}/5
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(int id, T entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EntryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOne", new { id = entity.Id }, entity);
        }

        // POST: api/{entity}
        [HttpPost]
        public virtual async Task<IActionResult> Post(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOne", new { id = entity.Id }, entity);
        }

        // DELETE: api/{entity}/5
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var entry = await _context.Set<T>().FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }

            _context.Set<T>().Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> EntryExists(int id)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id == id);
        }
    }
}
