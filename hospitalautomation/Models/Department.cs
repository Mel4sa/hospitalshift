using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hospitalautomation.Models
{
    public class Department : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [StringLength(50)]
        public int PatientCount { get; set; }

        [Required]
        [StringLength(50)]
        public int EmptybedCount { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; } 
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}
