using System.ComponentModel;

namespace VetClinicModelLibTest
{
    public class Service : ModelBase
    {
        [DisplayName("Название")]
        public string Title { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("Цена")]
        public double Price { get; set; }
    }
}
