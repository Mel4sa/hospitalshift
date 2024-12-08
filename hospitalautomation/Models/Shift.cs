using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace hospitalautomation.Models
{
    public class Shift : BaseEntity
    {



        public int AssistantId { get; set; }
        [ForeignKey("AsistantId")]
        public int DepartmantId { get; set; }
        [ForeignKey("DepartmentId")]

        [Required]
        [StringLength(50)]
        public DateTime StartTime { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime EndTime { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime ShiftDate { get; set; }

    }
}