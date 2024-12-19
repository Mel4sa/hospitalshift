using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hospitalautomation.Models
{
    public class Interview : BaseEntity
    {
   
        public int AssistantId { get; set; }

        [ForeignKey("AssistantId")]
        public virtual Assistant Assistant { get; set; } 

    
        public int InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public virtual Instructor Instructor { get; set; } 

       
        [Required]
        public DateTime StartTime { get; set; }


        [Required]
        public DateTime EndTime { get; set; }


        [Required]
        public DateTime ShiftDate { get; set; }
    }
}
