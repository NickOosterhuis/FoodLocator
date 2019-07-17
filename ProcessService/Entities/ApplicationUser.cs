using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProcessService.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey("Profile")]
        public string ProfileId { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
