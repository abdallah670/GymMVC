

using GymDAL.Entities.Communication;

namespace GymDAL.Entities.Users
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Notification>? Notifications { get; set; } = new List<Notification>();
        
    }



   
}