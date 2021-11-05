using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VetClinicModelLibTest
{
    public class ModelBase : ICloneable
    {
        [Key, DisplayName("ID"), ReadOnly(true)]
        public int Id { get; set; }

        [DisplayName("Дата создания"), ReadOnly(true)]
        public DateTime CreatedAt { get; set; }

        public virtual bool Equals(ModelBase entity)
        {
            return Id == entity.Id && CreatedAt == entity.CreatedAt;
        }
        
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
