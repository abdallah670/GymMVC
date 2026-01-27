using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Trainer
{
    public class TrainerProfileVM
    {
        public string Id { get; set; }

        [Required]
        public string FullName { get; set; }


        public string Email { get; set; }

        public string Phone { get; set; }
        public string ProfilePicture { get; set; }
        public int? ExperienceYears { get; set; }
        public string Bio { get; set; }


        [Display(Name = "Profile Picture")]
        [DataType(DataType.Upload)]
        public IFormFile ProfileImageFile { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
