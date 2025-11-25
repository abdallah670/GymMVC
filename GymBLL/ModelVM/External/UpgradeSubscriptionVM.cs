using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.External
{
    public class UpgradeSubscriptionVM
    {
        public int CurrentSubscriptionId { get; set; }
        
        [Required(ErrorMessage = "Member ID is required")]
        public string MemberId { get; set; }
        
        public string MemberName { get; set; }
        
        public int CurrentMembershipId { get; set; }
        
        public string CurrentMembershipType { get; set; }
        
        public decimal CurrentPrice { get; set; }
        
        [Required(ErrorMessage = "Please select a new membership plan")]
        public int NewMembershipId { get; set; }
        
        public string NewMembershipType { get; set; }
        
        public decimal NewPrice { get; set; }
        
        public decimal PriceDifference { get; set; }
        
        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(20)]
        public string PaymentMethod { get; set; }
        
        public DateTime CurrentEndDate { get; set; }
        
        public DateTime NewEndDate { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; }
        
        // Available upgrade options
        public List<MembershipVM> AvailableUpgrades { get; set; } = new List<MembershipVM>();
        
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
