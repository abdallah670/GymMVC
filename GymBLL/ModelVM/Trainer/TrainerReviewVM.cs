using System;
using System.ComponentModel.DataAnnotations;

namespace GymBLL.ModelVM.Trainer
{
    public class TrainerReviewVM
    {
        public int Id { get; set; }

        public string TrainerId { get; set; }
        public string? TrainerName { get; set; }
        
        public string MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? MemberPicture { get; set; }
        public DateTime ? JoinDate { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5 stars.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
