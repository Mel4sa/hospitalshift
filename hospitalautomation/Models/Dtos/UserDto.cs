using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using hospitalautomation.Models.Enum;

namespace hospitalautomation.Models.Dtos
{
    public class UserDto
    {
        [Required]
        [StringLength(50)]
        public required string Email { get; set; }

        [Required]
        [StringLength(50)]
        public required string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public required string Address { get; set; }

        [Required]
        [StringLength(11)]
        public required string TelNo { get; set; }
    }
}