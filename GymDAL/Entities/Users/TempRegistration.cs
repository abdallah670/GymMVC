using GymDAL.Enums;
using GymDAL.Entities.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace GymDAL.Entities.Users
{
    public class TempRegistration : BaseEntity
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public bool IsOtpVerified { get; set; } = false;

        // Step 2 Data (JSON or individual fields? Individual is safer for query)
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        
        public string? PhoneNumber { get; set; }

        // Fitness Profile
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string? FitnessGoal { get; set; }
        public string? ActivityLevel { get; set; }

        // Selected Membership
        public int? SelectedMembershipId { get; set; }

        // Status
        public string RegistrationStatus { get; set; } = "Pending"; // Pending, Completed, Expired
    }
}
