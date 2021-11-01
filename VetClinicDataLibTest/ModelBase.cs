using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VetClinicModelLibTest
{
    public class ModelBase
    {
        [Key, DisplayName("ID"), ReadOnly(true)]
        public int Id { get; set; }

        [DisplayName("Дата создания"), ReadOnly(true)]
        public DateTime CreatedAt { get; set; }
    }
}
