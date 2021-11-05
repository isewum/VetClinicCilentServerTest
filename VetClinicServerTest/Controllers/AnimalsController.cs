using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VetClinicModelLibTest;
using VetClinicServerTest.Models;

namespace VetClinicServerTest.Controllers
{
    public class AnimalsController : GenericControllerBase<Animal>
    {
        public AnimalsController(ClinicContext context) : base (context) { }

        // POST: api/{entity}
        [HttpPost]
        public override async Task<IActionResult> Post(Animal entity)
        {
            if (!(await TryAttachRef(entity)))
                return BadRequest();

            return await base.Post(entity);
        }

        // PUT: api/{entity}/5
        [HttpPut("{id}")]
        public override async Task<IActionResult> Put(int id, Animal entity)
        {
            if (!(await TryAttachRef(entity)))
                return BadRequest();

            return await base.Put(id, entity);
        }

        private async Task<bool> TryAttachRef(Animal entity)
        {
            Owner owner = await _context.Owners.FindAsync(entity.OwnerId);
            if (owner == null)
                return false;

            entity.Owner = owner;
            _context.Entry(owner).State = EntityState.Unchanged;
            return true;
        }
    }
}