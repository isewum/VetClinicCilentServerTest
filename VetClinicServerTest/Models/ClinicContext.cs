using Microsoft.EntityFrameworkCore;
using System;
using VetClinicModelLibTest;

namespace VetClinicServerTest.Models
{
    public class ClinicContext : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<Service> Services { get; set; }

        public ClinicContext(DbContextOptions<ClinicContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            
            if (Database.EnsureCreated())
                DbInit();
        }

        private void DbInit()
        {
            var now = DateTime.UtcNow;
            for (int i = 1; i < 4; i++)
            {
                var owner = new Owner() { FirstName = $"ownerFirstName{i}", LastName = $"ownerLastName{i}", Phone = "123456", CreatedAt = now };
                Owners.Add(owner);

                var doctor = new Doctor() { FirstName = $"doctorFirstName{i}", LastName = $"doctorLastName{i}", Phone = "123456", CreatedAt = now };
                Doctors.Add(doctor);

                var birth = now - TimeSpan.FromDays(365 * i);
                var animal = new Animal() { Name = $"animalName{i}", OwnerId = i, Type = (AnimalTypes)i, BirthDate = birth, CreatedAt = now };
                Animals.Add(animal);

                var done = now - TimeSpan.FromDays(3 * i);
                var vaccine = new Vaccine() { Title = $"vaccineTitle{i}", AnimalId = i, DoctorId = i, DoneAt = done, CreatedAt = now };
                Vaccines.Add(vaccine);

                var servise = new Service() { Title = $"serviceTitle{i}", Description = $"serviceDescription{i}", Price = 150 * i, CreatedAt = now };
                Services.Add(servise);
            }
            SaveChanges();
        }
    }
}