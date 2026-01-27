using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Financial
{
    public class SubscriptionVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Member is required")]
        public string MemberId { get; set; }

        public string? MemberName { get; set; }

        [Required(ErrorMessage = "Membership is required")]
        public int MembershipId { get; set; }

        public string? MembershipType { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = "Active";

        public int PaymentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
