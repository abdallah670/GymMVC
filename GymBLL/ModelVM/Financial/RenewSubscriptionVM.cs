using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Financial
{
    public class RenewSubscriptionVM
    {
        public int SubscriptionId { get; set; }

        [Required(ErrorMessage = "Member ID is required")]
        public string MemberId { get; set; }

        public string MemberName { get; set; }

        public int MembershipId { get; set; }

        public string MembershipType { get; set; }

        [StringLength(20)]
        public string PaymentMethod { get; set; } // "CreditCard", "DebitCard", "Cash"

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public DateTime CurrentEndDate { get; set; }

        public DateTime NewEndDate { get; set; }

        public int DurationInMonths { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        // Billing Information
        [StringLength(200)]
        public string BillingName { get; set; }

        [StringLength(500)]
        public string BillingAddress { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string BillingEmail { get; set; }
    }
}
