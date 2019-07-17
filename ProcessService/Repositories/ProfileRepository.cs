using System;
using ProcessService.Context;
using ProcessService.Entities;
using ProcessService.Interfaces;

namespace ProcessService.Repositories
{
    public class ProfileRepository : GenericRepository<Profile>, IProfileRepository
    {
        public ProfileRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
