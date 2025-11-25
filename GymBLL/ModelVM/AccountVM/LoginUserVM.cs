using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenoBLL.ModelVM.AccountVM
{
    public class LoginUserVM
    {
       public string Email { get; set; }
       public string Password { get; set; }
        public bool RememberMe { get; set; }


    }
}
