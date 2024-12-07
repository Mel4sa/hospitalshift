using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace hospitalautomation.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }


        [Required]
        [StringLength(50)]
        public required string Email { get; set; }

        [Required]
        [StringLength(50)]
        public required string Password { get; set; }
    }
}