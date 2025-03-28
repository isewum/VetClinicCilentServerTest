﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using VetClinicModelLibTest;
using VetClinicServerTest.Models;

namespace VetClinicServerTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericControllerBase<T> : ControllerBase where T : ModelBase
    {
        protected readonly ClinicContext _context;
        protected readonly DbSet<T> dbSet;

        public GenericControllerBase(ClinicContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        // GET: api/{entity}
        [HttpGet]
        public virtual async Task<IActionResult> GetMany()
        {
            var entities = await dbSet.ToListAsync();
            string jsonString = JsonConvert.SerializeObject(entities);
            return Ok(jsonString);
        }

        // GET: api/{entity}/5
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetOne(int id)
        {
            var entity = await dbSet.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            string jsonString = JsonConvert.SerializeObject(entity);
            return Ok(jsonString);
        }

        // POST: api/{entity}
        [HttpPost]
        public virtual async Task<IActionResult> Post(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOne", new { id = entity.Id }, entity);
        }

        // PUT: api/{entity}/5
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(int id, T entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            var oldEntity = await dbSet.FindAsync(id);
            if (entity.CreatedAt != oldEntity.CreatedAt)
                entity.CreatedAt = oldEntity.CreatedAt;

            _context.Entry(oldEntity).State = EntityState.Detached;
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

            string jsonString = JsonConvert.SerializeObject(entity);
            return Ok(jsonString);
        }

        // DELETE: api/{entity}/5
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var entry = await dbSet.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }

            dbSet.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<bool> EntryExists(int id)
        {
            return await dbSet.AnyAsync(e => e.Id == id);
        }
    }
}
