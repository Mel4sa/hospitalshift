using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hospitalautomation.Models
{
    public class Shift : BaseEntity
    {
        [Required]
        public int AssistantId { get; set; }

        [ForeignKey(nameof(AssistantId))]
        public Assistant Assistant { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }

        [Required]
        public DateTime ShiftDate { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
