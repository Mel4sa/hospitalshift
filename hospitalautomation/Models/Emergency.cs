using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace hospitalautomation.Models
{
    public class Emergency:BaseEntity
    {
       

        public int UserId { get; set; }
        [ForeignKey("UserId")]

        [Required]
        [StringLength(50)]
        public required string Title { get; set; }

        [Required]
        [StringLength(500)]
        public required string Content { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime CreatedAt { get; set; }

    }
}
