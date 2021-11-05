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

        public override bool Equals(ModelBase entity)
        {
            if (!(entity is Service))
                return base.Equals(entity);

            Service service = entity as Service;
            return Price == service.Price
                && string.Equals(Title, service.Title)
                && string.Equals(Description, service.Description)
                && base.Equals(entity);
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
