using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace hospitalautomation.Models
{
    public class MailEmergency
    {
        [Key]
        public int MailEmergencyId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]

        public int EmergencyId { get; set; }
        [ForeignKey("EmergencyId")]


        [Required]
        [StringLength(50)]
        public required string MailContent { get; set; }
    }
}