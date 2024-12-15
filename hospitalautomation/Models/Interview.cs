using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hospitalautomation.Models
{
    public class Interview : BaseEntity
    {
        // AssistantId için ForeignKey
        public int AssistantId { get; set; }

        [ForeignKey("AssistantId")]
        public virtual Assistant Assistant { get; set; } // Navigasyon özelliği

        // InstructorId için ForeignKey
        public int InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public virtual Instructor Instructor { get; set; } // Navigasyon özelliği

        // Randevu Başlangıç Zamanı
        [Required]
        public DateTime StartTime { get; set; }

        // Randevu Bitiş Zamanı
        [Required]
        public DateTime EndTime { get; set; }

        // Randevu Tarihi
        [Required]
        public DateTime ShiftDate { get; set; }
    }
}
