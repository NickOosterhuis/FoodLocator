using System;
using System.Collections.Generic;
using System.Text;

namespace DTO
{
    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }
        public bool RestaurantOwner { get; set; } = true;
    }
}
