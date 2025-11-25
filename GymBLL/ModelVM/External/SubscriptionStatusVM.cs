using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.External
{
    public class SubscriptionStatusVM
    {
        public int SubscriptionId { get; set; }
        public string MemberId { get; set; }
        public string MembershipType { get; set; }
        public string Status { get; set; } // "Active", "Expiring", "Expired"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRemaining { get; set; }
        public bool IsExpiringSoon { get; set; } // True if <= 7 days remaining
        public bool IsExpired { get; set; }
        public bool CanRenew { get; set; }
        public bool CanUpgrade { get; set; }
        public decimal RenewalPrice { get; set; }
        public string StatusBadgeClass { get; set; } // CSS class for status badge
        public string ProgressPercentage { get; set; } // For progress bar
    }
}
