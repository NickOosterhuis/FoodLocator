using System;
using System.Collections.Generic;
using System.Text;

namespace DTO
{
    public class UserAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Confirmed { get; set; }
        public string ProfilePicture { get; set; }
        public bool LockedOut { get; set; }
        public bool RestaurantFeatured { get; set; }
        //TODO add more fields when we're sure

    }
}
