using System;
using System.Collections.Generic;
using System.Text;

namespace DTO
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
