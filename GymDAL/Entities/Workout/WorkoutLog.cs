using GymDAL.Entities.Core;
using GymDAL.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymDAL.Entities.Workout
{
    public class WorkoutLog : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MemberId { get; set; }
        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }

        public int? WorkoutPlanId { get; set; }
        [ForeignKey("WorkoutPlanId")]
        public virtual WorkoutPlan WorkoutPlan { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int DurationMinutes { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public virtual ICollection<WorkoutLogEntry> Entries { get; set; } = new List<WorkoutLogEntry>();
    }
}
