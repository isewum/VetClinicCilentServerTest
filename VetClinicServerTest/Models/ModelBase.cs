using System;
using System.ComponentModel.DataAnnotations;

namespace VetClinicServerTest.Models
{
    public class ModelBase
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
