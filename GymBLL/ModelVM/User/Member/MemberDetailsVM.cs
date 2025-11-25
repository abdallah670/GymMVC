using GymBLL.ModelVM.External;
using GymBLL.ModelVM.User.AppUser;

namespace GymBLL.ModelVM.User.Member
{
    public class MemberDetailsVM:RegisterUserVM
    {   
        public int MembershipId { get; set; }
        public bool? Gender { get; set; }
    }
}