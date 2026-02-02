using System;
using Microsoft.AspNetCore.Http;
using GymBLL.ModelVM.Member;

namespace GymBLL.ModelVM.Member
{
    public class MemberProfileDisplayVM
    {
        public IFormFile ProfilePictureFile { get; set; }
        public string ProfilePicture { get; set; }
        public string MemberId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public double CurrentWeight { get; set; }
        public double Height { get; set; }
        public string FitnessGoal { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public double BMI => Height > 0 ? CurrentWeight / Math.Pow(Height / 100, 2) : 0;
        public MemberDashboardSubscriptionVM SubscriptionStatus { get; set; }
    }
}
