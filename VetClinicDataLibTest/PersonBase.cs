using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VetClinicModelLibTest
{
    public class PersonBase : ModelBase
    {
        [DisplayName("Имя"), Required(AllowEmptyStrings = false), StringLength(30, MinimumLength = 3)]
        public string FirstName { get; set; }

        [DisplayName("Фамилия"), Required(AllowEmptyStrings = false), StringLength(30, MinimumLength = 3)]
        public string LastName { get; set; }

        [DisplayName("Номер телефона"), Phone, Required]
        public string Phone { get; set; }
    }
}
