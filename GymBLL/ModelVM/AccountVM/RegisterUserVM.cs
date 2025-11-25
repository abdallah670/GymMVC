using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenoBLL.ModelVM.AccountVM
{
    public class RegisterUserVM
    {
    
       public string FullName { get; set; }
       public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
      
        public string Email { get; set; }
        public string? Phone { get; set; }
        public bool RememberMe { get; set; }

    }
}
