using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthModels
{
    public class LoginModel
    {
        public string UsernameOrEmail { set; get; }
        public string Password { set; get; }
    }
}
