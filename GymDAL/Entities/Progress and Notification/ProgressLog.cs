using GymDAL.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Progress_and_Notification
{
    public class ProgressLog : BaseEntity
    {
        [Required]
        public string MemberId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Range(0, 500)]
        public decimal Weight { get; set; } // kg

        [Range(0, 100)]
        public decimal? BodyFat { get; set; } // percentage

        [Range(0, 200)]
        public decimal? Chest { get; set; } // cm

        [Range(0, 200)]
        public decimal? Waist { get; set; } // cm

        [Range(0, 200)]
        public decimal? Hips { get; set; } // cm

        [Range(0, 100)]
        public decimal? Arms { get; set; } // cm

        [Range(0, 100)]
        public int WorkoutAdherence { get; set; } // percentage

        [Range(0, 100)]
        public int MealAdherence { get; set; } // percentage

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? ProgressPhoto { get; set; } // file path

        // Navigation properties
        public virtual Member Member { get; set; }
        public bool ToggleStatus(string DeletedBy)
        {
            if (!string.IsNullOrEmpty(DeletedBy))
            {
                this.IsActive = !this.IsActive;
                this.DeletedBy = DeletedBy;
                this.DeletedAt = DateTime.UtcNow;
                return true;
            }
            return false;
        }
    }

  
   

}
