using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProcessService.Entities;

namespace ProcessService.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Seed();
            base.OnModelCreating(builder);
        }
    }
}
