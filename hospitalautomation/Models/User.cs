using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using hospitalautomation.Models.Enum;

namespace hospitalautomation.Models
{
    public class User : BaseEntity
    {



        [Required]
        [StringLength(50)]
        public required string Email { get; set; }

        [Required]
        [StringLength(50)]
        public required string Password { get; set; }

          [Required]
        public UserRole Role { get; set; }   
    }
}