using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymDAL.Entities.Users
{
    public class TrainerReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TrainerId { get; set; }
        [ForeignKey("TrainerId")]
        public virtual Trainer Trainer { get; set; }

        [Required]
        public string MemberId { get; set; }
        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
