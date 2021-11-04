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
    }

    public enum AnimalTypes
    {
        [Display(Name = "Собака")]
        Dog,
        [Display(Name = "Кот")]
        Cat,
        [Display(Name = "Хомяк")]
        Hamster,
        [Display(Name = "Медведь")]
        Bear
    }
}
