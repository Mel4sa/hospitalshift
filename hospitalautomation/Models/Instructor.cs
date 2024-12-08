using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace hospitalautomation.Models
{
    public class Instructor:BaseEntity
    {
       
        public int DepartmantId { get; set; }
        [ForeignKey("DepartmentId")]

        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Required]
        [StringLength(11)]
        public int TelNo { get; set; }

        [Required]
        [StringLength(50)]
        public required string Email { get; set; }

        [Required]
        [StringLength(50)]
        public required string Address { get; set; }
    }
}