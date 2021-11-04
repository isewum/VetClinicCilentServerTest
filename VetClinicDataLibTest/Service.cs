using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VetClinicModelLibTest
{
    public class Service : ModelBase
    {
        [DisplayName("Название"), Required(AllowEmptyStrings = false), StringLength(30, MinimumLength = 3)]
        public string Title { get; set; }

        [DisplayName("Описание"), Required, StringLength(200)]
        public string Description { get; set; }

        [DisplayName("Цена"), Range(0, double.MaxValue), Required]
        public double Price { get; set; }
    }
}
