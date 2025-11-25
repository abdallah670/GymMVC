using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Users
{
    [Table("Admins")]
    public class Admin:ApplicationUser
    {
        // SUPER ADMIN FLAG - Only one should be true
        public bool IsSuperAdmin { get; set; } = false;
        public virtual ICollection<SystemLog>? SystemLogs { get; set; } = new List<SystemLog>();


    }

}
