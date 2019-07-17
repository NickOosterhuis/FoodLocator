using System;
using ProcessService.Context;
using ProcessService.Entities;
using ProcessService.Interfaces;

namespace ProcessService.Repositories
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
