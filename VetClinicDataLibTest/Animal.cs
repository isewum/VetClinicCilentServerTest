using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VetClinicModelLibTest
{
    public class Animal : ModelBase
    {
        [DisplayName("ID хозяина"), Range(1, int.MaxValue), Required]
        public int OwnerId { get; set; }

        [DisplayName("Кличка"), Required(AllowEmptyStrings = false), StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [DisplayName("Вид"), EnumDataType(typeof(AnimalTypes)), Required]
        public AnimalTypes Type { get; set; }

        [DisplayName("Дата рождения"), DataType(DataType.Date), Required]
        public DateTime BirthDate { get; set; }

        public override bool Equals(ModelBase entity)
        {
            if (!(entity is Animal))
                return base.Equals(entity);

            Animal animal = entity as Animal;
            return OwnerId == animal.OwnerId
                && Type == animal.Type
                && BirthDate == animal.BirthDate
                && string.Equals(Name, animal.Name)
                && base.Equals(entity);
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public enum AnimalTypes
    {
        Dog,
        Cat,
        Hamster,
        Bear
    }
}
