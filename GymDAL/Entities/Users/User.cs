

namespace GymDAL.Entities.Users
{
    public class ApplicationUser : IdentityUser,IGlobal
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfilePicture { get; set; }


        public DateTime? DeletedAt { get; set; } = DateTime.UtcNow;


        public string? CreatedBy { get; set; }
        public string? DeletedBy { get; set; }

        public string? UpdatedBy { get; set; }


        public bool IsActive { get; set; } = true;


        // Navigation properties

        public virtual ICollection<ReportLog>? GeneratedReports { get; set; } = new List<ReportLog>();
        public virtual ICollection<Notification>? Notifications { get; set; } = new List<Notification>();
        public ApplicationUser()
        {
        }
        public ApplicationUser( string fullName,string? Address, DateTime? DateOfBirth, string? Phone , string? ProfilePicture, string createdBy)
           
        {
           
            FullName = fullName;
         
            CreatedAt = DateTime.UtcNow;
            this.Address = Address;
            this.DateOfBirth = DateOfBirth;
            this.Phone = Phone;
            CreatedBy= createdBy;
            this.ProfilePicture = ProfilePicture;
        }
        public ApplicationUser( string fullName, string createdBy)
        {
           
            FullName = fullName;
         
            CreatedAt = DateTime.UtcNow;
            this.CreatedBy = createdBy;
        }
        public ApplicationUser( string UserName,string Password ,string email, string createdBy)
        {
           
            this. UserName = UserName;
            Email = email;
            PasswordHash = Password;
          
            CreatedAt = DateTime.UtcNow;
            this.CreatedBy = createdBy;
        }

        public bool ToggleStatus(string DeletedBy)
        {
          if(!string.IsNullOrEmpty(DeletedBy))
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