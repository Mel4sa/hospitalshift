using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace hospitalautomation.Models
{
    public class Interview:BaseEntity
    {


        public int AssistantId { get; set; }
        [ForeignKey("AsstiantId")]

        public int InstructorId { get; set; }
        [ForeignKey("InstructorId")]

        [Required]
        [StringLength(50)]
        public DateTime StartTime { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime EndTime { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime ShiftDate { get; set; }

        [Required]
        [StringLength(50)]
        public required string Status { get; set; }
    }
}