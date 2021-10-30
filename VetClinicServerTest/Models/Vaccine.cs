using System;

namespace VetClinicServerTest.Models
{
    public class Vaccine : ModelBase
    {
        public string Title { get; set; }
        public int AnimalId { get; set; }
        public int DoctorId { get; set; }
        public DateTime DoneAt { get; set; }
    }
}
