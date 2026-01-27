using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymDAL.Entities.Core;

namespace GymDAL.Entities.Users
{
    public class WeightLog : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MemberId { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }

        [Required]
        [Range(20, 300)]
        public double Weight { get; set; }

        [Required]
        public DateTime DateRecorded { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
