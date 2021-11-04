using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VetClinicModelLibTest
{
    public class Vaccine : ModelBase
    {
        [DisplayName("Название"), Required(AllowEmptyStrings = false), StringLength(30, MinimumLength = 3)]
        public string Title { get; set; }

        [DisplayName("ID животного"), Range(1, int.MaxValue), Required]
        public int AnimalId { get; set; }

        [DisplayName("ID ветеринара"), Range(1, int.MaxValue), Required]
        public int DoctorId { get; set; }

        [DisplayName("Дата постановки"), Required]
        public DateTime DoneAt { get; set; }

        public override bool Equals(ModelBase entity)
        {
            if (!(entity is Vaccine))
                return base.Equals(entity);

            Vaccine vaccine = entity as Vaccine;
            return AnimalId == vaccine.AnimalId
                && DoctorId == vaccine.DoctorId
                && DoneAt == vaccine.DoneAt
                && string.Equals(Title, vaccine.Title)
                && base.Equals(entity);
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
