using GymBLL.ModelVM.User.AppUser;

namespace GymBLL.ModelVM.User.Trainer
{
    public class TrainerVM: UserVM
    {
        [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50")]
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }
        
    }
}