using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM
{
    public class TempRegistrationVM
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string? OtpCode { get; set; } 
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string? FitnessGoal { get; set; }
        public string? ActivityLevel { get; set; }
        public int? SelectedMembershipId { get; set; }
        public bool IsOtpVerified { get; set; } = false;
        public string RegistrationStatus { get; set; } = "Pending";
    }
}
