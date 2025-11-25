using System.ComponentModel.DataAnnotations;
using GymDAL.Entities.Users;

namespace GymDAL.Entities
{
    public class Payment : BaseEntity
    {
        [Required]
        public string MemberId { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentType { get; set; } // "Membership", "PersonalTraining", "Classes"

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(20)]
        public string Currency { get; set; } = "USD";

        [Required]
        [StringLength(20)]
        public string PaymentMethod { get; set; } // "CreditCard", "DebitCard", "Cash", "BankTransfer"

        [StringLength(50)]
        public string? TransactionId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // "Pending", "Completed", "Failed", "Refunded"

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? ProcessedDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Billing information
        [StringLength(200)]
        public string? BillingName { get; set; }

        [StringLength(500)]
        public string? BillingAddress { get; set; }

        [StringLength(100)]
        public string? BillingEmail { get; set; }

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