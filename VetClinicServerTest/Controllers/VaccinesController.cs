using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VetClinicModelLibTest;
using VetClinicServerTest.Models;

namespace VetClinicServerTest.Controllers
{
    public class VaccinesController : GenericControllerBase<Vaccine>
    {
        public VaccinesController(ClinicContext context) : base(context) { }

        // POST: api/{entity}
        [HttpPost]
        public override async Task<IActionResult> Post(Vaccine entity)
        {
            if (!(await TryAttachRef(entity)))
                return BadRequest();

            return await base.Post(entity);
        }

        // PUT: api/{entity}/5
        [HttpPut("{id}")]
        public override async Task<IActionResult> Put(int id, Vaccine entity)
        {
            if (!(await TryAttachRef(entity)))
                return BadRequest();

            return await base.Put(id, entity);
        }

        private async Task<bool> TryAttachRef(Vaccine entity)
        {
            Doctor doctor = await _context.Doctors.FindAsync(entity.DoctorId);
            Animal animal = await _context.Animals.FindAsync(entity.AnimalId);

            if (doctor == null || animal == null)
                return false;

            entity.Doctor = doctor;
            entity.Animal = animal;
            _context.Entry(doctor).State = EntityState.Unchanged;
            _context.Entry(animal).State = EntityState.Unchanged;

            return true;
        }
    }
}