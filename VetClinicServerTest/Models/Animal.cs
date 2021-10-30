using System;

namespace VetClinicServerTest.Models
{
    public class Animal : ModelBase
    {
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public AnimalTypes Type { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public enum AnimalTypes
    {
        Dog,
        Cat,
        Hamster,
        Bear
    }
}
