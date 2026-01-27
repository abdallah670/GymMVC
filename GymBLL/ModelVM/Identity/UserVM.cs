using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymBLL.ModelVM.Identity
{
    public class UserVM
    {
        public string Id { get; set; }

        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }


        public string? ProfilePicture { get; set; }
        // Add this property for file upload
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfileImageFile { get; set; }
        public string? Password { get; set; }
    }
}
