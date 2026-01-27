

namespace GymDAL.Entities.Users
{
    public class Trainer : ApplicationUser
    {
        public int? ExperienceYears { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }
        public IEnumerable<TrainerReview> Reviews { get; set; }
       
    }
}
