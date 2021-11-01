using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VetClinicModelLibTest
{
    public class Animal : ModelBase
    {
        [DisplayName("ID хозяина")]
        public int OwnerId { get; set; }

        [DisplayName("Кличка")]
        public string Name { get; set; }

        [DisplayName("Вид"), EnumDataType(typeof(AnimalTypes))]
        public AnimalTypes Type { get; set; }

        [DisplayName("Дата рождения")]
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
