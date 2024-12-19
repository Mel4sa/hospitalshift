using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace hospitalautomation.Models
{
   public class MailEmergency : BaseEntity
    {
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int EmergencyId { get; set; }

        [ForeignKey("EmergencyId")]
        public Emergency Emergency { get; set; } 

        [Required]
        [StringLength(500)]
        public required string MailContent { get; set; }
    }
}