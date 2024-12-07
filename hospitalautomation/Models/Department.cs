using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace hospitalautomation.Models
{
    public class Department
    {
        [Key]
        public int DepartmantId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [StringLength(50)]
        public int PatientCount { get; set; }

         [Required]
        [StringLength(50)]
        public int EmptybedCount { get; set; }

    }
}