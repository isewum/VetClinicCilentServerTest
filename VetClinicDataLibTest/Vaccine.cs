using System;
using System.ComponentModel;

namespace VetClinicModelLibTest
{
    public class Vaccine : ModelBase
    {
        [DisplayName("Название")]
        public string Title { get; set; }

        [DisplayName("ID животного")]
        public int AnimalId { get; set; }

        [DisplayName("ID ветеринара")]
        public int DoctorId { get; set; }

        [DisplayName("Дата постановки")]
        public DateTime DoneAt { get; set; }
    }
}
