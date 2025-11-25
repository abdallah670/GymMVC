using GymDAL.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Progress_and_Notification
{
    public class ReportLog : BaseEntity
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string ReportType { get; set; } // "PDF", "CSV"

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? ReportName { get; set; }
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

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
    }
       public static class ReportTypes
    {
        public const string PDF = "PDF";
        public const string CSV = "CSV";
        public const string Excel = "Excel";
        public const string HTML = "HTML";
        public const string JSON = "JSON";
    }

    public static class ReportNames
    {
        public const string MemberProgress = "Member Progress Report";
        public const string WorkoutCompletion = "Workout Completion Report";
        public const string DietAdherence = "Diet Adherence Report";
        public const string FinancialSummary = "Financial Summary Report";
        public const string AttendanceReport = "Attendance Report";
        public const string TrainerPerformance = "Trainer Performance Report";
        public const string SystemUsage = "System Usage Report";
    }
}

